using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Events;

namespace HobbyHub.Application.RequestHandlers.Events.DeleteEventMember;

public class DeleteEventMemberCommand(int eventId, int userId) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int EventId { get; } = eventId;
    public int UserId { get; } = userId;
}