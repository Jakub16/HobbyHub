using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;
[Table("Notifications")]
public class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NotificationId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public string Link { get; set; }
    public User User { get; set; }
    public bool IsGroupInvitation { get; set; }
    public int GroupId { get; set; }
}