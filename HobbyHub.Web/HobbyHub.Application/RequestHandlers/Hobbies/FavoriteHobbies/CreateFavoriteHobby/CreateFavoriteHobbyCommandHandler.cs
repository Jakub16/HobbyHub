using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.CreateFavoriteHobby;

public class CreateFavoriteHobbyCommandHandler(
    IHobbyHubRepository repository,
    IFavoriteHobbiesService favoriteHobbiesService,
    ILogger log)
    : IRequestHandler<CreateFavoriteHobbyCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateFavoriteHobbyCommand request, CancellationToken cancellationToken)
    {
        Hobby? hobby = null;
        UserHobby? userHobby = null;

        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var existingFavoriteHobbies =
            await favoriteHobbiesService.GetUserFavoriteHobbies(request.UserId, cancellationToken);
        var identicalFavoriteHobby = existingFavoriteHobbies
            .FirstOrDefault(favoriteHobby =>
                favoriteHobby.HobbyId == request.HobbyId && favoriteHobby.IsPredefined == request.IsHobbyPredefined);

        if (identicalFavoriteHobby != null)
        {
            var error = HobbyErrors.HobbyAlreadyAddedToFavorites(
                request.HobbyId, request.UserId, request.IsHobbyPredefined);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        if (request.IsHobbyPredefined)
        {
            hobby = await repository.HobbiesRepository.GetHobbyById(request.HobbyId, cancellationToken);
        }
        else
        {
            userHobby = await repository.UserHobbiesRepository.GetUserHobbyById(request.HobbyId, cancellationToken);
        }
        
        if (hobby == null && userHobby == null)
        {
            var error = HobbyErrors.HobbyNotFoundCommand(request.HobbyId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var favoriteHobby = new FavoriteHobby()
        {
            User = user,
            Hobby = hobby ?? null,
            UserHobby = userHobby ?? null
        };

        await repository.FavoriteHobbiesRepository.Add(favoriteHobby, cancellationToken);
        log.Information($"Created favorite hobby with id {favoriteHobby.FavoriteHobbyId} for user with id {request.UserId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(favoriteHobby.FavoriteHobbyId));
    }
}