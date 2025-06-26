using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;

namespace HobbyHub.Application.RequestHandlers.Hobbies.FavoriteHobbies;

public interface IFavoriteHobbiesService
{
    Task<List<FavoriteHobbyResponse>> GetUserFavoriteHobbies(int userId, CancellationToken cancellationToken);
}

public class FavoriteHobbiesService(IHobbyHubRepository repository) : IFavoriteHobbiesService
{
    public async Task<List<FavoriteHobbyResponse>> GetUserFavoriteHobbies(int userId, CancellationToken cancellationToken)
    {
        var favoriteHobbies =
            await repository.FavoriteHobbiesRepository.GetUserFavoriteHobbies(userId, cancellationToken);

        var items = new List<FavoriteHobbyResponse>();
        
        favoriteHobbies.ForEach(favoriteHobby =>
        {
            if (favoriteHobby.Hobby != null && favoriteHobby.UserHobby == null)
            {
                items.Add(new FavoriteHobbyResponse()
                {
                    FavoriteHobbyId = favoriteHobby.FavoriteHobbyId,
                    HobbyId = favoriteHobby.Hobby.HobbyId,
                    Name = favoriteHobby.Hobby.Name,
                    IconType = favoriteHobby.Hobby.IconType,
                    IsPredefined = true
                });
            }
            
            if (favoriteHobby.UserHobby != null && favoriteHobby.Hobby == null)
            {
                items.Add(new FavoriteHobbyResponse()
                {
                    FavoriteHobbyId = favoriteHobby.FavoriteHobbyId,
                    HobbyId = favoriteHobby.UserHobby.UserHobbyId,
                    Name = favoriteHobby.UserHobby.Name,
                    IconType = favoriteHobby.UserHobby.IconType,
                    IsPredefined = false
                });
            }
        });
        return items;
    }
}