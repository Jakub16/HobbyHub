namespace HobbyHub.Repository.Abstractions.UserIdentity;

public interface IUserIdentityRepository
{
    Task Add(Database.Entities.UserIdentity userIdentity, CancellationToken cancellationToken);
    Task<Database.Entities.UserIdentity?> GetUserIdentityByEmail(string email, CancellationToken cancellationToken);
}