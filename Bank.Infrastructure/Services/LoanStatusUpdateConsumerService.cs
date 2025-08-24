using Bank.Domain.Entities.Enums;
using Bank.Shared.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Services
{
	public sealed class LoanStatusUpdateConsumerService : BackgroundService, IAsyncDisposable
	{
		private readonly IServiceProvider _services;
		private readonly IConfiguration _cfg;
		private readonly ILogger<LoanStatusUpdateConsumerService> _logger;

		private IConnection? _connection;
		private IChannel? _channel;
		private string? _consumerTag;

		public LoanStatusUpdateConsumerService(
			IServiceProvider services,
			IConfiguration cfg,
			ILogger<LoanStatusUpdateConsumerService> logger)
		{
			_services = services;
			_cfg = cfg;
			_logger = logger;
		}

		public override async Task StartAsync(CancellationToken ct)
		{
			var factory = new ConnectionFactory
			{
				HostName = _cfg["RabbitMQ:HostName"] ?? "rabbitmq",
				UserName = _cfg["RabbitMQ:UserName"] ?? "bankuser",
				Password = _cfg["RabbitMQ:Password"] ?? "bankpass",
				VirtualHost = _cfg["RabbitMQ:VirtualHost"] ?? "/",
				Port = int.TryParse(_cfg["RabbitMQ:Port"], out var p) ? p : AmqpTcpEndpoint.UseDefaultPort,
				AutomaticRecoveryEnabled = true
			};

			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
			_channel = await _connection.CreateChannelAsync(new CreateChannelOptions(
				publisherConfirmationsEnabled: false,
				publisherConfirmationTrackingEnabled: false
			), ct);

			await _channel.QueueDeclareAsync(
				queue: QueueNames.LoanStatusUpdates,
				durable: true, exclusive: false, autoDelete: false, arguments: null,
				cancellationToken: ct);

			await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: ct);

			await base.StartAsync(ct);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			if (_channel is null) throw new InvalidOperationException("Channel not initialized.");

			var consumer = new AsyncEventingBasicConsumer(_channel);
			consumer.ReceivedAsync += async (_, ea) =>
			{
				try
				{
					var json = Encoding.UTF8.GetString(ea.Body.ToArray());

					// Try Approved first
					var approved = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanApprovedEvent>(json);
					if (approved is not null && approved.LoanId != Guid.Empty)
					{
						await HandleApprovedAsync(approved, stoppingToken);
						await _channel!.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
						return;
					}

					// Then Rejected
					var rejected = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanRejectedEvent>(json);
					if (rejected is not null && rejected.LoanId != Guid.Empty)
					{
						await HandleRejectedAsync(rejected, stoppingToken);
						await _channel!.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
						return;
					}

					_logger.LogWarning("Unknown message format on {Queue}: {Json}", QueueNames.LoanStatusUpdates, json);
					await _channel!.BasicAckAsync(ea.DeliveryTag, false, stoppingToken); // or Nack(true) if you prefer requeue
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to process message {Tag}", ea.DeliveryTag);
					await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
				}
			};

			_consumerTag = await _channel.BasicConsumeAsync(
				queue: QueueNames.LoanStatusUpdates,
				autoAck: false,
				consumerTag: string.Empty,
				noLocal: false,
				exclusive: false,
				arguments: null,
				consumer: consumer,
				cancellationToken: stoppingToken);
		}

		private async Task HandleApprovedAsync(LoanApprovedEvent e, CancellationToken ct)
		{
			using var scope = _services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			var loan = await db.Loans.FirstOrDefaultAsync(x => x.Id == e.LoanId, ct);
			if (loan == null) return;

			// Only move forward if still pending
			if (loan.Status == LoanStatus.PendingApproval)
				loan.Status = LoanStatus.Approved;

			await db.SaveChangesAsync(ct);

			// Optional: trigger disbursement or schedule repayments here
		}

		private async Task HandleRejectedAsync(LoanRejectedEvent e, CancellationToken ct)
		{
			using var scope = _services.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			var loan = await db.Loans.FirstOrDefaultAsync(x => x.Id == e.LoanId, ct);
			if (loan == null) return;

			if (loan.Status == LoanStatus.PendingApproval)
				loan.Status = LoanStatus.Rejected;

			await db.SaveChangesAsync(ct);
		}

		public override async Task StopAsync(CancellationToken ct)
		{
			try { if (_channel != null && _consumerTag != null) await _channel.BasicCancelAsync(_consumerTag, cancellationToken: ct); } catch { }
			await base.StopAsync(ct);
		}

		public async ValueTask DisposeAsync()
		{
			try { if (_channel != null) { await _channel.CloseAsync(); await _channel.DisposeAsync(); } } catch { }
		}

		public override void Dispose()
		{
			_ = DisposeAsync();
			base.Dispose();
		}
	}
}
