using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class Tutorial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TutorialId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int CreatedBy { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public required string Category { get; set; }
    public string? MainPicturePath { get; set; }
    public List<User> Users { get; set; } = [];
    public List<TutorialStep> TutorialSteps { get; set; } = [];
    [Column("Attachments", TypeName = "text[]")]
    public string[] Attachments { get; set; } = [];
    [Column("Resources", TypeName = "text[]")]
    public string[] Resources { get; set; } = [];
    public double Price { get; set; } = 0;
}