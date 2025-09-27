using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Infrastructure.Errors;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                if (feature == null) return;

                var ex = feature.Error;
                ProblemDetails problem;
                int status;

                switch (ex)
                {
                    case ValidationException vex:
                        status = StatusCodes.Status400BadRequest;
                        problem = new ValidationProblemDetails(vex.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
                        {
                            Title = "Validation failed",
                            Status = status,
                            Instance = context.Request.Path
                        };
                        break;
                    default:
                        status = StatusCodes.Status500InternalServerError;
                        problem = new ProblemDetails
                        {
                            Title = "An unexpected error occurred",
                            Detail = ex.Message,
                            Status = status,
                            Instance = context.Request.Path
                        };
                        break;
                }

                problem.Extensions["traceId"] = context.TraceIdentifier;
                context.Response.StatusCode = status;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(problem);
            });
        });

        return app;
    }
}
