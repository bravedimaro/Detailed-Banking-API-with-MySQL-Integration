# ── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution + project files first for layer-cached restore
COPY BankingAPI.sln .
COPY src/BankingAPI.Domain/BankingAPI.Domain.csproj             src/BankingAPI.Domain/
COPY src/BankingAPI.Application/BankingAPI.Application.csproj   src/BankingAPI.Application/
COPY src/BankingAPI.Infrastructure/BankingAPI.Infrastructure.csproj src/BankingAPI.Infrastructure/
COPY src/BankingAPI.API/BankingAPI.API.csproj                   src/BankingAPI.API/
COPY tests/BankingAPI.Application.Tests/BankingAPI.Application.Tests.csproj tests/BankingAPI.Application.Tests/

RUN dotnet restore

# Copy everything else and publish
COPY . .
RUN dotnet publish src/BankingAPI.API/BankingAPI.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

# Ownership
RUN chown -R appuser:appgroup /app
USER appuser

# ASP.NET Core listens on 8080 by default in .NET 8+
EXPOSE 8080

# Environment variable defaults (overridable at runtime)
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__DefaultConnection="" \
    Jwt__Key="" \
    Jwt__Issuer="BankingAPI" \
    Jwt__Audience="BankingAPIUsers"

ENTRYPOINT ["dotnet", "BankingAPI.API.dll"]
