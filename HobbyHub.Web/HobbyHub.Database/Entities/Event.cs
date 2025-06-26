using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class Event
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EventId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? MainPicturePath { get; set; }
    public DateTime? DateTime { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public int CreatedBy { get; set; }
    public Group? Group { get; set; }
    public string? Address { get; set; }
    public List<User> Users { get; set; } = [];
    public bool IsPrivate { get; set; }
}