using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetUserOrderedMarketplaceItems;

public class GetUserOrderedMarketplaceItemsQuery(int userId) : IHobbyHubRequest<Result<ListResponse<MarketplaceItemResponse>>>
{
    public int UserId { get; } = userId;
}