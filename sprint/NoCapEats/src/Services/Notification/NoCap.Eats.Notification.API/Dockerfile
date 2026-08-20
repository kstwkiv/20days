# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/NoCap.Eats.BuildingBlocks.Events.csproj \
     src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/
COPY src/Services/Notification/NoCap.Eats.Notification.Domain/NoCap.Eats.Notification.Domain.csproj \
     src/Services/Notification/NoCap.Eats.Notification.Domain/
COPY src/Services/Notification/NoCap.Eats.Notification.Application/NoCap.Eats.Notification.Application.csproj \
     src/Services/Notification/NoCap.Eats.Notification.Application/
COPY src/Services/Notification/NoCap.Eats.Notification.Infrastructure/NoCap.Eats.Notification.Infrastructure.csproj \
     src/Services/Notification/NoCap.Eats.Notification.Infrastructure/
COPY src/Services/Notification/NoCap.Eats.Notification.API/NoCap.Eats.Notification.API.csproj \
     src/Services/Notification/NoCap.Eats.Notification.API/

RUN dotnet restore src/Services/Notification/NoCap.Eats.Notification.API/NoCap.Eats.Notification.API.csproj

COPY . .
RUN dotnet publish src/Services/Notification/NoCap.Eats.Notification.API/NoCap.Eats.Notification.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Notification.API.dll"]
