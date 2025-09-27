using FluentValidation;
using MediatR;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.Models;
using Backend.API.Infrastructure.Data;
using Backend.API.Infrastructure.Validation;
using Backend.API.Infrastructure.Errors;


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

builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<GetUsersHandler>());

// Register all validators in assembly
builder.Services.AddValidatorsFromAssemblyContaining<UserFilterValidator>();
builder.Services.AddSingleton<IUserStore, FakeUserSeeder>();

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

app.UseAuthorization();

app.MapControllers();

// Root endpoint convenience
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
