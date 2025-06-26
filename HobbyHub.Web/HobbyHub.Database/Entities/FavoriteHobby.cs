using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class FavoriteHobby
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int FavoriteHobbyId { get; set; }

    public required User User { get; set; }
    public Hobby? Hobby { get; set; }
    public UserHobby? UserHobby { get; set; }
}