using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

[Table("Follows")]
public class Follow
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FollowId { get; set; }
    public int FollowerId { get; set; }
    public User Follower { get; set; }
    public int FollowedId { get; set; }
    public User Followed { get; set; }
}