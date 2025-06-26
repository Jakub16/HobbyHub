namespace HobbyHub.Contract.Responses.Hobbies;

public record FavoriteHobbyResponse
{
    public int FavoriteHobbyId { get; set; }
    public int HobbyId { get; init; }
    public required string Name { get; set; }
    public string? IconType { get; init; }
    public bool IsPredefined { get; set; }
}