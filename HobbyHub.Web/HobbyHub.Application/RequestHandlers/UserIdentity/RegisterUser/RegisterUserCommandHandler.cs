using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.UserIdentity.Provider.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.UserIdentity.RegisterUser;

public class RegisterUserCommandHandler(IHobbyHubRepository repository, IPasswordHasher passwordHasher, ILogger log)
    : IRequestHandler<RegisterUserCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var foundUser = await repository.UserIdentityRepository.GetUserIdentityByEmail(request.Email, cancellationToken);

        if (foundUser != null)
        {
            var error = UserErrors.EmailAlreadyRegistered(request.Email);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var user = new User()
        {
            Email = request.Email,
            Name = request.Name,
            Surname = request.Surname,
            DateOfBirth = request.DateOfBirth ?? null
        };

        await repository.UsersRepository.Add(user, cancellationToken);

        var password = passwordHasher.Hash(request.Password);
        
        var userIdentity = new Database.Entities.UserIdentity()
        {
            Email = request.Email,
            Password = password,
            User = user
        };

        await repository.UserIdentityRepository.Add(userIdentity, cancellationToken);
        
        log.Information($"Created new user with id {user.UserId} and email {user.Email}");

        return Result<CommandResponse>.Success(new CommandResponse(user.UserId));
    }
}