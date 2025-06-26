using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public static class GroupErrors
{
    public static Error GroupNotFound(int groupId) => new(
        "Not Found", $"Group with id {groupId} not found", StatusCodes.Status404NotFound);
    public static Error GroupNotFoundCommand(int groupId) => new(
        "Bad Request", $"Group with id {groupId} not found", StatusCodes.Status400BadRequest);
    public static Error UserIsAlreadyAdded(int userId, int groupId) => new(
        "Bad Request", $"User with id {userId} is already a member of a group with id {groupId}",
        StatusCodes.Status400BadRequest);
    public static Error UserIsNotAGroupMember(int userId, int groupId) => new(
        "Bad Request", $"User with id {userId} is not a member of a group with id {groupId}",
        StatusCodes.Status400BadRequest);
}