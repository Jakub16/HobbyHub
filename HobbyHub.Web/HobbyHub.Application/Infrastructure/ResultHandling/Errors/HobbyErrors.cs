using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public static class HobbyErrors
{
    public static Error HobbyNotFound(int hobbyId) => new Error(
        "Not Found", $"Hobby with id {hobbyId} not found", StatusCodes.Status404NotFound);
    public static Error HobbyNotFoundCommand(int hobbyId) => new Error(
        "Bad Request", $"Hobby with id {hobbyId} not found", StatusCodes.Status400BadRequest);
    public static Error FavoriteHobbyNotFound(int favoriteHobbyId) => new Error(
        "Not Found", $"Favorite Hobby with id {favoriteHobbyId} not found", StatusCodes.Status404NotFound);
    public static Error HobbyAlreadyAddedToFavorites(int hobbyId, int userId, bool isHobbyPredefined) {
        if (isHobbyPredefined)
        {
            return new Error("Bad Request",
                $"Predefined Hobby with id {hobbyId} is already added to favorites for user with id {userId}",
                StatusCodes.Status400BadRequest);
        }
        return new Error("Bad Request",
            $"Custom Hobby with id {hobbyId} is already added to favorites for user with id {userId}",
            StatusCodes.Status400BadRequest);
    }
    
    public static Error HobbyWithTheSameNameAlreadyExist(string hobbyName, bool isHobbyPredefined, int? userId) {
        if (isHobbyPredefined)
        {
            return new Error("Bad Request",
                $"Predefined Hobby with name {hobbyName} already exists",
                StatusCodes.Status400BadRequest);
        }
        return new Error("Bad Request",
            $"Custom Hobby with name {hobbyName} already exists for user with id {userId}",
            StatusCodes.Status400BadRequest);
    }
}