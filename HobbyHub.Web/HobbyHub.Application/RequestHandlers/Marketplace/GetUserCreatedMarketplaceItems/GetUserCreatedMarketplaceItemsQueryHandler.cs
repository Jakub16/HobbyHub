using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetUserCreatedMarketplaceItems;

public class GetUserCreatedMarketplaceItemsQueryHandler(
    IHobbyHubRepository repository,
    IMarketplaceItemMapper marketplaceItemMapper,
    IMapper mapper,
    ILogger log)
    : IRequestHandler<GetUserCreatedMarketplaceItemsQuery, Result<ListResponse<MarketplaceItemResponse>>>
{
    public async Task<Result<ListResponse<MarketplaceItemResponse>>> Handle(GetUserCreatedMarketplaceItemsQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<MarketplaceItemResponse>>.Failure(error);
        }
        
        var marketplaceItems =  await repository.MarketplaceItemsRepository
            .GetUserMarketplaceItems(request.UserId, cancellationToken);
        var userSavedMarketplaceItems =
            await repository.SavedMarketplaceItemsRepository.GetSavedMarketplaceItems(request.UserId,
                cancellationToken);

        var userDictionary = new Dictionary<int, UserSummaryResponse>();

        var creator = mapper.Map<UserSummaryResponse>(user);
        
        marketplaceItems.ForEach(marketplaceItem =>
        {
            userDictionary.Add(marketplaceItem.MarketplaceItemId, creator);
        });

        var result = marketplaceItemMapper.Map(marketplaceItems, userDictionary);
        
        result.ForEach(item => item.IsSaved = userSavedMarketplaceItems.Any(savedItem =>
            savedItem.MarketplaceItemId == item.MarketplaceItemId));
        
        log.Information($"Found {result.Count} marketplace items created by user with id {request.UserId}");
        
        return Result<ListResponse<MarketplaceItemResponse>>.Success(new ListResponse<MarketplaceItemResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}