# Banking REST API

A secure banking REST API built with **ASP.NET Core 9** following **Clean Architecture** principles.

## Architecture

```
src/
├── BankingAPI.Domain/          # Entities, exceptions — no dependencies
├── BankingAPI.Application/     # MediatR handlers, interfaces, DTOs, validators
├── BankingAPI.Infrastructure/  # EF Core (MySQL), repositories, JWT service
└── BankingAPI.API/             # Controllers, middleware, Swagger, Program.cs
tests/
└── BankingAPI.Application.Tests/  # xUnit unit tests (Moq + FluentAssertions)
```

## Tech Stack

- ASP.NET Core 9 Web API
- MediatR (CQRS pattern)
- Entity Framework Core 9 + Pomelo MySQL provider
- FluentValidation
- JWT Bearer authentication
- Swashbuckle (Swagger UI)
- BCrypt.Net for password hashing
- xUnit + Moq + FluentAssertions for testing

## Prerequisites

- .NET 9 SDK
- MySQL 8.x running locally (or Docker)

## Setup

### 1. Database

```sql
-- Run database/setup.sql to create the database
CREATE DATABASE BankingDB CHARACTER SET utf8mb4;
```

### 2. Configuration

Update `src/BankingAPI.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=BankingDB;User=root;Password=yourpassword;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "BankingAPI",
    "Audience": "BankingAPIUsers"
  }
}
```

### 3. Run EF Core Migrations

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/BankingAPI.Infrastructure --startup-project src/BankingAPI.API
dotnet ef database update --project src/BankingAPI.Infrastructure --startup-project src/BankingAPI.API
```

> The API also auto-migrates on startup via `db.Database.Migrate()`.

### 4. Run the API

```bash
dotnet run --project src/BankingAPI.API
```

Swagger UI: `http://localhost:5000/swagger`

## API Endpoints

### Auth
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and get JWT token |

### Users (requires JWT)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/me` | Get current user profile |
| PUT | `/api/users/me` | Update profile (name, phone, address) |

### Accounts (requires JWT)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/accounts/me` | Get account details and balance |

### Transactions (requires JWT)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/transactions/transfer` | Transfer funds to another account |
| GET | `/api/transactions/history` | Get transaction history |

## Running Tests

```bash
dotnet test
```

## Security

- Passwords hashed with BCrypt (work factor 11)
- JWT tokens expire after 24 hours
- All account/transaction endpoints require a valid JWT
- FluentValidation on all inputs
- Global exception middleware returns structured error responses
