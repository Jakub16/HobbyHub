using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.DeleteMarketplaceItem;

public class DeleteMarketplaceItemCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteMarketplaceItemCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteMarketplaceItemCommand request, CancellationToken cancellationToken)
    {
        var marketplaceItem = await repository.MarketplaceItemsRepository
            .GetMarketplaceItemById(request.MarketplaceItemId, cancellationToken);

        if (marketplaceItem == null)
        {
            var error = MarketplaceErrors.MarketplaceItemNotFoundCommand(request.MarketplaceItemId);
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        await repository.MarketplaceItemsRepository.Delete(marketplaceItem, cancellationToken);
        
        log.Information($"Successfully deleted marketplace item with id {request.MarketplaceItemId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(marketplaceItem.MarketplaceItemId));
    }
    
}