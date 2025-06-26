using HobbyHub.Contract.Requests.Tutorials;
using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Tutorials;

public interface ITutorialsRepository
{
    Task<Tutorial?> GetTutorialById(int tutorialId, CancellationToken cancellationToken);
    Task<List<Tutorial>> GetAllTutorials(CancellationToken cancellationToken);
    Task<List<Tutorial>> GetUserTutorials(int userId, CancellationToken cancellationToken);
    Task<List<Tutorial>> GetUserAssignedTutorials(int userId, CancellationToken cancellationToken);
    Task<TutorialsWithPaging> SearchTutorials(SearchTutorialsFilters filters, CancellationToken cancellationToken);
}

public record TutorialsWithPaging
{
    public List<Tutorial> Tutorials { get; init; }
    public int TotalRecords { get; init; }
    public int TotalPages { get; set; }
}