# Multi-stage Dockerfile for Backend.API
# Optimized for production deployment

# Base stage for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files first for better cache
COPY Backend.API/Backend.API.csproj Backend.API/
# COPY Directory.Build.props ./
RUN dotnet restore Backend.API/Backend.API.csproj

# Copy the rest of the source
COPY . .

# Build
WORKDIR /src/Backend.API
RUN dotnet build Backend.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Backend.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final production stage
FROM base AS final
WORKDIR /app

# Install curl for healthcheck (Debian-based aspnet image)
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Copy published output
COPY --from=publish /app/publish .

# Environment
ENV ASPNETCORE_ENVIRONMENT=Production
# Prefer a single way to bind; nginx terminates TLS:
ENV ASPNETCORE_URLS=http://+:8080
# Optional hardening (disable diagnostics pipe in prod)
ENV DOTNET_EnableDiagnostics=0

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Backend.API.dll"]
