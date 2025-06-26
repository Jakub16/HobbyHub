using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.MarketplaceItems;

public interface IMarketplaceItemsRepository
{
    Task Add(MarketplaceItem marketplaceItem, CancellationToken cancellationToken);
    Task Delete(MarketplaceItem marketplaceItem, CancellationToken cancellationToken);
    Task<MarketplaceItem?> GetMarketplaceItemById(int marketplaceItemId, CancellationToken cancellationToken);
    Task<List<MarketplaceItem>> GetUserMarketplaceItems(int userId, CancellationToken cancellationToken);
    Task<List<MarketplaceItem>> GetUserOrderedMarketplaceItems(int userId, CancellationToken cancellationToken);
    Task<List<MarketplaceItem>> GetMarketplaceItems(CancellationToken cancellationToken);
    Task<MarketplaceItemsWithPaging> SearchMarketPlaceItems(SearchMarketplaceItemsFilters filters,
        CancellationToken cancellationToken);
}

public record MarketplaceItemsWithPaging
{
    public List<MarketplaceItem> MarketplaceItems { get; init; }
    public int TotalRecords { get; init; }
    public int TotalPages { get; set; }
}