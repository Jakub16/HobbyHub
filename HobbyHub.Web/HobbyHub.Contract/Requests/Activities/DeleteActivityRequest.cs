namespace HobbyHub.Contract.Requests.Activities;

public record DeleteActivityRequest
{
    public int ActivityId { get; set; }
}