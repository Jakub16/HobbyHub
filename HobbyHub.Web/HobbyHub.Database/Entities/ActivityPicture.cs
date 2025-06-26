using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class ActivityPicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivityPhotoId { get; set; }

    public string PathToPicture { get; set; } = "init";
    public Activity Activity { get; set; }
}