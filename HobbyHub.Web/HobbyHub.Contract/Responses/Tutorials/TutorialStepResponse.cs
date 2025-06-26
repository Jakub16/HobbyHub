namespace HobbyHub.Contract.Responses.Tutorials;

public class TutorialStepResponse
{
    public int TutorialStepId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int StepNumber { get; set; }
    public List<string> PathsToPictures { get; set; }
}