using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.SavedMarketplaceItems;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.SavedMarketplaceItems;

public class SavedMarketplaceItemsRepository(AppDbContext dbContext) : ISavedMarketplaceItemsRepository
{
    public async Task<List<MarketplaceItem>> GetSavedMarketplaceItems(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.SavedMarketplaceItems
            .AsNoTracking()
            .Include(savedMarketplaceItem => savedMarketplaceItem.User)
            .Include(savedMarketplaceItem => savedMarketplaceItem.MarketplaceItem)
            .ThenInclude(marketplaceItem => marketplaceItem.User)
            .Include(savedMarketplaceItem => savedMarketplaceItem.MarketplaceItem)
            .ThenInclude(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .Where(savedMarketplaceItem => savedMarketplaceItem.User.UserId == userId)
            .OrderByDescending(savedMarketplaceItem => savedMarketplaceItem.SavedMarketplaceItemId)
            .Select(savedMarketplaceItem => savedMarketplaceItem.MarketplaceItem)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> Delete(int marketplaceItemId, int userId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var savedMarketplaceItem = await dbContext.SavedMarketplaceItems
            .Include(item => item.MarketplaceItem)
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.MarketplaceItem.MarketplaceItemId == marketplaceItemId 
                                         && item.User.UserId == userId, cancellationToken);
        
        try
        {
            dbContext.SavedMarketplaceItems.Remove(savedMarketplaceItem);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return savedMarketplaceItem.SavedMarketplaceItemId;
    }

    public Task<bool> IsMarketplaceItemSaved(int userId, int marketplaceItemId, CancellationToken cancellationToken)
    {
        return dbContext.SavedMarketplaceItems
            .AsNoTracking()
            .Include(savedMarketplaceItem => savedMarketplaceItem.MarketplaceItem)
            .Include(savedMarketplaceItem => savedMarketplaceItem.User)
            .AnyAsync(savedMarketplaceItem =>
                savedMarketplaceItem.User.UserId == userId 
                && savedMarketplaceItem.MarketplaceItem.MarketplaceItemId == marketplaceItemId,
                cancellationToken);
    }
}