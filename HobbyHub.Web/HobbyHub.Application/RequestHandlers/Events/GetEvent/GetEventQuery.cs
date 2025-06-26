using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Responses.Events;

namespace HobbyHub.Application.RequestHandlers.Events.GetEvent;

public class GetEventQuery(int eventId) : IHobbyHubRequest<Result<EventResponse>>
{
    public int EventId { get; } = eventId;
}