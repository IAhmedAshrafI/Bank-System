using Bank.Shared.Events;
using LoanApprovalService.Entities;
using LoanApprovalService.Entities.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace LoanApprovalService.Services
{
	public sealed class LoanRequestConsumerService : BackgroundService, IAsyncDisposable
	{
		private readonly IServiceProvider _services;
		private readonly IConfiguration _cfg;
		private readonly ILogger<LoanRequestConsumerService> _logger;

		private IConnection? _connection;
		private IChannel? _channel;
		private string? _consumerTag;

		public LoanRequestConsumerService(IServiceProvider services, IConfiguration cfg, ILogger<LoanRequestConsumerService> logger)
		{
			_services = services;
			_cfg = cfg;
			_logger = logger;
		}

		public override async Task StartAsync(CancellationToken stoppingToken)
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
			), stoppingToken);

			// Ensure queue exists
			await _channel.QueueDeclareAsync(
				queue: QueueNames.LoanApprovals,
				durable: true, exclusive: false, autoDelete: false, arguments: null,
				cancellationToken: stoppingToken
			);

			await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 10, global: false, cancellationToken: stoppingToken);

			await base.StartAsync(stoppingToken);
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
					var @event = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanRequestedEvent>(json);

					using var scope = _services.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<LoanApprovalDbContext>();

					db.LoanApprovals.Add(new LoanApproval
					{
						LoanId = @event!.LoanId,
						AccountId = @event.AccountId,
						Amount = @event.Amount,
						TermMonths = @event.TermMonths,
						AnnualInterestRate = @event.AnnualInterestRate,
						RequestedAt = @event.RequestedAt,
						Status = LoanStatus.PendingApproval
					});

					await db.SaveChangesAsync(stoppingToken);
					await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to process message {Tag}", ea.DeliveryTag);
					await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
				}
			};

			// CONSUME FROM THE EXACT QUEUE WE DECLARED (matches publisher)
			_consumerTag = await _channel.BasicConsumeAsync(
				queue: QueueNames.LoanApprovals,
				autoAck: false,
				consumerTag: string.Empty,
				noLocal: false,
				exclusive: false,
				arguments: null,
				consumer: consumer,
				cancellationToken: stoppingToken
			);
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
