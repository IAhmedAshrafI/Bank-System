using Hangfire;
using LoanApprovalService.Entities;
using LoanApprovalService.Interfaces;
using LoanApprovalService.Jobs;
using LoanApprovalService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.UseUrls("http://0.0.0.0:80");

// Add DbContext
builder.Services.AddDbContext<LoanApprovalDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<LoanRequestConsumerService>();
builder.Services.AddScoped<IRabbitMQPublisher, LoanApprovalService.Services.RabbitMQPublisher>();

builder.Services.AddHostedService<LoanRequestConsumerService>();

//builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
//builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{

	var services = scope.ServiceProvider;

	var dbContext = services.GetRequiredService<LoanApprovalDbContext>();
	await dbContext.Database.MigrateAsync();

	
}

app.Run();
