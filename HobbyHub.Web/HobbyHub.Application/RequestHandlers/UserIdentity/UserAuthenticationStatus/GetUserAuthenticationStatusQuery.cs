using HobbyHub.Application.Infrastructure.RequestHandling;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.UserAuthenticationStatus;

public class GetUserAuthenticationStatusQuery(string token) : IHobbyHubRequest<bool>
{
    public string Token { get; } = token;
}