using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Tutorials;

public class CreateTutorialRequest
{
    public int UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public string[] Attachments { get; set; } = [];
    public string[] Resources { get; set; } = [];
    public double Price { get; set; } = 0;
    public IFormFile? MainPicture { get; set; }
}