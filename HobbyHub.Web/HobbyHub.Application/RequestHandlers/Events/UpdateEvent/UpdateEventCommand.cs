using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Events.UpdateEvent;

public class UpdateEventCommand(UpdateEventRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int EventId { get; } = request.EventId;
    public string Title { get; } = request.Title;
    public string? Description { get; } = request.Description;
    public IFormFile? MainPicture { get; } = request.MainPicture;
    public DateTime? DateTime { get; } = request.DateTime;
    public string? Address { get; } = request.Address;
}