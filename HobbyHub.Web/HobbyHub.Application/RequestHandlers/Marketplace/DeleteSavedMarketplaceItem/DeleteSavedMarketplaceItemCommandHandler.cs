using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.DeleteSavedMarketplaceItem;

public class DeleteSavedMarketplaceItemCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteSavedMarketplaceItemCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteSavedMarketplaceItemCommand request, CancellationToken cancellationToken)
    {
        var isMarketplaceItemSaved = await repository.SavedMarketplaceItemsRepository
            .IsMarketplaceItemSaved(request.UserId, request.MarketplaceItemId, cancellationToken);

        if (!isMarketplaceItemSaved)
        {
            var error = MarketplaceErrors.SavedMarketplaceItemNotFoundCommand(request.MarketplaceItemId, request.UserId);
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var id = await repository.SavedMarketplaceItemsRepository
            .Delete(request.MarketplaceItemId, request.UserId, cancellationToken);
        
        log.Information($"Deleted saved marketplace item with id {id}");
        
        return Result<CommandResponse>.Success(new CommandResponse(id));
    }
}