using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Groups;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Groups.CreateGroupPost;

public class CreateGroupPostCommand(CreateGroupPostRequest request) : IHobbyHubRequest<Result<CommandResponse>>
{
    public int GroupId { get; } = request.GroupId;
    public int UserId { get; } = request.UserId;
    public string? Content { get; } = request.Content;
    public List<IFormFile>? Pictures { get; } = request.Pictures;
}