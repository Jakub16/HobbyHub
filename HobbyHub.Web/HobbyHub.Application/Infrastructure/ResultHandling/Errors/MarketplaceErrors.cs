using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.Infrastructure.ResultHandling.Errors;

public class MarketplaceErrors
{
    public static Error MarketplaceItemNotFoundCommand(int marketplaceItemId) => new Error(
        "Not Found", $"Marketplace item with id {marketplaceItemId} not found", StatusCodes.Status400BadRequest);
    public static Error MarketplaceItemAlreadySavedCommand(int marketplaceItemId, int userId) => new Error(
        "Bad Request", $"Marketplace item with id {marketplaceItemId} is already saved for user with id {userId}", StatusCodes.Status400BadRequest);
    public static Error SavedMarketplaceItemNotFoundCommand(int marketplaceItemId, int userId) => new Error(
        "Not Found", $"Marketplace item with id {marketplaceItemId} is not saved for user with id {userId}", StatusCodes.Status400BadRequest);
}
