using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class UserHobby
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserHobbyId { get; set; }
    public required string Name { get; set; }
    public string? IconType { get; set; }
    public User User { get; set; }
    public List<Activity> Activities { get; set; } = [];
}