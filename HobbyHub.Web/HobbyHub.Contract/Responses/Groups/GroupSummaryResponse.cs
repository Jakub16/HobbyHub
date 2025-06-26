using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Groups;

public record GroupSummaryResponse
{
    public int GroupId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? MainPicturePath { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public UserSummaryResponse? CreatedBy { get; set; }
}