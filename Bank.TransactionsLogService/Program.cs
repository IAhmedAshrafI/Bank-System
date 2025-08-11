using Bank.TransactionsLogService.Entities;
using Bank.TransactionsLogService.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddDbContext<TransactionLogDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<TransactionLogConsumerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	var dbContext = services.GetRequiredService<TransactionLogDbContext>();
	await dbContext.Database.MigrateAsync();

}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
