using System.Reflection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Bank.Infrastructure;
using Bank.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Bank.Infrastructure.Extensions;
using Bank_System.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

// === Serilog setup ===
var configuration = builder.Configuration;
var env = builder.Environment.EnvironmentName;
var esUri = configuration["ElasticConfigration:Uri"] ?? "http://localhost:9200";

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(configuration)
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Environment", env)
	.Enrich.WithProperty("Application", Assembly.GetEntryAssembly()?.GetName().Name ?? "Bank.Api")
	.Enrich.WithExceptionDetails()
	.WriteTo.Console()
	.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esUri))
	{
		AutoRegisterTemplate = true,
		AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
		IndexFormat = $"bank-logs-{{0:yyyy.MM.dd}}-{env?.ToLower()}",
		BatchPostingLimit = 50,
		Period = TimeSpan.FromSeconds(2),
		FailureCallback = (logEvent, exception) =>
		{
			Console.Error.WriteLine(
				$"[Serilog-ES] Failed to submit event: {exception?.Message} | {logEvent?.MessageTemplate}");
		},
		EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
						   EmitEventFailureHandling.WriteToFailureSink |
						   EmitEventFailureHandling.RaiseCallback
	})
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		ValidAudience = builder.Configuration["Jwt:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});

builder.Services.AddAuthorization();


builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
	{
		Title = "Takafull Mobile API",
		Version = "v1"
	});

	// Add JWT Bearer Auth to Swagger
	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "Please enter a valid JWT (Format: Bearer <token>)",
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});

	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

// === Services ===
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<RateLimiterMiddleware>();

// === Middleware ===
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	await services.SeedRolesAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseSerilogRequestLogging();

try
{
	Log.Information("Starting up {App} in {Env}", "Bank.Api", env);
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed");
	throw;
}
finally
{
	Log.CloseAndFlush();
}
