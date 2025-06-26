using HobbyHub.Database.Entities;
using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.UserHobbies;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.UserHobbies;

public class UserHobbiesRepository(AppDbContext dbContext) : IUserHobbiesRepository
{
    public async Task Add(UserHobby userHobby, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(userHobby, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(int userHobbyId, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await dbContext.FavoriteHobbies
                .Include(favoriteHobby => favoriteHobby.UserHobby)
                .Where(favoriteHobby => favoriteHobby.UserHobby.UserHobbyId == userHobbyId)
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.UserHobbies
                .Where(userHobby => userHobby.UserHobbyId == userHobbyId)
                .ExecuteDeleteAsync(cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<UserHobby?> GetUserHobbyById(int hobbyId, CancellationToken cancellationToken)
    {
        return await dbContext.UserHobbies.FirstOrDefaultAsync(userHobby =>
            userHobby.UserHobbyId == hobbyId, cancellationToken);
    }

    public async Task<UserHobby?> GetUserHobbyByName(string hobbyName, int userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserHobbies
            .AsNoTracking()
            .Include(userHobby => userHobby.User)
            .FirstOrDefaultAsync(userHobby =>
            userHobby.User.UserId == userId && userHobby.Name == hobbyName, cancellationToken);
    }

    public async Task<List<UserHobby>> GetUserHobbiesByUserId(int userId, CancellationToken cancellationToken)
    {
        return await dbContext.UserHobbies
            .AsNoTracking()
            .Where(userHobby => userHobby.User.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHobbyExists(int hobbyId, CancellationToken cancellationToken)
    {
        return await dbContext.UserHobbies
            .AsNoTracking()
            .AnyAsync(userHobby => userHobby.UserHobbyId == hobbyId, cancellationToken);
    }
}