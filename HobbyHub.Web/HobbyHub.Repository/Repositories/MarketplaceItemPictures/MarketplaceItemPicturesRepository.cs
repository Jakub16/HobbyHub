using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.MarketplaceItemPictures;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.MarketplaceItemPictures;

public class MarketplaceItemPicturesRepository(AppDbContext dbContext) : IMarketplaceItemPicturesRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(MarketplaceItemPicture marketplaceItemPicture, CancellationToken cancellationToken)
    {
        await dbContext.MarketplaceItemPictures.AddAsync(marketplaceItemPicture, cancellationToken);
    }

    public async Task<MarketplaceItemPicture> GetMarketplaceItemPictureById(int marketplaceItemPictureId, CancellationToken cancellationToken)
    {
        return await dbContext.MarketplaceItemPictures
            .SingleAsync(picture => picture.MarketplaceItemPictureId == marketplaceItemPictureId, cancellationToken);
    }
}