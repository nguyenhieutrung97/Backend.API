using FluentValidation;
using MediatR;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.Models;
using Backend.API.Infrastructure.Data;


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

builder.Services.AddScoped<IValidator<UserFilter>, UserFilterValidator>();
builder.Services.AddSingleton<IUserStore, FakeUserSeeder>();

builder.Services.AddOpenTelemetry()
  .ConfigureResource(r => r.AddService("UserApi"))
  .WithTracing(t => t
      .AddAspNetCoreInstrumentation()
      .AddHttpClientInstrumentation()
      .AddOtlpExporter(o => { o.Endpoint = new Uri("http://otel-collector:4317"); }))
  .WithMetrics(m => m
      .AddAspNetCoreInstrumentation()
      .AddRuntimeInstrumentation()
      .AddOtlpExporter(o => { o.Endpoint = new Uri("http://otel-collector:4317"); }));


var app = builder.Build();

app.UseSerilogRequestLogging();

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

app.Run();
