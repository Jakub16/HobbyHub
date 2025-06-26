using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HobbyHub.Database.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? ProfilePicturePath { get; set; }
    public UserIdentity UserIdentity { get; set; }
    public List<Group> Groups { get; set; } = [];
    public List<Event> Events { get; set; } = [];
    public List<Post> Posts { get; set; } = [];
    public List<UserHobby> UserHobbies { get; set; } = [];
    public List<FavoriteHobby> FavoriteHobbies { get; set; } = [];
    public List<Follow> Followers { get; set; } = [];
    public List<Follow> Following { get; set; } = [];
    public List<Activity> Activities { get; set; } = [];
    public List<GroupPostLike> GroupPostLikes { get; set; } = [];
    public List<GroupPostComment> GroupPostComments { get; set; } = [];
    public List<MarketplaceItem> MarketplaceItems { get; set; } = [];
    public List<Tutorial> Tutorials { get; set; } = [];
    public List<SavedMarketplaceItem> SavedMarketplaceItems { get; set; } = [];
    public List<Notification> Notifications { get; set; }
}