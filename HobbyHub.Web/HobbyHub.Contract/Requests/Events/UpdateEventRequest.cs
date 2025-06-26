using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Events;

public class UpdateEventRequest
{
    public int EventId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public IFormFile? MainPicture { get; set; }
    public DateTime? DateTime { get; set; }
    public string? Address { get; set; }
}