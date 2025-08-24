using Bank.Shared.Events;
using LoanApprovalService.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace LoanApprovalService.Services
{
	public sealed class RabbitMQPublisher : IRabbitMQPublisher, IDisposable, IAsyncDisposable
	{
		private readonly IConnection _connection;
		private readonly IChannel _channel;

		public RabbitMQPublisher(IConfiguration cfg)
		{
			var factory = new ConnectionFactory
			{
				HostName = cfg["RabbitMQ:HostName"] ?? "localhost",
				UserName = cfg["RabbitMQ:UserName"] ?? "guest",
				Password = cfg["RabbitMQ:Password"] ?? "guest",
				VirtualHost = cfg["RabbitMQ:VirtualHost"] ?? "/",
				Port = int.TryParse(cfg["RabbitMQ:Port"], out var p) ? p : AmqpTcpEndpoint.UseDefaultPort,
				AutomaticRecoveryEnabled = true,
				NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
				RequestedConnectionTimeout = TimeSpan.FromSeconds(10)
			};

			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
			_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

			// Ensure the status-updates queue exists
			_channel.QueueDeclareAsync(
				queue: "loan-status-updates",
				durable: true, exclusive: false, autoDelete: false, arguments: null
			).GetAwaiter().GetResult();
		}

		public void Publish<T>(T @event) where T : class
		{
			// Route by event type
			var queue = @event switch
			{
				LoanApprovedEvent => "loan-status-updates",
				LoanRejectedEvent => "loan-status-updates",
				_ => throw new InvalidOperationException($"No queue mapping for {@event.GetType().Name}")
			};

			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

			_channel.QueueDeclareAsync(queue, true, false, false, null)
					.GetAwaiter().GetResult(); // idempotent, but guarantees existence

			_channel.BasicPublishAsync(
				exchange: "",
				routingKey: queue,
				mandatory: false,
				body: body,
				cancellationToken: CancellationToken.None
			).GetAwaiter().GetResult();
		}

		public void Dispose()
		{
			try { _channel?.CloseAsync().GetAwaiter().GetResult(); } catch { }
			try { _channel?.DisposeAsync().AsTask().GetAwaiter().GetResult(); } catch { }
			try { _connection?.Dispose(); } catch { }
		}

		public async ValueTask DisposeAsync()
		{
			if (_channel != null) { await _channel.CloseAsync(); await _channel.DisposeAsync(); }
			_connection?.Dispose();
		}
	}
}
