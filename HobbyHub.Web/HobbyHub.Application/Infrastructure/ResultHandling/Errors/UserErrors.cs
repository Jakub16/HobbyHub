using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public static class UserErrors
{
    public static Error EmailAlreadyRegistered(string email) => new(
        "Bad Request", $"Email {email} is already registered", StatusCodes.Status400BadRequest);
    
    public static readonly Error InvalidEmailOrPassword = new(
        "Unauthorized", "Invalid email or password", StatusCodes.Status401Unauthorized);
    public static readonly Error CannotFollowItself = new(
        "Bad Request", "User cannot follow itself", StatusCodes.Status400BadRequest);

    public static Error UserNotFound(int userId) => new(
        "Not Found", $"User with id {userId} not found", StatusCodes.Status404NotFound);
    public static Error UserNotFoundCommand(int userId) => new(
        "Bad request", $"User with id {userId} not found", StatusCodes.Status400BadRequest);
    
    public static Error UserIsAlreadyFollowed(int userId, int userToFollowId) => new(
        "Bad Request", $"User with id {userId} already follows user with id {userToFollowId}",
        StatusCodes.Status400BadRequest);
    
    public static Error UserIsNotFollowed(int userId, int userToFollowId) => new(
        "Bad Request", $"User with id {userId} doesn't follow user with id {userToFollowId}",
        StatusCodes.Status400BadRequest);
}