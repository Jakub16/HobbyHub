namespace HobbyHub.Contract.Requests.Activities;

public record NoteRequest
{
    public string Content { get; set; } = string.Empty;
}