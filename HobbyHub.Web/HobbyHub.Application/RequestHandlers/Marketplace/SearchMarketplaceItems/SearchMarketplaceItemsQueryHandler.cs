using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.SearchMarketplaceItems;

public class SearchMarketplaceItemsQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    IMarketplaceItemMapper marketplaceItemMapper,
    ILogger log)
    : IRequestHandler<SearchMarketplaceItemsQuery, Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>>
{
    public async Task<Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>>
        Handle(SearchMarketplaceItemsQuery request, CancellationToken cancellationToken)
    {
        var marketplaceItems =
            await repository.MarketplaceItemsRepository.SearchMarketPlaceItems(request.Filters, cancellationToken);
        var userSavedMarketplaceItems =
            await repository.SavedMarketplaceItemsRepository.GetSavedMarketplaceItems(request.Filters.UserId,
                cancellationToken);
        
        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        var creatorsIds = marketplaceItems.MarketplaceItems.Select(marketplaceItem => marketplaceItem.User.UserId).Distinct().ToList();
        var creators = await repository.UsersRepository.GetUsersByIds(creatorsIds, cancellationToken);
        
        marketplaceItems.MarketplaceItems.ForEach(marketplaceItem =>
        {
            var user = creators.FirstOrDefault(creator => creator.UserId == marketplaceItem.User.UserId);
            var userResponse = mapper.Map<UserSummaryResponse>(user);
            
            userDictionary.Add(marketplaceItem.MarketplaceItemId, userResponse);
        });

        var result = marketplaceItemMapper.Map(marketplaceItems.MarketplaceItems, userDictionary);
        
        result.ForEach(item => item.IsSaved = userSavedMarketplaceItems.Any(savedItem =>
            savedItem.MarketplaceItemId == item.MarketplaceItemId));
        
        log.Information($"Found {result.Count} marketplace items for page {request.Filters.PageNumber}");
        
        return Result<PagedResponse<ListResponse<MarketplaceItemResponse>>>.Success(
            new PagedResponse<ListResponse<MarketplaceItemResponse>>
        {
            Data = new ListResponse<MarketplaceItemResponse>()
            {
                Count = result.Count,
                Items = result
            },
            PageNumber = request.Filters.PageNumber,
            PageSize = request.Filters.PageSize,
            TotalPages = marketplaceItems.TotalPages,
            TotalRecords = marketplaceItems.TotalRecords
        });
    }
}