using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Groups;

public record CreateGroupPostRequest
{
    public int GroupId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<IFormFile> Pictures { get; set; } = [];
}