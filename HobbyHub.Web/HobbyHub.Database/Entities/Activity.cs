using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class Activity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ActivityId { get; set; }
    public required string Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public TimeSpan? Time { get; set; }
    public double? Distance { get; set; }
    public bool IsDistanceAvailable { get; set; }
    public Hobby? Hobby { get; set; }
    public UserHobby? UserHobby { get; set; }
    public User User { get; set; }
    public Post? Post { get; set; }
    public List<Note> Notes { get; set; } = [];
    public List<ActivityPicture> Pictures { get; set; } = [];
}