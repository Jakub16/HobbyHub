using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Responses.Hobbies;

namespace HobbyHub.Application.RequestHandlers.Hobbies.PredefinedHobbies.GetPredefinedHobbies;

public class GetPredefinedHobbiesQuery : IHobbyHubRequest<Result<ListResponse<HobbyResponse>>>
{
    
}