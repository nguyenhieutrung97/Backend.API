using FluentValidation;
using MediatR;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.Models;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.Repositories;
using Backend.API.Infrastructure.Data;
using Backend.API.Infrastructure.Validation;
using Backend.API.Infrastructure.Errors;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// CORS configuration
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsProduction())
    {
        // Production: Only allow trungtero.com
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("https://trungtero.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    }
    else
    {
        // Development: Allow localhost and trungtero.com
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("https://trungtero.com", "http://localhost:3000", "http://localhost:3001")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    }
});

builder.Services.AddAutoMapper(typeof(UserProfile), typeof(SeafileProfile));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<GetUsersHandler>());

// Register all validators in assembly
builder.Services.AddValidatorsFromAssemblyContaining<UserFilterValidator>();
builder.Services.AddSingleton<IUserStore, FakeUserSeeder>();

// MongoDB configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "BackendAPI";

builder.Services.AddSingleton<IMongoClient>(provider => new MongoClient(mongoConnectionString));
builder.Services.AddScoped<IMongoDatabase>(provider => 
{
    var client = provider.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDatabaseName);
});

// Seafile services
builder.Services.AddScoped<ISeafileTrackRepository, MongoSeafileTrackRepository>();

builder.Services.AddOpenTelemetry()
  .ConfigureResource(r => r.AddService(
      serviceName: "UserApi",
      serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString())
      .AddAttributes(new KeyValuePair<string, object>[]
      {
          new("deployment.environment", builder.Environment.EnvironmentName)
      }))
  .WithTracing(t => t
      .AddAspNetCoreInstrumentation(o => o.RecordException = true)
      .AddHttpClientInstrumentation()
      .AddOtlpExporter(o =>
      {
          var endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://otel-collector:4317";
          o.Endpoint = new Uri(endpoint);
      }))
  .WithMetrics(m => m
      .AddAspNetCoreInstrumentation()
      .AddRuntimeInstrumentation()
      .AddOtlpExporter(o =>
      {
          var endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://otel-collector:4317";
          o.Endpoint = new Uri(endpoint);
      }));

// MediatR pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


var app = builder.Build();

app.UseSerilogRequestLogging();

// Global exception / problem details handling
app.UseGlobalExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.UseHttpsRedirection();

// Security headers for production
if (app.Environment.IsProduction())
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Permissions-Policy", "geolocation=(), microphone=(), camera=()");
        
        // Strict CSP for production
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; connect-src 'self' https://trungtero.com https://api.trungtero.com;");
        
        await next();
    });
}

// Enable CORS
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

// Root endpoint convenience
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
