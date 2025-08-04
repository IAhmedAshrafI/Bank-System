using System.Collections.Concurrent;

namespace Bank_System.MiddleWare
{
	public class RateLimiterMiddleware
	{
		private readonly RequestDelegate _next;
		private static readonly ConcurrentDictionary<string, ClientRateLimit> _rateLimits = new();

		public RateLimiterMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			var ipAddress = context.Connection.RemoteIpAddress?.ToString();
			if (string.IsNullOrEmpty(ipAddress))
			{
				await _next(context);
				return;
			}

			if (!_rateLimits.TryGetValue(ipAddress, out var clientInfo))
			{
				clientInfo = new ClientRateLimit();
				_rateLimits[ipAddress] = clientInfo;
			}

			if (!clientInfo.IsAllowed())
			{
				context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
				await context.Response.WriteAsync("Too many requests. Try again later.");
				return;
			}

			await _next(context);
		}
	}

	public class ClientRateLimit
	{
		private int _count;
		private DateTime _resetTime;

		public ClientRateLimit()
		{
			ResetWindow();
		}

		public bool IsAllowed()
		{
			if (DateTime.UtcNow > _resetTime)
			{
				_count = 0;
				ResetWindow();
			}

			if (_count >= 30)
			{
				return false;
			}

			_count++;
			return true;
		}

		private void ResetWindow()
		{
			_resetTime = DateTime.UtcNow.AddSeconds(30);
		}
	}
}

