using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.MarketplaceItemPictures;

public interface IMarketplaceItemPicturesRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(MarketplaceItemPicture marketplaceItemPicture, CancellationToken cancellationToken);
    Task<MarketplaceItemPicture> GetMarketplaceItemPictureById(int marketplaceItemPictureId, CancellationToken cancellationToken);
}