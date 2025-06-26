using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.CreatePredefinedHobby;

public class CreatePredefinedHobbyCommandHandler(IHobbyHubRepository repository, ILogger log)
    : IRequestHandler<CreatePredefinedHobbyCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreatePredefinedHobbyCommand request, CancellationToken cancellationToken)
    {
        var hobbyWithIdenticalName =
            await repository.HobbiesRepository.GetHobbyByName(request.Name, cancellationToken);

        if (hobbyWithIdenticalName != null)
        {
            var error = HobbyErrors.HobbyWithTheSameNameAlreadyExist(
                request.Name, true, null);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var hobby = new Hobby()
        {
            Name = request.Name,
            IconType = request.IconType
        };
        
        await repository.HobbiesRepository.Add(hobby, cancellationToken);
        
        log.Information($"Created hobby with id {hobby.HobbyId}");

        return Result<CommandResponse>.Success(new CommandResponse(hobby.HobbyId));
    }
}