using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateGroup;

public class CreateGroupCommand(CreateGroupRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public string Name { get; } = request.Name;
    public string? Description { get; } = request.Description;
    public bool IsPrivate { get; } = request.IsPrivate;
    public IFormFile? MainPicture { get; } = request.MainPicture;
}