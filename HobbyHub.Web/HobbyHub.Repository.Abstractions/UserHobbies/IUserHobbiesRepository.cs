using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.UserHobbies;

public interface IUserHobbiesRepository
{
    Task Add(UserHobby userHobby, CancellationToken cancellationToken);
    Task Delete(int userHobbyId, CancellationToken cancellationToken);
    Task<UserHobby?> GetUserHobbyById(int hobbyId, CancellationToken cancellationToken);
    Task<UserHobby?> GetUserHobbyByName(string hobbyName, int userId, CancellationToken cancellationToken);
    Task<List<UserHobby>> GetUserHobbiesByUserId(int userId, CancellationToken cancellationToken);
    Task<bool> UserHobbyExists(int userHobbyId, CancellationToken cancellationToken);
}