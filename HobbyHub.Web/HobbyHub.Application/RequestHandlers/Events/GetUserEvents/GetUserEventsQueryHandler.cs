using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.GetUserEvents;

public class GetUserEventsQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetUserEventsQuery, Result<ListResponse<EventResponse>>>
{
    public async Task<Result<ListResponse<EventResponse>>> Handle(GetUserEventsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            
            log.Warning(error.Detail);
            return Result<ListResponse<EventResponse>>.Failure(error);
        }
        
        var userEvents = await repository.EventsRepository.GetUserEvents(request.UserId, cancellationToken);
        var userIds = userEvents.SelectMany(e => e.Users.Select(u => u.UserId)).Distinct().ToList();
        var users = await repository.UsersRepository.GetUsersByIds(userIds, cancellationToken);
        
        var result = userEvents.Select(groupEvent =>
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
                MainPicturePath = groupEvent.MainPicturePath,
                TimeOfCreation = groupEvent.TimeOfCreation,
                CreatedBy = mapper.Map<UserSummaryResponse>(creator),
                Address = groupEvent.Address
            };

            return eventResponse;
        }).ToList();
        
        log.Information($"Found {result.Count} events for user with id {request.UserId}");
        
        return Result<ListResponse<EventResponse>>.Success(new ListResponse<EventResponse>()
        {
            Count = result.Count,
            Items = result.ToList()
        });
    }
}