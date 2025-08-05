using System.Text;

namespace Bank_System.MiddleWare
{
	public class HttpHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<HttpHandlerMiddleware> _logger;

		public HttpHandlerMiddleware(RequestDelegate next,
		ILogger<HttpHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			LogRequest(context);

			var originalBodyStream = context.Response.Body;
			using (var responseBody = new MemoryStream())
			{
				context.Response.Body = responseBody;

				await _next(context);

				LogResponse(context);

				await responseBody.CopyToAsync(originalBodyStream);
			}
		}

		private void LogRequest(HttpContext context)
		{
			var request = context.Request;

			var requestLog = new StringBuilder();
			requestLog.AppendLine("Incoming Request:");
			requestLog.AppendLine($"HTTP {request.Method} {request.Path}");
			requestLog.AppendLine($"Host: {request.Host}");
			requestLog.AppendLine($"Content-Type: {request.ContentType}");
			requestLog.AppendLine($"Content-Length: {request.ContentLength}");

			_logger.LogInformation(requestLog.ToString());
		}

		private void LogResponse(HttpContext context)
		{
			var response = context.Response;

			var responseLog = new StringBuilder();
			responseLog.AppendLine("Outgoing Response:");
			responseLog.AppendLine($"HTTP {response.StatusCode}");
			responseLog.AppendLine($"Content-Type: {response.ContentType}");
			responseLog.AppendLine($"Content-Length: {response.ContentLength}");

			_logger.LogInformation(responseLog.ToString());
		}
	}
}
