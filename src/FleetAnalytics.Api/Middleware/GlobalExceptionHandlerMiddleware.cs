using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FleetAnalytics.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred while processing your request.",
            Detail = exception.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = context.Request.Path
        };

        if (exception is KeyNotFoundException)
        {
            problemDetails.Title = "Resource not found.";
            problemDetails.Status = (int)HttpStatusCode.NotFound;
        }
        else if (exception is ArgumentException || exception is InvalidOperationException)
        {
            problemDetails.Title = "Invalid request.";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
        }

        context.Response.StatusCode = problemDetails.Status.Value;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
