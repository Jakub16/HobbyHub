namespace HobbyHub.Repository.Abstractions.FavoriteHobbies;

public interface IFavoriteHobbiesRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(Database.Entities.FavoriteHobby favoriteHobby, CancellationToken cancellationToken);
    Task Delete(int favoriteHobbyId, CancellationToken cancellationToken);
    Task<Database.Entities.FavoriteHobby?> GetFavoriteHobbyById(int favoriteHobbyId, CancellationToken cancellationToken);
    Task<List<Database.Entities.FavoriteHobby>> GetUserFavoriteHobbies(int userId, CancellationToken cancellationToken);
}