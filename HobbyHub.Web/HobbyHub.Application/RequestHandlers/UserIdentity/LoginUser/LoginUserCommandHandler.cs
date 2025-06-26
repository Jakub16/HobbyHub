using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Repository.Abstractions;
using HobbyHub.UserIdentity.Provider.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.LoginUser;

public class LoginUserCommandHandler(IHobbyHubRepository repository,
    ILogger log,
    IJwtService jwtService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var userIdentity = await repository.UserIdentityRepository
            .GetUserIdentityByEmail(request.Email, cancellationToken);

        if (userIdentity == null)
        {
            log.Warning($"Authentication of user with email {request.Email} failed");
            return Result<string>.Failure(UserErrors.InvalidEmailOrPassword);
        }

        var isPasswordValid = passwordHasher.Verify(request.Password, userIdentity.Password);

        if (!isPasswordValid)
        {
            log.Warning($"Authentication of user with email {request.Email} failed");
            return Result<string>.Failure(UserErrors.InvalidEmailOrPassword);
        }

        var token = jwtService.Generate(
            userIdentity.UserId,
            userIdentity.Email,
            userIdentity.User.Name,
            userIdentity.User.Surname,
            "user");
        
        log.Information($"User with email {request.Email} logged in successfully");

        return Result<string>.Success(token);
    }
}