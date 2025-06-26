namespace HobbyHub.Contract.Requests.Hobbies;

public record CreateUserHobbyRequest
{
    public required int UserId { get; init; }
    public required string Name { get; init; }
    public string? IconType { get; init; }
}