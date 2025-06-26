using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.BuyMarketplaceItem;

public class BuyMarketplaceItemCommand(int userId, int marketplaceItemId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = userId;
    public int MarketplaceItemId { get; } = marketplaceItemId;
}