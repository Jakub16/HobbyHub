using HobbyHub.Contract.Common;

namespace HobbyHub.Contract.Requests.Marketplace;

public record SearchMarketplaceItemsFilters
{
    public int UserId { get; set; }
    public string? Type { get; set; }
    public string? Category { get; set; }
    public string? Keyword { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public PriceRange? PriceRange { get; set; }
    public string? PriceSortDirection { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}