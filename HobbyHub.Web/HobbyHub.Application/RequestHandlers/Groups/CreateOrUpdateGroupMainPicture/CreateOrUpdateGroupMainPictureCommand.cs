using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateOrUpdateGroupMainPicture;

public class CreateOrUpdateGroupMainPictureCommand(CreateOrUpdateGroupMainPictureRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int GroupId { get; } = request.GroupId;
    public IFormFile Picture { get; } = request.Picture;
}