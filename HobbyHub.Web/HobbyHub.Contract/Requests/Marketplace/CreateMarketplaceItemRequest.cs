using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Marketplace;

public class CreateMarketplaceItemRequest
{
    public int UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public required string Type { get; set; }
    public required string Category { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public List<IFormFile>? Pictures { get; set; } = [];
}