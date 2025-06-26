using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.CreateSavedMarketplaceItem;

public class CreateSavedMarketplaceItemCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<CreateSavedMarketplaceItemCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateSavedMarketplaceItemCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var marketplaceItem = await repository.MarketplaceItemsRepository
            .GetMarketplaceItemById(request.MarketplaceItemId, cancellationToken);

        if (marketplaceItem == null)
        {
            var error = MarketplaceErrors.MarketplaceItemNotFoundCommand(request.MarketplaceItemId);
            log.Warning(error.Detail);
            
            return Result<CommandResponse>.Failure(error);
        }

        if (await repository.SavedMarketplaceItemsRepository.IsMarketplaceItemSaved(request.UserId, request.MarketplaceItemId, cancellationToken))
        {
            var error = MarketplaceErrors.MarketplaceItemAlreadySavedCommand(request.MarketplaceItemId, request.UserId);
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var savedMarketPlaceItem = new SavedMarketplaceItem()
        {
            MarketplaceItem = marketplaceItem,
            User = user
        };
        
        await repository.Add(savedMarketPlaceItem, cancellationToken);
        await repository.SaveChanges(cancellationToken);
        
        log.Information($"Created saved marketplace item with id {savedMarketPlaceItem.SavedMarketplaceItemId}");
        log.Information($"Marketplace item with id {request.MarketplaceItemId} added to saved for user with id {request.UserId}");

        return Result<CommandResponse>.Success(new CommandResponse(savedMarketPlaceItem.SavedMarketplaceItemId));
    }
}