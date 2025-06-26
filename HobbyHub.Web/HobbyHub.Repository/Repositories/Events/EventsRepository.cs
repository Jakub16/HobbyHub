using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Events;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Events;

public class EventsRepository(AppDbContext dbContext) : IEventsRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(Event groupEvent, CancellationToken cancellationToken)
    {
        await dbContext.Events.AddAsync(groupEvent, cancellationToken);
        await SaveChanges(cancellationToken);
    }

    public async Task Delete(Event groupEvent, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            dbContext.Events.Remove(groupEvent);
            await SaveChanges(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> EventExists(int groupEventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events.AsNoTracking()
            .AnyAsync(groupEvent => groupEvent.EventId == groupEventId, cancellationToken);
    }

    public async Task<Event?> GetEventById(int groupEventId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .Include(groupEvent => groupEvent.Users)
            .FirstOrDefaultAsync(groupEvent => groupEvent.EventId == groupEventId, cancellationToken);
    }

    public async Task<List<Event>> GetGroupEvents(int groupId, CancellationToken cancellationToken)
    {
        return await dbContext.Events
            .AsNoTracking()
            .Include(groupEvent => groupEvent.Group)
            .Include(groupEvent => groupEvent.Users)
            .Where(groupEvent => groupEvent.Group != null && groupEvent.Group.GroupId == groupId)
            .OrderByDescending(groupEvent => groupEvent.EventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Event>> GetUserEvents(int userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);
        
        return await dbContext.Events
            .AsNoTracking()
            .Include(groupEvent => groupEvent.Group)
            .Include(groupEvent => groupEvent.Users)
            .Where(groupEvent => user != null && groupEvent.Users.Contains(user))
            .OrderByDescending(groupEvent => groupEvent.EventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetEventMembers(int groupEventId, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Include(user => user.Events)
            .Where(user => user.Events.Any(groupEvent => groupEvent.EventId == groupEventId))
            .ToListAsync(cancellationToken);
    }
}