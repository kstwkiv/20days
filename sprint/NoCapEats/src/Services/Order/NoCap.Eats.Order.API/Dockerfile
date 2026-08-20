# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/NoCap.Eats.BuildingBlocks.Events.csproj \
     src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/
COPY src/Services/Order/NoCap.Eats.Order.Domain/NoCap.Eats.Order.Domain.csproj \
     src/Services/Order/NoCap.Eats.Order.Domain/
COPY src/Services/Order/NoCap.Eats.Order.Application/NoCap.Eats.Order.Application.csproj \
     src/Services/Order/NoCap.Eats.Order.Application/
COPY src/Services/Order/NoCap.Eats.Order.Infrastructure/NoCap.Eats.Order.Infrastructure.csproj \
     src/Services/Order/NoCap.Eats.Order.Infrastructure/
COPY src/Services/Order/NoCap.Eats.Order.API/NoCap.Eats.Order.API.csproj \
     src/Services/Order/NoCap.Eats.Order.API/

RUN dotnet restore src/Services/Order/NoCap.Eats.Order.API/NoCap.Eats.Order.API.csproj

COPY . .
RUN dotnet publish src/Services/Order/NoCap.Eats.Order.API/NoCap.Eats.Order.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Order.API.dll"]
