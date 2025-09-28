# Multi-stage Dockerfile for Backend.API

# Base stage for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Backend.API/Backend.API.csproj Backend.API/
RUN dotnet restore Backend.API/Backend.API.csproj
COPY . .
WORKDIR /src/Backend.API
RUN dotnet build Backend.API.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Backend.API.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Switch to root just to install curl, then drop back to non-root
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/*
USER app

# Copy published output
COPY --from=publish /app/publish .

# Environment (single binding; nginx terminates TLS)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=0
# Optional: honor X-Forwarded-* from nginx-proxy
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# Healthcheck (longer start period)
HEALTHCHECK --interval=30s --timeout=5s --start-period=25s --retries=3 \
  CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "Backend.API.dll"]
