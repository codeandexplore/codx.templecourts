using System.Net;
using System.Text.Json;
using Codx.Temple.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Codx.Temple.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedException ex => (HttpStatusCode.Unauthorized, ex.Message),
            ForbiddenException ex => (HttpStatusCode.Forbidden, ex.Message),
            ConflictException ex => (HttpStatusCode.Conflict, ex.Message),
            ValidationException ex => (HttpStatusCode.UnprocessableEntity, JsonSerializer.Serialize(ex.Errors)),
            GatingBlockedException ex => (HttpStatusCode.UnprocessableEntity, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = new { error = message };
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
