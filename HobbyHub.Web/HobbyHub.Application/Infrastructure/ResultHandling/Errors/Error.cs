using System.Net;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public sealed record Error(string Title, string Detail, int StatusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, StatusCodes.Status200OK);
}