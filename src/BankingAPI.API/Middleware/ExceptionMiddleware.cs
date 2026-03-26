using System.Net;
using System.Text.Json;
using BankingAPI.Domain.Exceptions;
using FluentValidation;

namespace BankingAPI.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, message) = ex switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest,
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            NotFoundException nfe => (HttpStatusCode.NotFound, nfe.Message),
            DuplicateEmailException dee => (HttpStatusCode.Conflict, dee.Message),
            InsufficientFundsException ife => (HttpStatusCode.UnprocessableEntity, ife.Message),
            DomainException de => (HttpStatusCode.BadRequest, de.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(body);
    }
}
