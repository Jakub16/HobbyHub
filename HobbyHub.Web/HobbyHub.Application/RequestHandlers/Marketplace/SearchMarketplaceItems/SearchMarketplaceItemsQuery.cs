using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Marketplace;
using HobbyHub.Contract.Responses.Marketplace;

namespace HobbyHub.Application.RequestHandlers.Marketplace.SearchMarketplaceItems;

public class SearchMarketplaceItemsQuery(SearchMarketplaceItemsFilters filters) 
    : IHobbyHubRequest<Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>>
{
    public SearchMarketplaceItemsFilters Filters { get; } = filters;
}