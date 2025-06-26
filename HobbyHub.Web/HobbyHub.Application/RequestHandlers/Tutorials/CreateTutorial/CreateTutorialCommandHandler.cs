using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorial;

public class CreateTutorialCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateTutorialCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateTutorialCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(UserErrors.UserNotFoundCommand(request.UserId));
        }

        var tutorial = new Tutorial()
        {
            Attachments = request.Attachments,
            Category = request.Category,
            CreatedBy = request.UserId,
            Description = request.Description,
            Price = request.Price,
            Resources = request.Resources,
            TimeOfCreation = DateTime.Now,
            Title = request.Title
        };

        await repository.Add(tutorial, cancellationToken);
        await repository.SaveChanges(cancellationToken);
        
        if (request.MainPicture != null)
        {
            var pictureUrl = await fileUploader.Send(
                "tutorials/main",
                $"tutorial_{tutorial.TutorialId.ToString()}",
                request.MainPicture);

            tutorial.MainPicturePath = pictureUrl;
        }
        
        log.Information($"Created tutorial with id {tutorial.TutorialId}");
        
        await repository.SaveChanges(cancellationToken);

        return Result<CommandResponse>.Success(new CommandResponse(tutorial.TutorialId));
    }
}