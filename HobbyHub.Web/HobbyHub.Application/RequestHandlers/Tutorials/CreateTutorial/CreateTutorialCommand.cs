using HobbyHub.Application.Infrastructure.RequestHandling;
using HobbyHub.Application.Infrastructure.ResultHandling;
using HobbyHub.Contract.Common;
using HobbyHub.Contract.Requests.Tutorials;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Application.RequestHandlers.Tutorials.CreateTutorial;

public class CreateTutorialCommand(CreateTutorialRequest request): IHobbyHubRequest<Result<CommandResponse>>
{
    public int UserId { get; } = request.UserId;
    public string Title { get; } = request.Title;
    public string? Description { get; } = request.Description;
    public string Category { get; } = request.Category;
    public string[] Attachments { get; } = request.Attachments;
    public string[] Resources { get; } = request.Resources;
    public double Price { get; } = request.Price;
    public IFormFile? MainPicture { get; } = request.MainPicture;
}