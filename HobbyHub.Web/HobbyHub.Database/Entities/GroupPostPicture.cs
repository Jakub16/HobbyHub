using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class GroupPostPicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GroupPostPictureId { get; set; }
    public string PathToPicture { get; set; } = "init";
    public GroupPost GroupPost { get; set; }
}