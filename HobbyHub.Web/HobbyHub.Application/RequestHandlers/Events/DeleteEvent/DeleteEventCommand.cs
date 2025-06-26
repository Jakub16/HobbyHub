using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;

namespace HobbyHub.Application.RequestHandlers.Events.DeleteEvent;

public class DeleteEventCommand(int eventId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int EventId { get; } = eventId;
}