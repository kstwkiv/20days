# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first for layer-cached restore
COPY src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/NoCap.Eats.BuildingBlocks.Events.csproj \
     src/BuildingBlocks/NoCap.Eats.BuildingBlocks.Events/
COPY src/Services/Identity/NoCap.Eats.Identity.Domain/NoCap.Eats.Identity.Domain.csproj \
     src/Services/Identity/NoCap.Eats.Identity.Domain/
COPY src/Services/Identity/NoCap.Eats.Identity.Application/NoCap.Eats.Identity.Application.csproj \
     src/Services/Identity/NoCap.Eats.Identity.Application/
COPY src/Services/Identity/NoCap.Eats.Identity.Infrastructure/NoCap.Eats.Identity.Infrastructure.csproj \
     src/Services/Identity/NoCap.Eats.Identity.Infrastructure/
COPY src/Services/Identity/NoCap.Eats.Identity.API/NoCap.Eats.Identity.API.csproj \
     src/Services/Identity/NoCap.Eats.Identity.API/

RUN dotnet restore src/Services/Identity/NoCap.Eats.Identity.API/NoCap.Eats.Identity.API.csproj

# Copy everything and publish
COPY . .
RUN dotnet publish src/Services/Identity/NoCap.Eats.Identity.API/NoCap.Eats.Identity.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Identity.API.dll"]
