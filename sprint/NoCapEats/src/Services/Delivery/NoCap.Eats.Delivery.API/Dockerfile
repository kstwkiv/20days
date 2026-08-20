# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/NoCap.Eats.BuildingBlocks.Events.csproj \
     src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/
COPY src/Services/Delivery/NoCap.Eats.Delivery.Domain/NoCap.Eats.Delivery.Domain.csproj \
     src/Services/Delivery/NoCap.Eats.Delivery.Domain/
COPY src/Services/Delivery/NoCap.Eats.Delivery.Application/NoCap.Eats.Delivery.Application.csproj \
     src/Services/Delivery/NoCap.Eats.Delivery.Application/
COPY src/Services/Delivery/NoCap.Eats.Delivery.Infrastructure/NoCap.Eats.Delivery.Infrastructure.csproj \
     src/Services/Delivery/NoCap.Eats.Delivery.Infrastructure/
COPY src/Services/Delivery/NoCap.Eats.Delivery.API/NoCap.Eats.Delivery.API.csproj \
     src/Services/Delivery/NoCap.Eats.Delivery.API/

RUN dotnet restore src/Services/Delivery/NoCap.Eats.Delivery.API/NoCap.Eats.Delivery.API.csproj

COPY . .
RUN dotnet publish src/Services/Delivery/NoCap.Eats.Delivery.API/NoCap.Eats.Delivery.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Delivery.API.dll"]
