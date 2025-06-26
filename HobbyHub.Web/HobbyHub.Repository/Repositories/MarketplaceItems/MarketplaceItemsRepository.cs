using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.MarketplaceItems;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.MarketplaceItems;

public class MarketplaceItemsRepository(AppDbContext dbContext) : IMarketplaceItemsRepository
{
    public async Task Add(MarketplaceItem marketplaceItem, CancellationToken cancellationToken)
    {
        await dbContext.MarketplaceItems.AddAsync(marketplaceItem, cancellationToken);
    }

    public async Task Delete(MarketplaceItem marketplaceItem, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            dbContext.MarketplaceItems.Remove(marketplaceItem);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<MarketplaceItem?> GetMarketplaceItemById(int marketplaceItemId, CancellationToken cancellationToken)
    {
        return await dbContext.MarketplaceItems
            .Include(marketplaceItem => marketplaceItem.User)
            .Include(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .FirstOrDefaultAsync(marketplaceItem => 
                marketplaceItem.MarketplaceItemId == marketplaceItemId, cancellationToken);
    }

    public async Task<List<MarketplaceItem>> GetUserMarketplaceItems(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.MarketplaceItems
            .AsNoTracking()
            .Include(marketplaceItem => marketplaceItem.User)
            .Include(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .Where(marketplaceItem => marketplaceItem.User.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MarketplaceItem>> GetUserOrderedMarketplaceItems(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.MarketplaceItems
            .AsNoTracking()
            .Include(marketplaceItem => marketplaceItem.User)
            .Include(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .Where(marketplaceItem => marketplaceItem.BoughtBy == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MarketplaceItem>> GetMarketplaceItems(CancellationToken cancellationToken)
    {
        return await dbContext.MarketplaceItems
            .AsNoTracking()
            .Include(marketplaceItem => marketplaceItem.User)
            .Include(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .ToListAsync(cancellationToken);
    }

    public async Task<MarketplaceItemsWithPaging> SearchMarketPlaceItems(
        SearchMarketplaceItemsFilters filters,
        CancellationToken cancellationToken)
    {
        var query = dbContext.MarketplaceItems
            .AsNoTracking()
            .AsQueryable()
            .Where(item => item.IsSold == false)
            .Where(item => item.User.UserId != filters.UserId);
        
        if (!string.IsNullOrEmpty(filters.Category))
        {
            query = query.Where(item =>
                item.Category.ToLower() == filters.Category.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filters.Type))
        {
            query = query.Where(item =>
                item.Type.ToLower() == filters.Type.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filters.City))
        {
            query = query.Where(item => 
                item.City != null && item.City.ToLower() == filters.City.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filters.Country))
        {
            query = query.Where(item => 
                item.Country != null && item.Country.ToLower() == filters.Country.ToLower());
        }
        
        if (!string.IsNullOrEmpty(filters.Keyword))
        {
            query = query.Where(item =>
                EF.Functions.Like(item.Title.ToLower(), $"%{filters.Keyword.ToLower()}%")
                || (item.Description != null && EF.Functions.Like(item.Description.ToLower(),
                    $"%{filters.Keyword.ToLower()}%")));
        }

        if (filters.PriceRange != null)
        {
            if (filters.PriceRange.From != 0)
            {
                query = query.Where(item => item.Price >= filters.PriceRange.From);
            }
            
            if (filters.PriceRange.To != 0)
            {
                query = query.Where(item => item.Price <= filters.PriceRange.To);
            }
        }

        if (filters.PriceSortDirection != null)
        {
            query = filters.PriceSortDirection switch
            {
                "asc" => query.OrderBy(item => item.Price),
                "desc" => query.OrderByDescending(item => item.Price),
                _ => query.OrderByDescending(item => item.MarketplaceItemId)
            };
        }
        else
        {
            query = query.OrderByDescending(item => item.MarketplaceItemId);
        }
        
        var totalRecords = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize);
            
        var marketplaceItems = await query
            .Include(marketplaceItem => marketplaceItem.User)
            .Include(marketplaceItem => marketplaceItem.MarketplaceItemPictures)
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        return new MarketplaceItemsWithPaging()
        {
            MarketplaceItems = marketplaceItems,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }
}