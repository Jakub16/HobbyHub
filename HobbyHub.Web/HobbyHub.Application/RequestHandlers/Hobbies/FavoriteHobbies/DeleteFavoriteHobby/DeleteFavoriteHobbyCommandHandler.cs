using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.DeleteFavoriteHobby;

public class DeleteFavoriteHobbyCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<DeleteFavoriteHobbyCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(DeleteFavoriteHobbyCommand request, CancellationToken cancellationToken)
    {
        var favoriteHobby =
            await repository.FavoriteHobbiesRepository.GetFavoriteHobbyById(request.FavoriteHobbyId, cancellationToken);

        if (favoriteHobby == null)
        {
            var error = HobbyErrors.FavoriteHobbyNotFound(request.FavoriteHobbyId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        await repository.FavoriteHobbiesRepository.Delete(request.FavoriteHobbyId, cancellationToken);
        await repository.FavoriteHobbiesRepository.SaveChanges(cancellationToken);
        
        log.Information($"Successfully deleted favorite hobby with id {request.FavoriteHobbyId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(request.FavoriteHobbyId));
    }
}