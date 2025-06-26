using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Events;

public interface IEventsRepository
{
    Task SaveChanges(CancellationToken cancellationToken);
    Task Add(Event groupEvent, CancellationToken cancellationToken);
    Task Delete(Event groupEvent, CancellationToken cancellationToken);
    Task<bool> EventExists(int groupEventId, CancellationToken cancellationToken);
    Task<Event?> GetEventById(int groupEventId, CancellationToken cancellationToken);
    Task<List<Event>> GetGroupEvents(int groupId, CancellationToken cancellationToken);
    Task<List<Event>> GetUserEvents(int userId, CancellationToken cancellationToken);
    Task<List<User>> GetEventMembers(int groupEventId, CancellationToken cancellationToken);
}