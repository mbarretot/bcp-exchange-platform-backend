using System.Net;
using System.Text.Json;
using Bcp.Exchange.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Bcp.Exchange.FunctionApp.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    : IFunctionsWorkerMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private sealed record ProblemDetails
    {
        public required string Title { get; init; }
        public required int Status { get; init; }
        public string? Detail { get; init; }
        public string? Instance { get; init; }
        public string? TraceId { get; init; }
        public Dictionary<string, object>? Errors { get; init; }
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ResultFailureException resultException)
        {
            logger.LogWarning(
                "Business logic error in {FunctionName}: {ErrorCode}",
                context.FunctionDefinition.Name,
                resultException.Error.Code
            );

            await HandleResultFailureAsync(context, resultException);
        }
        catch (ValidationException validationException)
        {
            logger.LogWarning(
                validationException,
                "Validation failed for {FunctionName}",
                context.FunctionDefinition.Name
            );

            await HandleValidationExceptionAsync(context, validationException);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Unhandled exception in {FunctionName}",
                context.FunctionDefinition.Name
            );

            await HandleUnexpectedExceptionAsync(context, exception);
        }
    }

    private static async Task HandleResultFailureAsync(
        FunctionContext context,
        ResultFailureException resultException
    )
    {
        var request = await context.GetHttpRequestDataAsync();
        if (request is null)
            return;

        var (statusCode, title) = MapErrorCodeToStatus(resultException.Error.Code);

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Detail = resultException.Error.Message,
            Instance = request.Url.AbsolutePath,
            TraceId = context.TraceContext.TraceParent,
        };

        await WriteResponseAsync(request, problemDetails, context);
    }

    private static (int StatusCode, string Title) MapErrorCodeToStatus(string errorCode) =>
        errorCode switch
        {
            var code when code.Contains("NotFound") => (
                StatusCodes.Status404NotFound,
                "Resource not found"
            ),
            var code when code.Contains("Unauthorized") => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized"
            ),
            var code when code.Contains("Forbidden") => (
                StatusCodes.Status403Forbidden,
                "Forbidden"
            ),
            var code when code.Contains("Conflict") || code.Contains("Duplicate") => (
                StatusCodes.Status409Conflict,
                "Conflict"
            ),
            _ => (StatusCodes.Status400BadRequest, "Bad request"),
        };

    private static async Task HandleValidationExceptionAsync(
        FunctionContext context,
        ValidationException validationException
    )
    {
        var request = await context.GetHttpRequestDataAsync();
        if (request is null)
            return;

        var errors = validationException
            .Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray() as object);

        var problemDetails = new ProblemDetails
        {
            Title = "Validation failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = "Se produjeron uno o más errores de validación",
            Instance = request.Url.AbsolutePath,
            TraceId = context.TraceContext.TraceParent,
            Errors = errors,
        };

        await WriteResponseAsync(request, problemDetails, context);
    }

    private static async Task HandleUnexpectedExceptionAsync(
        FunctionContext context,
        Exception exception
    )
    {
        var request = await context.GetHttpRequestDataAsync();
        if (request is null)
            return;

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Ha ocurrido un error inesperado en el servidor",
            Instance = request.Url.AbsolutePath,
            TraceId = context.TraceContext.TraceParent,
        };

        await WriteResponseAsync(request, problemDetails, context);
    }

    private static async Task WriteResponseAsync(
        HttpRequestData request,
        ProblemDetails problemDetails,
        FunctionContext context
    )
    {
        var response = request.CreateResponse((HttpStatusCode)problemDetails.Status);
        response.Headers.Add("Content-Type", "application/problem+json");

        await response.WriteStringAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));

        context.GetInvocationResult().Value = response;
    }
}
