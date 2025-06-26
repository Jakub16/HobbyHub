using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies.GetFavoriteHobbies;

public class GetFavoriteHobbiesQueryHandler(
    IHobbyHubRepository repository,
    IFavoriteHobbiesService favoriteHobbiesService,
    ILogger log)
    : IRequestHandler<GetFavoriteHobbiesQuery, Result<ListResponse<FavoriteHobbyResponse>>>
{
    public async Task<Result<ListResponse<FavoriteHobbyResponse>>> Handle(GetFavoriteHobbiesQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<FavoriteHobbyResponse>>.Failure(error);
        }

        var items = await favoriteHobbiesService.GetUserFavoriteHobbies(
            request.UserId, cancellationToken);
        
        log.Information($"Found {items.Count} favorite hobbies for user with id {request.UserId}");
        
        return Result<ListResponse<FavoriteHobbyResponse>>.Success(new ListResponse<FavoriteHobbyResponse>()
        {
            Items = items,
            Count = items.Count
        });
    }
}