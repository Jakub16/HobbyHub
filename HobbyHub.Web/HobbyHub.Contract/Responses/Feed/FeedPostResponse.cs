using HobbyHub.Contract.Responses.Hobbies;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Feed;

public record FeedPostResponse
{
    public int ActivityId { get; set; }
    public DateTime? DateTime { get; set; }
    public TimeSpan? Duration { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> PathsToPictures { get; set; } = [];
    public UserSummaryResponse UserSummary { get; set; }
    public HobbyResponse Hobby { get; set; }
    public double? Distance { get; set; }
    public bool IsDistanceAvailable { get; set; }
    public bool IsActivityEnded { get; set; }
}