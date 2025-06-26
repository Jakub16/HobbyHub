using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Requests.UserIdentity;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.LoginUser;

public class LoginUserCommand(LoginUserRequest request) : IHobbyHubRequest<Result<string>>
{
    public string Email { get; } = request.Email;
    public string Password { get; } = request.Password;
}