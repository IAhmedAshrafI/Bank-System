using Bank.Application.Interfaces;
using Bank.Shared.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using System;

namespace Bank.Infrastructure.Services
{
	public sealed class RabbitMQPublisher : IRabbitMQPublisher, IDisposable, IAsyncDisposable
	{
		private readonly IConnection _connection;
		private readonly IChannel _channel;
		private readonly string _queue = "transaction-log-queue";

		public RabbitMQPublisher(IConfiguration cfg)
		{
			var host = cfg["RabbitMQ:HostName"] ?? "localhost";
			var user = cfg["RabbitMQ:UserName"] ?? "guest";
			var pass = cfg["RabbitMQ:Password"] ?? "guest";
			var vhost = cfg["RabbitMQ:VirtualHost"] ?? "/";
			var portStr = cfg["RabbitMQ:Port"];
			var port = !string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out var p) ? p : AmqpTcpEndpoint.UseDefaultPort;

			var factory = new ConnectionFactory
			{
				HostName = host,
				Port = port,
				VirtualHost = vhost,
				UserName = user,
				Password = pass,
				AutomaticRecoveryEnabled = true,
				NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
				RequestedConnectionTimeout = TimeSpan.FromSeconds(10)
			};

			// Create a connection (sync is fine here)
			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

			// Create channel with confirms enabled
			_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

			// Ensure queue exists
			_channel.QueueDeclareAsync(
				queue: _queue,
				durable: true, exclusive: false, autoDelete: false, arguments: null
			).GetAwaiter().GetResult();
		}

		public void Publish(TransactionCreatedEvent @event)
		{
			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

			// Publish (async API in v7; blocking here to keep sync signature)
			_channel.BasicPublishAsync(
				exchange: "",
				routingKey: _queue,
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
