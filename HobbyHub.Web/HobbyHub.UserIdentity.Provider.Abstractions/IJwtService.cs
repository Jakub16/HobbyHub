namespace HobbyHub.UserIdentity.Provider.Abstractions;

public interface IJwtService
{
    string Generate(int userId, string email, string name, string surname, string role);
    bool Verify(string jwt);
}