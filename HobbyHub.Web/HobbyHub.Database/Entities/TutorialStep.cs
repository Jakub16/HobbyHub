using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class TutorialStep
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TutorialStepId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public int StepNumber { get; set; }
    public List<TutorialStepPicture> TutorialStepPictures { get; set; } = [];
    public required Tutorial Tutorial { get; set; }
}