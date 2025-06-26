using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetMarketplaceItems;

public class GetMarketplaceItemsQuery : IHobbyHubRequest<Result<ListResponse<MarketplaceItemResponse>>>
{
    
}