using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class TutorialStepPicture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TutorialStepPictureId { get; set; }

    public string PathToPicture { get; set; } = "init";
    public TutorialStep TutorialStep { get; set; }
}