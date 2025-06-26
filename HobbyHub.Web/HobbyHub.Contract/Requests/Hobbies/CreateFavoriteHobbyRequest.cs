namespace HobbyHub.Contract.Requests.Hobbies;

public record CreateFavoriteHobbyRequest
{
    public int UserId { get; init; }
    public int HobbyId { get; init; }
    public bool IsHobbyPredefined { get; set; }
}