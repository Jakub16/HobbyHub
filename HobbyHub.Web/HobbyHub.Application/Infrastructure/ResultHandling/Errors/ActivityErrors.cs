using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public static class ActivityErrors
{
    public static Error ActivityNotFoundCommand(int activityId) => new Error(
        "Bad Request", $"Activity with id {activityId} not found", StatusCodes.Status400BadRequest);

    public static Error MoreThanOneActiveActivity(int userId) => new Error(
        "Bad Request", $"Found more than one active activities for user with id {userId}",
        StatusCodes.Status400BadRequest);
}