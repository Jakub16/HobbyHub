using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Groups;

public record GroupPostResponse
{
    public int GroupPostId { get; set; }
    public DateTime DateTime { get; set; }
    public UserSummaryResponse UserSummary { get; set; }
    public string? Content { get; set; }
    public List<string> PathsToPictures { get; set; } = [];
}