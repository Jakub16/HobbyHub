namespace HobbyHub.Contract.Requests.Activities;

public record NoteResponse
{
    public int NoteId { get; init; }
    public string Content { get; init; }
}