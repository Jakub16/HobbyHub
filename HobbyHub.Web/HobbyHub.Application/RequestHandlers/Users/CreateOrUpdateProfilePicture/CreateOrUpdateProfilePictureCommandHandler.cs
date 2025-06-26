using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.CreateOrUpdateProfilePicture;

public class CreateOrUpdateProfilePictureCommandHandler(IHobbyHubRepository repository, IFileUploader fileUploader, ILogger log)
    : IRequestHandler<CreateOrUpdateProfilePictureCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(CreateOrUpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.UsersRepository.GetUserByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            var error = UserErrors.UserNotFoundCommand(request.UserId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }
        
        var pictureUrl = await fileUploader.Send(
            "users/main",
            $"user_{user.UserId.ToString()}",
            request.Picture);
        
        user.ProfilePicturePath = pictureUrl;
        await repository.SaveChanges(cancellationToken);
        
        log.Information($"Created or updated profile picture for user with id {user.UserId}");
        
        return Result<CommandResponse>.Success(new CommandResponse(user.UserId));
    }
}