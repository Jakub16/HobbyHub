using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Events.CreateEvent;

public class CreateEventCommand(CreateEventRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int GroupId { get; } = request.GroupId;
    public int UserId { get; } = request.UserId;
    public string Title { get; } = request.Title;
    public string? Description { get; } = request.Description;
    public IFormFile? MainPicture { get; } = request.MainPicture;
    public DateTime? DateTime { get; } = request.DateTime;
    public string? Address { get; } = request.Address;
}