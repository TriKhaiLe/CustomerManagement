using System.Net;
using System.Text.Json;
using CustomerManagement.Shared.Common;
using Server.Application.Common.Exceptions;

namespace Server.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, errors) = exception switch
        {
            AppValidationException validationEx =>
                (HttpStatusCode.BadRequest, "ValidationError", (IDictionary<string, string[]>?)validationEx.Errors),
            NotFoundException => (HttpStatusCode.NotFound, "NotFound", null),
            ConflictException => (HttpStatusCode.Conflict, "Conflict", null),
            UnauthorizedAppException => (HttpStatusCode.Unauthorized, "Unauthorized", null),
            _ => (HttpStatusCode.InternalServerError, "ServerError", null)
        };

        if (status == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "{Title} while processing {Method} {Path}", title, context.Request.Method, context.Request.Path);
        }

        var response = new ApiErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Status = (int)status,
            Title = title,
            Detail = status == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred. Please try again later."
                : exception.Message,
            Errors = errors is null ? null : new Dictionary<string, string[]>(errors),
            TraceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
