namespace HobbyHub.Contract.Requests.Marketplace;

public record DeleteSavedMarketplaceItemRequest
{
    public int MarketplaceItemId { get; set; }
    public int UserId { get; set; }
}