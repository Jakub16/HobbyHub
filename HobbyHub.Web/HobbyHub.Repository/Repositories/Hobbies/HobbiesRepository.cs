using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.Hobbies;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.Hobbies;

public class HobbiesRepository(AppDbContext dbContext) : IHobbiesRepository
{
    public async Task Add(Hobby hobby, CancellationToken cancellationToken)
    {
        await dbContext.Hobbies.AddAsync(hobby, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Hobby?> GetHobbyById(int hobbyId, CancellationToken cancellationToken)
    {
        return await dbContext.Hobbies.FirstOrDefaultAsync(hobby => hobby.HobbyId == hobbyId, cancellationToken);
    }

    public async Task<Hobby?> GetHobbyByName(string hobbyName, CancellationToken cancellationToken)
    {
        return await dbContext.Hobbies.AsNoTracking().FirstOrDefaultAsync(hobby => hobby.Name == hobbyName, cancellationToken);
    }

    public async Task<List<Hobby>> GetAllHobbies(CancellationToken cancellationToken)
    {
        return await dbContext.Hobbies.AsNoTracking().ToListAsync(cancellationToken);
    }
}