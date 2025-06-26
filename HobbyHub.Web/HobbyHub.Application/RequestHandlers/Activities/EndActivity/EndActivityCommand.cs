using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Activities;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Activities.EndActivity;

public class EndActivityCommand(EndActivityRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int ActivityId { get; } = request.ActivityId;
    public TimeSpan? PauseTime { get; } = request.PauseTime;
    public List<NoteRequest>? Notes { get; } = request.Notes;
    public List<IFormFile>? Pictures = request.Pictures;
    public bool IsDistanceAvailable { get; } = request.IsDistanceAvailable;
    public double? Distance { get; } = request.Distance;
}