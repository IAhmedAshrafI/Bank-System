using Bank.Shared.Events;
using Bank.TransactionsLogService.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Bank.TransactionsLogService.Service
{
	public sealed class TransactionLogConsumerService : BackgroundService, IAsyncDisposable
	{
		private readonly IServiceProvider _services;
		private readonly IConfiguration _cfg;
		private readonly ILogger<TransactionLogConsumerService> _logger;

		private IConnection? _connection;
		private IChannel? _channel;
		private string? _consumerTag;

		public TransactionLogConsumerService(IServiceProvider services, IConfiguration cfg, ILogger<TransactionLogConsumerService> logger)
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
				UserName = _cfg["RabbitMQ:UserName"] ?? "bankuser", // <- don't use guest over the network
				Password = _cfg["RabbitMQ:Password"] ?? "bankpass",
				VirtualHost = _cfg["RabbitMQ:VirtualHost"] ?? "/",
				Port = int.TryParse(_cfg["RabbitMQ:Port"], out var p) ? p : AmqpTcpEndpoint.UseDefaultPort,
				AutomaticRecoveryEnabled = true
			};

			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

			var opts = new CreateChannelOptions(
				publisherConfirmationsEnabled: false,              // not needed for consumers
				publisherConfirmationTrackingEnabled: false
			);

			_channel = await _connection.CreateChannelAsync(opts, ct);

			await _channel.QueueDeclareAsync(
				queue: "transaction-log-queue",
				durable: true,
				exclusive: false,
				autoDelete: false,
				arguments: null,
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
					var @event = Newtonsoft.Json.JsonConvert.DeserializeObject<TransactionCreatedEvent>(json);

					using var scope = _services.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<TransactionLogDbContext>();

					db.TransactionLogs.Add(new TransactionLog
					{
						Id = @event!.Id,
						AccountId = @event.AccountId,
						Amount = @event.Amount,
						Type = @event.Type,
						Timestamp = @event.Timestamp,
						SourceService = @event.SourceService
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

			_consumerTag = await _channel.BasicConsumeAsync(
				queue: "transaction-log-queue",
				autoAck: false,
				consumerTag: string.Empty,
				noLocal: false,
				exclusive: false,
				arguments: null,
				consumer: consumer,
				cancellationToken: stoppingToken);
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
