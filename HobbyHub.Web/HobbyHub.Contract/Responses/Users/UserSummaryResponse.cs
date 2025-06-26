namespace HobbyHub.Contract.Responses.Users;

public record UserSummaryResponse
{
    public int UserId { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public string? ProfilePicturePath { get; set; }
}