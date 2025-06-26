using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Marketplace.CreateMarketplaceItem;

public class CreateMarketplaceItemCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateMarketplaceItemCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateMarketplaceItemCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            log.Warning(error.Detail);
            
            return Result<CommandResponse>.Failure(error);
        }
        
        var marketplaceItem = new MarketplaceItem()
        {
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Title = request.Title,
            Description = request.Description,
            PhoneNumber = request.PhoneNumber,
            Price = request.Price,
            Type = request.Type,
            Category = request.Category,
            User = user,
            TimeOfCreation = DateTime.Now
        };

        await repository.MarketplaceItemsRepository.Add(marketplaceItem, cancellationToken);
        await repository.SaveChanges(cancellationToken);
        
        request.Pictures?.ForEach(x => AddPicture(x).GetAwaiter().GetResult());

        log.Information($"Created marketplace item with id {marketplaceItem.MarketplaceItemId} with {request.Pictures?.Count} Pictures");
        
        return Result<CommandResponse>.Success(new CommandResponse(marketplaceItem.MarketplaceItemId));
        
        async Task AddPicture(IFormFile requestPicture)
        {
            var marketplaceItemPicture = new MarketplaceItemPicture()
            {
                MarketplaceItem = marketplaceItem
            };

            await repository.MarketplaceItemPicturesRepository.Add(marketplaceItemPicture, cancellationToken);
            await repository.MarketplaceItemPicturesRepository.SaveChanges(cancellationToken);
            
            var pictureUrl = await fileUploader.Send(
                "marketplace",
                $"marketplaceItem_{marketplaceItem.MarketplaceItemId.ToString()}/marketplaceItemPicture_{marketplaceItemPicture.MarketplaceItemPictureId}",
                requestPicture);

            var foundMarketplaceItemPicture =
                await repository.MarketplaceItemPicturesRepository
                    .GetMarketplaceItemPictureById(marketplaceItemPicture.MarketplaceItemPictureId, cancellationToken);

            foundMarketplaceItemPicture.PathToPicture = pictureUrl;
            await repository.MarketplaceItemPicturesRepository.SaveChanges(cancellationToken);
        }
    }
}