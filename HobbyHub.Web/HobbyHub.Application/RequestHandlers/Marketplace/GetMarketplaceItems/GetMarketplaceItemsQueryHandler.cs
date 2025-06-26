using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetMarketplaceItems;

public class GetMarketplaceItemsQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    IMarketplaceItemMapper marketplaceItemMapper,
    ILogger log)
    : IRequestHandler<GetMarketplaceItemsQuery, Result<ListResponse<MarketplaceItemResponse>>>
{
    public async Task<Result<ListResponse<MarketplaceItemResponse>>> Handle(GetMarketplaceItemsQuery request, CancellationToken cancellationToken)
    {
        var marketplaceItems =  await repository.MarketplaceItemsRepository
            .GetMarketplaceItems(cancellationToken);

        var userDictionary = new Dictionary<int, UserSummaryResponse>();
        
        var creatorsIds = marketplaceItems.Select(marketplaceItem => marketplaceItem.User.UserId).Distinct().ToList();
        var creators = await repository.UsersRepository.GetUsersByIds(creatorsIds, cancellationToken);
        
        marketplaceItems.ForEach(marketplaceItem =>
        {
            var user = creators.FirstOrDefault(creator => creator.UserId == marketplaceItem.User.UserId);
            var userResponse = mapper.Map<UserSummaryResponse>(user);
            
            userDictionary.Add(marketplaceItem.MarketplaceItemId, userResponse);
        });

        var result = marketplaceItemMapper.Map(marketplaceItems, userDictionary);
        
        log.Information($"Found {result.Count} marketplace items");
        
        return Result<ListResponse<MarketplaceItemResponse>>.Success(new ListResponse<MarketplaceItemResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}