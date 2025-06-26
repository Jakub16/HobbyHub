using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.CreateSavedMarketplaceItem;

public class CreateSavedMarketplaceItemCommand(CreateSavedMarketplaceItemRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public int MarketplaceItemId { get; } = request.MarketplaceItemId;
}