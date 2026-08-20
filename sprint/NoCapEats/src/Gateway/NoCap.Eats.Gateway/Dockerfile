# syntax=docker/dockerfile:1
# ── Build stage ───────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Gateway/NoCap.Eats.Gateway/NoCap.Eats.Gateway.csproj \
     src/Gateway/NoCap.Eats.Gateway/

RUN dotnet restore src/Gateway/NoCap.Eats.Gateway/NoCap.Eats.Gateway.csproj

COPY . .
RUN dotnet publish src/Gateway/NoCap.Eats.Gateway/NoCap.Eats.Gateway.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NoCap.Eats.Gateway.dll"]
