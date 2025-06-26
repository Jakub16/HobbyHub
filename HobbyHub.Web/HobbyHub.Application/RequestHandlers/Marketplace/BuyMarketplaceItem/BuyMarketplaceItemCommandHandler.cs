using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.BuyMarketplaceItem;

public class BuyMarketplaceItemCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<BuyMarketplaceItemCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(BuyMarketplaceItemCommand request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
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
        
        marketplaceItem.BoughtBy = request.UserId;
        marketplaceItem.IsSold = true;

        await repository.SaveChanges(cancellationToken);
        
        log.Information($"User with id {request.UserId} successfully bought marketplace item with id {request.MarketplaceItemId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(marketplaceItem.MarketplaceItemId));
    }
}