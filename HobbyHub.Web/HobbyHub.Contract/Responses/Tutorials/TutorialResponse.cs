using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Tutorials;

public record TutorialResponse
{
    public int TutorialId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; }
    public string[] Attachments { get; set; }
    public string[] Resources { get; set; }
    public double Price { get; set; } = 0;
    public string? MainPicturePath { get; set; }
    public UserSummaryResponse CreatedBy { get; set; }
    public List<TutorialStepResponse> TutorialSteps { get; set; }
}