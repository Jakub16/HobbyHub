using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Activities;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Activities;

public class ActivitiesRepository(AppDbContext dbContext) : IActivitiesRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(Activity activity, CancellationToken cancellationToken)
    {
        await dbContext.Activities.AddAsync(activity, cancellationToken);
        await SaveChanges(cancellationToken);
    }

    public async Task Delete(Activity activity, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            dbContext.Activities.Remove(activity);
            await SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<Activity?> GetActivityById(int activityId, CancellationToken cancellationToken)
    {
        return await dbContext.Activities.FirstOrDefaultAsync(activity =>
            activity.ActivityId == activityId, cancellationToken);
    }

    public async Task<List<Activity>> GetUserActiveActivities(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.User.UserId == userId && activity.EndTime == null)
            .Include(activity => activity.Hobby)
            .Include(activity => activity.UserHobby)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Activity>> GetUserEndedActivities(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.User.UserId == userId && activity.EndTime != null)
            .Include(activity => activity.Hobby)
            .Include(activity => activity.UserHobby)
            .ToListAsync(cancellationToken);
    }
}