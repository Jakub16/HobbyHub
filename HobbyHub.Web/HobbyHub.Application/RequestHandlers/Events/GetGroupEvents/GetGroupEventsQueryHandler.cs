using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.GetGroupEvents;

public class GetGroupEventsQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetGroupEventsQuery, Result<ListResponse<EventResponse>>>
{
    public async Task<Result<ListResponse<EventResponse>>> Handle(GetGroupEventsQuery request, CancellationToken cancellationToken)
    {
        var groupExists = await repository.GroupsRepository.GroupExists(request.GroupId, cancellationToken);

        if (!groupExists)
        {
            var error = GroupErrors.GroupNotFound(request.GroupId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<EventResponse>>.Failure(error);
        }

        var groupEvents = await repository.EventsRepository.GetGroupEvents(request.GroupId, cancellationToken);
        var userIds = groupEvents.SelectMany(e => e.Users.Select(u => u.UserId)).Distinct().ToList();
        var users = await repository.UsersRepository.GetUsersByIds(userIds, cancellationToken);
        
        var result = groupEvents.Select(groupEvent =>
        {
            var creator = users.FirstOrDefault(u => u.UserId == groupEvent.CreatedBy);
            
            var eventResponse = new EventResponse()
            {
                DateTime = groupEvent.DateTime,
                Description = groupEvent.Description,
                EventId = groupEvent.EventId,
                GroupId = groupEvent.Group?.GroupId,
                IsPrivate = groupEvent.IsPrivate,
                Title = groupEvent.Title,
                Users = mapper.Map<List<UserSummaryResponse>>(groupEvent.Users),
                TimeOfCreation = groupEvent.TimeOfCreation,
                MainPicturePath = groupEvent.MainPicturePath,
                CreatedBy = mapper.Map<UserSummaryResponse>(creator),
                Address = groupEvent.Address
            };

            return eventResponse;
        }).ToList();
        
        log.Information($"Found {result.Count} events for group with id {request.GroupId}");
        
        return Result<ListResponse<EventResponse>>.Success(new ListResponse<EventResponse>()
        {
            Count = result.Count,
            Items = result.ToList()
        });
    }
}