using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.SavedMarketplaceItems;

public interface ISavedMarketplaceItemsRepository
{
    Task<List<MarketplaceItem>> GetSavedMarketplaceItems(int userId, CancellationToken cancellationToken);
    Task<int> Delete(int marketplaceItemId, int userId, CancellationToken cancellationToken);
    Task<bool> IsMarketplaceItemSaved(int userId, int marketplaceItemId, CancellationToken cancellationToken);
}