using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Activities;

public interface IActivitiesRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(Activity activity, CancellationToken cancellationToken);
    Task Delete(Activity activity, CancellationToken cancellationToken);
    Task<Activity?> GetActivityById(int activityId, CancellationToken cancellationToken);
    Task<List<Activity>> GetUserActiveActivities(int userId, CancellationToken cancellationToken);
    Task<List<Activity>> GetUserEndedActivities(int userId, CancellationToken cancellationToken);
}