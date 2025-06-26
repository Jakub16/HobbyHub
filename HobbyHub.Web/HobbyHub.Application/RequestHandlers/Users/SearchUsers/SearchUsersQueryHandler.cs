using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Users.SearchUsers;

public class SearchUsersQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<SearchUsersQuery, Result<ListResponse<UserSummaryResponse>>>
{
    public async Task<Result<ListResponse<UserSummaryResponse>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.UsersRepository.UsersSearch(request.Keyword, cancellationToken);
        
        log.Information($"Found {result.Count} users");
        
        return Result<ListResponse<UserSummaryResponse>>.Success(new ListResponse<UserSummaryResponse>()
        {
            Count = result.Count,
            Items = mapper.Map<List<UserSummaryResponse>>(result)
        });
    }
}