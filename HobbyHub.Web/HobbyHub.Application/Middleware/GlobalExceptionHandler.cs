using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace HobbyHub.Application.Middleware;

public class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case ValidationException fluentException:
            {
                problemDetails.Title = "Validation Failure.";
                problemDetails.Detail = "One or more validation errors occurred.";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                var validationErrors = fluentException.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList();
                problemDetails.Extensions.Add("errors", validationErrors);
                break;
            }
            case UnauthorizedAccessException:
                problemDetails.Title = "Unauthorized.";
                problemDetails.Detail = "Access denied. You must authenticate before accessing this resource.";
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
            case SecurityTokenExpiredException:
                problemDetails.Title = "Unauthorized.";
                problemDetails.Detail = "Security token has expired, authenticate again.";
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                break;
            case ArgumentException:
                problemDetails.Title = "Bad Request.";
                problemDetails.Detail = "The provided value does not meet the expected criteria.";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            case FormatException:
                problemDetails.Title = "Bad Request.";
                problemDetails.Detail = "The provided value does not match the expected format.";
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                break;
            default:
                problemDetails.Title = "Internal server error.";
                problemDetails.Detail = "Internal server error occured. Please try again later";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        logger.Error("{ProblemDetailsTitle}", problemDetails.Title);

        problemDetails.Status = httpContext.Response.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}