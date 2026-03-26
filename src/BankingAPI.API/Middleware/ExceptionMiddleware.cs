using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using BankingAPI.Application.Common;
using BankingAPI.Domain.Exceptions;
using FluentValidation;

namespace BankingAPI.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

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
        var (statusCode, code, message) = ex switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest, "01",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            NotFoundException nfe  => (HttpStatusCode.NotFound, "02", nfe.Message),
            DuplicateEmailException dee => (HttpStatusCode.Conflict, "03", dee.Message),
            InsufficientFundsException ife => (HttpStatusCode.UnprocessableEntity, "04", ife.Message),
            DomainException de     => (HttpStatusCode.BadRequest, "05", de.Message),
            _                      => (HttpStatusCode.InternalServerError, "99", "An unexpected error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var body = JsonSerializer.Serialize(
            ApiResponse<object>.Failure(code, message),
            _jsonOptions);

        return context.Response.WriteAsync(body);
    }
}
