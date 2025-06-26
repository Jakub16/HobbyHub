using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Notes;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Notes;

public class NotesRepository(AppDbContext dbContext) : INotesRepository
{
    public async Task Add(Note note, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(note, cancellationToken);
    }
}