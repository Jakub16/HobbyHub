using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.UserIdentity;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.RegisterUser;

public class RegisterUserCommand(RegisterUserRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public string Email { get; init; } = request.Email;
    public string Password { get; init; } = request.Password;
    public string Name { get; init; } = request.Name;
    public string Surname { get; init; } = request.Surname;
    public DateOnly? DateOfBirth { get; init; } = request.DateOfBirth;
}