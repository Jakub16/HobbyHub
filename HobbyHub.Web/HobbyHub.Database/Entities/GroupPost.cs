namespace HobbyHub.Database.Entities;

public class GroupPost
{
    public int GroupPostId { get; set; }
    public string? Content { get; set; }
    public DateTime DateTime { get; set; }
    public required User User { get; set; }
    public required Group Group { get; set; }
    public List<GroupPostLike> GroupPostLikes { get; set; } = [];
    public List<GroupPostComment> GroupPostComments { get; set; } = [];
    public List<GroupPostPicture> GroupPostPictures { get; set; } = [];
}