using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;
[Table("SavedMarketplaceItems")]
public class SavedMarketplaceItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SavedMarketplaceItemId { get; set; }
    public User User { get; set; }
    public MarketplaceItem MarketplaceItem { get; set; }
}