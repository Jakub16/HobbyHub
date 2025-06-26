using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class MarketplaceItemPicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MarketplaceItemPictureId { get; set; }
    public string PathToPicture { get; set; } = "init";
    public MarketplaceItem MarketplaceItem { get; set; }
}