using AutoMapper;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Application.Infrastructure.ResultHandling.Errors;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Groups;
using HobbyHub.Contract.Responses.Users;
using HobbyHub.Repository.Abstractions;
using MediatR;
using Serilog;

namespace HobbyHub.Application.RequestHandlers.Groups.GetSuggestedGroups;

public class GetSuggestedGroupsQueryHandler(IHobbyHubRepository repository, IMapper mapper, ILogger log)
    : IRequestHandler<GetSuggestedGroupsQuery, Result<ListResponse<GroupSummaryResponse>>>
{
    public async Task<Result<ListResponse<GroupSummaryResponse>>> Handle(GetSuggestedGroupsQuery request, CancellationToken cancellationToken)
    {
        var userExists = await repository.UsersRepository.UserExists(request.UserId, cancellationToken);

        if (!userExists)
        {
            var error = UserErrors.UserNotFound(request.UserId);
            log.Warning(error.Detail);
            return Result<ListResponse<GroupSummaryResponse>>.Failure(error);
        }
        
        var userFavoriteHobbies = await repository.FavoriteHobbiesRepository
            .GetUserFavoriteHobbies(request.UserId, cancellationToken);
        
        var keywords = userFavoriteHobbies
            .Select(hobby => hobby.Hobby != null ? hobby.Hobby.Name : hobby.UserHobby.Name)
            .ToList();
        
        var suggestedGroups = await repository.GroupsRepository.GetSuggestedGroups(request.UserId, keywords, cancellationToken);
        
        var creatorsIds = suggestedGroups.Select(group => group.CreatedBy).ToList();
        var creators = await repository.UsersRepository.GetUsersByIds(creatorsIds, cancellationToken);
        
        var result = suggestedGroups.Select(group =>
        {
            var creator = creators.FirstOrDefault(creator => creator.UserId == group.CreatedBy);

            return new GroupSummaryResponse()
            {
                CreatedBy = mapper.Map<UserSummaryResponse>(creator),
                Description = group.Description,
                GroupId = group.GroupId,
                IsPrivate = group.IsPrivate,
                MainPicturePath = group.MainPicturePath,
                Name = group.Name,
                TimeOfCreation = group.TimeOfCreation
            };
        }).ToList();
        
        log.Information($"Found {result.Count} suggested groups for user with id {request.UserId}");
        
        return Result<ListResponse<GroupSummaryResponse>>.Success(new ListResponse<GroupSummaryResponse>()
        {
            Count = result.Count,
            Items = result
        });
    }
}