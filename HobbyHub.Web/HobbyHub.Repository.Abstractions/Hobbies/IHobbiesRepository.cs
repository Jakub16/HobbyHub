using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Hobbies;

public interface IHobbiesRepository
{
    Task Add(Hobby hobby, CancellationToken cancellationToken);
    Task<Hobby?> GetHobbyById(int hobbyId, CancellationToken cancellationToken);
    Task<Hobby?> GetHobbyByName(string hobbyName, CancellationToken cancellationToken);
    Task<List<Hobby>> GetAllHobbies(CancellationToken cancellationToken);
}