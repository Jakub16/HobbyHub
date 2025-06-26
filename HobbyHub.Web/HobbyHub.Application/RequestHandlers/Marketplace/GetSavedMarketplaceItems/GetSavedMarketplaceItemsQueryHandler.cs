using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Marketplace;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.GetSavedMarketplaceItems;

public class GetSavedMarketplaceItemsQueryHandler(
    IHobbyHubRepository repository,
    IMapper mapper,
    IMarketplaceItemMapper marketplaceItemMapper,
    ILogger logger)
    : IRequestHandler<GetSavedMarketplaceItemsQuery, Result<ListResponse<MarketplaceItemResponse>>>
{
    public async Task<Result<ListResponse<MarketplaceItemResponse>>> Handle(GetSavedMarketplaceItemsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            logger.Warning(error.Detail);
            return Result<ListResponse<MarketplaceItemResponse>>.Failure(error);
        }
        
        var marketplaceItems = await repository.SavedMarketplaceItemsRepository
            .GetSavedMarketplaceItems(request.UserId, cancellationToken);
        
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
        
        result.ForEach(item => item.IsSaved = true);
        
        logger.Information($"Found {result.Count} saved marketplace items for user with id {request.UserId}");
        
        return Result<ListResponse<MarketplaceItemResponse>>.Success(new ListResponse<MarketplaceItemResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}