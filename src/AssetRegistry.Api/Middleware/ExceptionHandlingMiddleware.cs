using AssetRegistry.Application.Exceptions;
using AssetRegistry.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AssetRegistry.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await WriteProblemDetailsAsync(context, exception);
        }
    }

    private async Task WriteProblemDetailsAsync(HttpContext context, Exception exception)
    {
        var problemDetails = CreateProblemDetails(exception);

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Path}.", context.Request.Path);
        }

        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static ProblemDetails CreateProblemDetails(Exception exception)
    {
        return exception switch
        {
            NotFoundException => Build(StatusCodes.Status404NotFound, "Resource not found", exception.Message),
            ConflictException => Build(StatusCodes.Status409Conflict, "Conflict", exception.Message),
            DomainValidationException => Build(StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
            _ => Build(StatusCodes.Status500InternalServerError, "Unexpected error", "Something went wrong. Try again.")
        };
    }

    private static ProblemDetails Build(int status, string title, string detail)
    {
        return new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };
    }
}
