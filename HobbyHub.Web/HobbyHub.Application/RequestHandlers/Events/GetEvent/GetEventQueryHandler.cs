using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Responses.Events;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Events.GetEvent;

public class GetEventQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetEventQuery, Result<EventResponse>>
{
    public async Task<Result<EventResponse>> Handle(GetEventQuery request, CancellationToken cancellationToken)
    {
        var _event = await repository.EventsRepository.GetEventById(request.EventId, cancellationToken);

        if (_event == null)
        {
            var error = EventErrors.EventNotFound(request.EventId);
            
            log.Warning(error.Detail);
            return Result<EventResponse>.Failure(error);
        }
        
        var creator = await repository.UsersRepository.GetUserByIdAsync(_event.CreatedBy, cancellationToken);
            
        var eventResponse = new EventResponse()
        {
            DateTime = _event.DateTime,
            Description = _event.Description,
            EventId = _event.EventId,
            GroupId = _event.Group?.GroupId,
            IsPrivate = _event.IsPrivate,
            Title = _event.Title,
            Users = mapper.Map<List<UserSummaryResponse>>(_event.Users),
            TimeOfCreation = _event.TimeOfCreation,
            MainPicturePath = _event.MainPicturePath,
            CreatedBy = mapper.Map<UserSummaryResponse>(creator),
            Address = _event.Address
        };
        
        log.Information($"Found event with id {_event.EventId}");
        
        return Result<EventResponse>.Success(eventResponse);
    }
}