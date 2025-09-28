# Multi-stage Dockerfile for Backend.API
# Optimized for production deployment

# Base stage for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file first for better layer caching
COPY ["Backend.API/Backend.API.csproj", "Backend.API/"]
RUN dotnet restore "./Backend.API/Backend.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Backend.API"
RUN dotnet build "./Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Backend.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final production stage
FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Health check for container orchestration
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "Backend.API.dll"]