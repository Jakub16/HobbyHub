using HobbyHub.Contract.Common;

namespace HobbyHub.Contract.Requests.Tutorials;

public record SearchTutorialsFilters
{
    public int UserId { get; set; }
    public string? Category { get; set; }
    public string? Keyword { get; set; }
    public PriceRange? PriceRange { get; set; }
    public int? CreatedBy { get; set; }
    public string? PriceSortDirection { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool OnlyForFree { get; set; } = false;
}