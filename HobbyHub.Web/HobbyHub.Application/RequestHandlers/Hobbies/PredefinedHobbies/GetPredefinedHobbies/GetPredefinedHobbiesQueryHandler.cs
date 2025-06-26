using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;

public class GetPredefinedHobbiesQueryHandler(IHobbyHubRepository repository, ILogger log, IMapper mapper)
    : IRequestHandler<GetPredefinedHobbiesQuery, Result<ListResponse<HobbyResponse>>>
{
    public async Task<Result<ListResponse<HobbyResponse>>> Handle(GetPredefinedHobbiesQuery request, CancellationToken cancellationToken)
    {
        var hobbies = await repository.HobbiesRepository.GetAllHobbies(cancellationToken);
        
        var count = hobbies.Count;
        
        var items = mapper.Map<List<HobbyResponse>>(hobbies);
        items.ForEach(item => item.IsPredefined = true);
        
        log.Information($"Found {count} predefined hobbies");
        
        return Result<ListResponse<HobbyResponse>>.Success(new ListResponse<HobbyResponse>()
        {
            Items = items,
            Count = count
        });
    }
}