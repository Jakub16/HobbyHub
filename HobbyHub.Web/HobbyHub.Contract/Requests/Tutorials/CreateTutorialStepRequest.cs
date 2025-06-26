using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Tutorials;

public record CreateTutorialStepRequest
{
    public int TutorialId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int StepNumber { get; set; }
    public List<IFormFile>? Pictures { get; set; } = [];
}