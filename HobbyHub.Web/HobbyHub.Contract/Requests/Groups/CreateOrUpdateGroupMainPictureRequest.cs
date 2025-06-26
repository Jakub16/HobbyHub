using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Groups;

public record CreateOrUpdateGroupMainPictureRequest
{
    public int GroupId { get; set; }
    public required IFormFile Picture { get; set; }
}