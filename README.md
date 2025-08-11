# Bank System (Event-driven Banking API)

A .NET 9 solution demonstrating a layered, event-driven banking system with JWT-authenticated API, CQRS via MediatR, EF Core with SQL Server, Redis caching, RabbitMQ messaging, and centralized logging to Elasticsearch/Kibana. A separate Transactions Log Service consumes transaction events and persists an audit log.

## Architecture
- **Bank System** (`Bank System`): ASP.NET Core Web API (net9.0)
  - Auth (register/login/refresh) using ASP.NET Identity
  - Bank Accounts (create, get, deposit, withdraw, transfer, update account config)
  - Swagger UI enabled in development
  - Serilog to console and Elasticsearch
  - Redis caching (StackExchange.Redis)
  - Publishes `TransactionCreatedEvent` to RabbitMQ on money movements
- **Application Layer** (`Bank.Application`):
  - CQRS with MediatR (commands/queries/handlers)
  - Background job wiring for interest accrual (`ApplyInterestJob`)—Hangfire integration present but disabled
- **Domain Layer** (`Bank.Domain`):
  - Entities: `Account`, `BankAccount` (+ `CheckingAccount`, `SavingsAccount`), `Transaction`, `RefreshToken`, `AccountTypeConfig`
  - Repository interfaces
- **Infrastructure Layer** (`Bank.Infrastructure`):
  - EF Core `ApplicationDbContext`, migrations, repository implementations
  - ASP.NET Identity store
  - RabbitMQ publisher implementation
  - DI registrations
- **Shared** (`Bank.Shared`): Cross-service contracts/events (`TransactionCreatedEvent`)
- **Transactions Log Service** (`Bank.TransactionsLogService`):
  - Minimal ASP.NET Core service (net9.0) with EF Core context for `TransactionLog`
  - Background consumer reads from RabbitMQ queue `transaction-log-queue` and persists logs
  - Basic controller to list logs

Docker Compose orchestrates:
- API (`api`)
- Transactions Log Service (`transaction-log-service`)
- SQL Server 2022 (`sqlserver`)
- RabbitMQ + management UI (`rabbitmq`)
- Redis (`redis`)
- Elasticsearch (`elasticsearch`) and Kibana (`kibana`)

## Tech Stack
- .NET 9, ASP.NET Core Web API, MediatR, EF Core (SQL Server), ASP.NET Identity
- RabbitMQ.Client 7.x, StackExchange.Redis, Serilog + Elasticsearch sink
- Docker, Docker Compose

## Getting Started (Docker)
Prerequisites: Docker Desktop/Engine and Docker Compose.

1) Build and run all services

```bash
docker compose up -d --build
```

2) Service endpoints
- API Swagger: http://localhost:5000/swagger
- Transactions Log Service (OpenAPI doc): http://localhost:5002/openapi/v1.json
- RabbitMQ UI: http://localhost:15672 (username: `bankuser`, password: `bankpass`)
- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601

3) Default configuration (from `docker-compose.yml` / `appsettings.Docker.json`)
- API connection string: `Server=sqlserver;Database=BankSystem;User=sa;Password=<...>;TrustServerCertificate=true;`
- Log service connection string: `Server=sqlserver;Database=TransactionLogDb;User=sa;Password=<...>;TrustServerCertificate=true;`
- JWT: issuer `Bank-api`, audience `Bank-client`, symmetric key (replace in production)
- RabbitMQ: host `rabbitmq`, user `bankuser`, password `bankpass`, vhost `/`
- Elasticsearch URI: `http://elasticsearch:9200`

Note: On first boot, both services run EF Core migrations automatically.

## Running Locally (without Docker)
- Install .NET SDK 9.0
- Ensure SQL Server, RabbitMQ, Redis, and Elasticsearch are running and connection strings match your environment
- Set ASPNETCORE_ENVIRONMENT=Development and update `appsettings.Development.json`
- From `Bank System` and `Bank.TransactionsLogService` projects:

```bash
dotnet restore
dotnet ef database update # if you prefer manual migrations
dotnet run
```

## API Overview
- Auth (`/api/Account`)
  - POST `/register` — create user (default role User)
  - POST `/login` — JWT issuance
  - POST `/refresh` — refresh token
- Bank Accounts (`/api/BankAccounts`)
  - POST `/` — create bank account
  - GET `/{id}` — fetch account by id
  - POST `/deposit` — deposit funds
  - POST `/withdraw` — withdraw funds
  - POST `/transfer` — transfer between accounts
  - PATCH `/UpdateAccountTypeConfig` — update account type config (e.g., interest/overdraft)
- Transaction Logs Service (`/api/TransactionLogs`)
  - GET `/GetAllTransactionLogs` — list logged transactions

Transaction events: On deposit/withdraw/transfer, API publishes `TransactionCreatedEvent` to RabbitMQ. The log service consumes from `transaction-log-queue` and persists records.

## Observability
- Serilog sends structured logs to console and Elasticsearch (`ElasticConfigration:Uri`).
- View logs in Kibana (configured in compose file with security disabled for local dev).

## Configuration Notes
- Environment-specific configuration for Docker is in `Bank System/appsettings.Docker.json` and `Bank.TransactionsLogService/appsettings.Docker.json`.
- Compose sets environment variables for connection strings, JWT, RabbitMQ, and Elasticsearch.

## Development Notes and Recommendations
- Redis host in API is hardcoded as `127.0.0.1:6379` in `Program.cs`. When running in Docker, this should be `redis:6379` or read from configuration. Consider changing to:
  - `builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(configuration["Redis:Connection"] ?? "redis:6379"));`
  - `builder.Services.AddStackExchangeRedisCache(o => o.Configuration = configuration["Redis:Connection"] ?? "redis:6379");`
  - And add `"Redis": { "Connection": "redis:6379" }` to `appsettings.Docker.json`.
- Secrets (SA password, JWT key) are committed in compose/appsettings. For production, move to environment variables, Docker secrets, or a vault. Rotate keys regularly.
- There appear to be two projects for the log service: `Bank.TransactionLogService` and `Bank.TransactionsLogService`. The solution and compose reference the plural form. Remove the unused one to avoid confusion.
- Hangfire wiring is present but commented. If you need scheduled jobs (e.g., `ApplyInterestJob`), enable Hangfire; add connection string, call `UseHangfireDashboard`, and configure recurring jobs.
- Add unit/integration tests (application handlers, repositories, controller contract tests).
- Consider enabling HTTPS and configuring proper CORS.

## Migrations
Migrations run on startup. To add migrations during development:

```bash
# API DbContext
cd Bank.Infrastructure
dotnet ef migrations add <Name> -s ../"Bank System"/Bank\ System.csproj -c ApplicationDbContext

# Transactions Log Service DbContext
cd ../Bank.TransactionsLogService
dotnet ef migrations add <Name> -c TransactionLogDbContext
```

## License
Not specified.
