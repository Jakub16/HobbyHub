using HobbyHub.Database.Infrastructure;
using HobbyHub.Repository.Abstractions.UserIdentity;
using Microsoft.EntityFrameworkCore;

namespace HobbyHub.Repository.Repositories.UserIdentity;

using Database.Entities;

public class UserIdentityRepository(AppDbContext dbContext) : IUserIdentityRepository
{
    public async Task Add(UserIdentity userIdentity, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(userIdentity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserIdentity?> GetUserIdentityByEmail(string email, CancellationToken cancellationToken)
    {
        return await dbContext.UserIdentities
            .Include(userIdentity => userIdentity.User)
            .FirstOrDefaultAsync(userIdentity => userIdentity.Email.Equals(email), cancellationToken);
    }
}