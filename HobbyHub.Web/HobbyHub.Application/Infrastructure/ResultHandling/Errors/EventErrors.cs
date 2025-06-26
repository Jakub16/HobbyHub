using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public class EventErrors
{
    public static Error EventNotFound(int eventId) => new(
        "Not Found", $"Event with id {eventId} not found", StatusCodes.Status404NotFound);
    public static Error EventNotFoundCommand(int eventId) => new(
        "Bad Request", $"Event with id {eventId} not found", StatusCodes.Status400BadRequest);
    
    public static Error UserIsAlreadyAdded(int userId, int eventId) => new(
        "Bad Request", $"User with id {userId} is already a member of an event with id {eventId}",
        StatusCodes.Status400BadRequest);
}