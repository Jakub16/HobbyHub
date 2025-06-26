using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.DeleteMarketplaceItem;

public class DeleteMarketplaceItemCommand(int marketplaceItemId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int MarketplaceItemId { get; } = marketplaceItemId;
}