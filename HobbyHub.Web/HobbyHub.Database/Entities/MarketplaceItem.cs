using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class MarketplaceItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MarketplaceItemId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public required string Type { get; set; }
    public required string Category { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public User User { get; set; }
    public List<MarketplaceItemPicture> MarketplaceItemPictures { get; set; } = [];
    public bool IsSold { get; set; }
    public int? BoughtBy { get; set; }
}