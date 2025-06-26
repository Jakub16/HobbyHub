namespace HobbyHub.Contract.Responses.Hobbies;

public record HobbyResponse
{
    public int HobbyId { get; init; }
    public required string Name { get; set; }
    public string? IconType { get; init; }
    public bool IsPredefined { get; set; }
}