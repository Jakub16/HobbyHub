using HobbyHub.Contract.Requests.Events;
using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Events;

public record EventResponse
{
    public int EventId { get; set; }
    public int? GroupId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? MainPicturePath { get; set; }
    public DateTime? DateTime { get; set; }
    public string? Address { get; set; }
    public List<UserSummaryResponse> Users { get; set; } = [];
    public bool IsPrivate { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public UserSummaryResponse CreatedBy { get; set; }
}