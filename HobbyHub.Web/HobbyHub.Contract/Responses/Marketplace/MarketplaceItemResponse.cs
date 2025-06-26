using HobbyHub.Contract.Responses.Users;

namespace HobbyHub.Contract.Responses.Marketplace;

public class MarketplaceItemResponse
{
    public int MarketplaceItemId { get; set; }
    public UserSummaryResponse UserSummary { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public required string Type { get; set; }
    public required string Category { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public List<string> PathsToPictures { get; set; } = [];
    public bool IsSold { get; set; }
    public bool IsSaved { get; set; }
    public int? BoughtBy { get; set; }
}