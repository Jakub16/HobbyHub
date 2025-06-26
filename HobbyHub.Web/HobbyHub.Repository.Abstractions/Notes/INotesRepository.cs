using HobbyHub.Database.Entities;

namespace HobbyHub.Repository.Abstractions.Notes;

public interface INotesRepository
{
    Task Add(Note note, CancellationToken cancellationToken);
}