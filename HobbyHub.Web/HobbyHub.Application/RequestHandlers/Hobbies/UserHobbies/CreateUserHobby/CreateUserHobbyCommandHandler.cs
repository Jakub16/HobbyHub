using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.UserHobbies.CreateUserHobby;

public class CreateUserHobbyCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<CreateUserHobbyCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateUserHobbyCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var userHobbyWithIdenticalName =
            await repository.UserHobbiesRepository.GetUserHobbyByName(request.Name, request.UserId, cancellationToken);

        if (userHobbyWithIdenticalName != null)
        {
            var error = HobbyErrors.HobbyWithTheSameNameAlreadyExist(
                request.Name, false, request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        var predefinedHobbyWithIdenticalName =
            await repository.HobbiesRepository.GetHobbyByName(request.Name, cancellationToken);

        if (predefinedHobbyWithIdenticalName != null)
        {
            var error = HobbyErrors.HobbyWithTheSameNameAlreadyExist(
                request.Name, true, request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var hobby = new UserHobby()
        {
            Name = request.Name,
            IconType = request.IconType,
            User = user
        };

        await repository.UserHobbiesRepository.Add(hobby, cancellationToken);
        
        log.Information($"Created user hobby with id {hobby.UserHobbyId} for user with id {request.UserId}");

        return Result<CommandResponse>.Success(new CommandResponse(hobby.UserHobbyId));
    }
}