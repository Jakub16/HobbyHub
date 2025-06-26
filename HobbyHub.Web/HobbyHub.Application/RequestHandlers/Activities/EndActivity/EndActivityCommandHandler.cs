using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Activities;
using HobbyHub.Database.Entities;
using HobbyHub.Repository.Abstractions;
using HobbyHub.S3.Driver.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Activities.EndActivity;

public class EndActivityCommandHandler(IHobbyHubRepository repository, ILogger log, IFileUploader fileUploader)
    : IRequestHandler<EndActivityCommand, Result<CommandResponse>>
{
    public async Task<Result<CommandResponse>> Handle(EndActivityCommand request, CancellationToken cancellationToken)
    {
        var endTime = DateTime.UtcNow;
        var pauseTime = request.PauseTime;
        
        var activity = await repository.ActivitiesRepository.GetActivityById(request.ActivityId, cancellationToken);

        if (activity == null)
        {
            var error = ActivityErrors.ActivityNotFoundCommand(request.ActivityId);
            
            log.Warning(error.Detail);
            return Result<CommandResponse>.Failure(error);
        }

        activity.EndTime = endTime;
        activity.IsDistanceAvailable = request.IsDistanceAvailable;
        activity.Distance = request.Distance;
        
        var duration = pauseTime is null 
            ? endTime.AddHours(1) - activity.StartTime 
            : endTime.AddHours(1) - activity.StartTime - pauseTime;
        activity.Time = duration;
        
         await repository.ActivitiesRepository.SaveChanges(cancellationToken);
        
        request.Pictures?.ForEach(x => AddPicture(x).GetAwaiter().GetResult());
        
        request.Notes?.ForEach(x => AddNote(x).GetAwaiter().GetResult());
        
        log.Information($"Set duration of activity with id {request.ActivityId} to {duration} and added {request.Pictures?.Count} pictures");
        
        return Result<CommandResponse>.Success(new CommandResponse(request.ActivityId));

        async Task AddPicture(IFormFile requestPicture)
        {
            var activityPicture = new ActivityPicture() { Activity = activity };

            await repository.ActivityPicturesRepository.Add(activityPicture, cancellationToken);
            await repository.ActivitiesRepository.SaveChanges(cancellationToken);
            
            var pictureUrl = await fileUploader.Send(
                "activity",
                $"activity_{activity.ActivityId.ToString()}/activityPicture_{activityPicture.ActivityPhotoId}",
                requestPicture);

            var foundActivityPicture =
                await repository.ActivityPicturesRepository.GetActivityPictureById(activityPicture.ActivityPhotoId,
                    cancellationToken);

            foundActivityPicture.PathToPicture = pictureUrl;
            await repository.ActivitiesRepository.SaveChanges(cancellationToken);
        }

        async Task AddNote(NoteRequest requestNote)
        {
            var note = new Note() { Activity = activity, Content = requestNote.Content };

            await repository.NotesRepository.Add(note, cancellationToken);
            await repository.ActivitiesRepository.SaveChanges(cancellationToken);
        }
    }
    
}