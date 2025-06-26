using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class Hobby
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int HobbyId { get; set; }
    public required string Name { get; set; }
    public string? IconType { get; set; }
    public List<Activity> Activities { get; set; } = [];
}