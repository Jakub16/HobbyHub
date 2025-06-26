using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Groups;

public record CreateGroupRequest
{
    public required int UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public IFormFile? MainPicture { get; set; }
}