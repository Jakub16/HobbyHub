using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class Group
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int GroupId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? MainPicturePath { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public int CreatedBy { get; set; }
    public List<User> Users { get; set; } = [];
    public List<Event> Events { get; set; } = [];
    public List<GroupPost> GroupPosts { get; set; } = [];
}