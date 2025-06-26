using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.DeleteSavedMarketplaceItem;

public class DeleteSavedMarketplaceItemCommand(DeleteSavedMarketplaceItemRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int MarketplaceItemId { get; } = request.MarketplaceItemId;
    public int UserId { get; } = request.UserId;
}