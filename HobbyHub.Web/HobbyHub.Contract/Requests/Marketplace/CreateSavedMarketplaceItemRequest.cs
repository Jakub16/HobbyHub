namespace HobbyHub.Contract.Requests.Marketplace;

public record CreateSavedMarketplaceItemRequest
{
    public int UserId { get; set; }
    public int MarketplaceItemId { get; set; }
}