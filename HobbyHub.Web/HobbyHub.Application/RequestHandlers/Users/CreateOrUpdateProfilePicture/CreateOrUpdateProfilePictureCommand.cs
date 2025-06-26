using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Users;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Users.CreateOrUpdateProfilePicture;

public class CreateOrUpdateProfilePictureCommand(int userId, CreateOrUpdateProfilePictureRequest request) 
    : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = userId;
    public IFormFile Picture { get; } = request.Picture;
}