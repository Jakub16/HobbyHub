using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.FavoriteHobbies;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.FavoriteHobby;

public class FavoriteHobbiesRepository(AppDbContext dbContext) : IFavoriteHobbiesRepository
{
    public async Task SaveChanges(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Add(Database.Entities.FavoriteHobby favoriteHobby, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(favoriteHobby, cancellationToken);
        await SaveChanges(cancellationToken);
    }

    public async Task Delete(int favoriteHobbyId, CancellationToken cancellationToken)
    {
        await dbContext.FavoriteHobbies
            .Where(favoriteHobby => favoriteHobby.FavoriteHobbyId == favoriteHobbyId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Database.Entities.FavoriteHobby?> GetFavoriteHobbyById(int favoriteHobbyId, CancellationToken cancellationToken)
    {
        return await dbContext.FavoriteHobbies.FirstOrDefaultAsync(
            favoriteHobby => favoriteHobby.FavoriteHobbyId == favoriteHobbyId, cancellationToken);
    }

    public async Task<List<Database.Entities.FavoriteHobby>> GetUserFavoriteHobbies(int userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.FavoriteHobbies
            .AsNoTracking()
            .Include(favoriteHobby => favoriteHobby.User)
            .Include(favoriteHobby => favoriteHobby.Hobby)
            .Include(favoriteHobby => favoriteHobby.UserHobby)
            .Where(favoriteHobby => favoriteHobby.User.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}