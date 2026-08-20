# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/NoCap.Eats.BuildingBlocks.Events.csproj \
     src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/
COPY src/Services/Catalog/NoCap.Eats.Catalog.Domain/NoCap.Eats.Catalog.Domain.csproj \
     src/Services/Catalog/NoCap.Eats.Catalog.Domain/
COPY src/Services/Catalog/NoCap.Eats.Catalog.Application/NoCap.Eats.Catalog.Application.csproj \
     src/Services/Catalog/NoCap.Eats.Catalog.Application/
COPY src/Services/Catalog/NoCap.Eats.Catalog.Infrastructure/NoCap.Eats.Catalog.Infrastructure.csproj \
     src/Services/Catalog/NoCap.Eats.Catalog.Infrastructure/
COPY src/Services/Catalog/NoCap.Eats.Catalog.API/NoCap.Eats.Catalog.API.csproj \
     src/Services/Catalog/NoCap.Eats.Catalog.API/

RUN dotnet restore src/Services/Catalog/NoCap.Eats.Catalog.API/NoCap.Eats.Catalog.API.csproj

COPY . .
RUN dotnet publish src/Services/Catalog/NoCap.Eats.Catalog.API/NoCap.Eats.Catalog.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Catalog.API.dll"]
