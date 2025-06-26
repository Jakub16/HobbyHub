using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.GetEventMembers;

public class GetEventMembersQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetEventMembersQuery, Result<ListResponse<UserSummaryResponse>>>
{
    public async Task<Result<ListResponse<UserSummaryResponse>>> Handle(GetEventMembersQuery request, CancellationToken cancellationToken)
    {
        var eventExists = await repository.EventsRepository.EventExists(request.EventId, cancellationToken);

        if (!eventExists)
        {
            var error = EventErrors.EventNotFound(request.EventId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<UserSummaryResponse>>.Failure(error);
        }
        
        var eventMembers = await repository.EventsRepository.GetEventMembers(request.EventId, cancellationToken);
        
        log.Information($"Found {eventMembers.Count} members for event with id {request.EventId}");
        
        return Result<ListResponse<UserSummaryResponse>>.Success(new ListResponse<UserSummaryResponse>()
        {
            Count = eventMembers.Count,
            Items = mapper.Map<List<UserSummaryResponse>>(eventMembers)
        });
    }
}