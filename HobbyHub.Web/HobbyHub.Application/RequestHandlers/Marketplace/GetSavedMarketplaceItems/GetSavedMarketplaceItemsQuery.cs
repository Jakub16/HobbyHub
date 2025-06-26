using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetSavedMarketplaceItems;

public class GetSavedMarketplaceItemsQuery(int userId) : IHobbyHubRequest<Result<ListResponse<MarketplaceItemResponse>>>
{
    public int UserId { get; } = userId;
}