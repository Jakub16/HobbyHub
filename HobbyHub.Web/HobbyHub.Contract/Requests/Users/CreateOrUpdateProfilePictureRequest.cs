using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Users;

public record CreateOrUpdateProfilePictureRequest
{
    public IFormFile Picture { get; set; }
}