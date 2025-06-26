namespace HobbyHub.Contract.Requests.Activities;

public record StartActivityRequest
{
    public int UserId { get; set; }
    public required int HobbyId { get; set; }
    public bool IsHobbyPredefined { get; set; }
}