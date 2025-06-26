using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Database.Entities;

namespace HobbyHub.Application.RequestHandlers.Marketplace;

public interface IMarketplaceItemMapper
{
    List<MarketplaceItemResponse> Map(List<MarketplaceItem> marketplaceItems,
        Dictionary<int, UserSummaryResponse> userDictionary);
}

public class MarketplaceItemMapper : IMarketplaceItemMapper
{
    public List<MarketplaceItemResponse> Map(List<MarketplaceItem> marketplaceItems,
        Dictionary<int, UserSummaryResponse> userDictionary)
    {
        return marketplaceItems.Select(marketplaceItem =>
        {
            return new MarketplaceItemResponse()
            {
                MarketplaceItemId = marketplaceItem.MarketplaceItemId,
                City = marketplaceItem.City,
                Country = marketplaceItem.Country,
                Description = marketplaceItem.Description,
                PathsToPictures = marketplaceItem.MarketplaceItemPictures
                    .OrderBy(picture => picture.MarketplaceItemPictureId)
                    .Select(x => x.PathToPicture).ToList(),
                PostalCode = marketplaceItem.PostalCode,
                PhoneNumber = marketplaceItem.PhoneNumber,
                Price = marketplaceItem.Price,
                Title = marketplaceItem.Title,
                Type = marketplaceItem.Type,
                Category = marketplaceItem.Category,
                UserSummary = userDictionary[marketplaceItem.MarketplaceItemId],
                IsSold = marketplaceItem.IsSold,
                BoughtBy = marketplaceItem.BoughtBy
            };
        }).ToList();
    }
}