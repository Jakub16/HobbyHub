namespace HobbyHub.Contract.Requests.Hobbies;

public record CreateHobbyRequest
{
    public required string Name { get; init; }
    public string? IconType { get; init; }
}