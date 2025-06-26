namespace HobbyHub.Database.Entities;

public class GroupPostComment
{
    public int GroupPostCommentId { get; set; }
    public string? Content { get; set; }
    public string? PathToPicture { get; set; }
    public DateTime DateTime { get; set; }
    public required GroupPost GroupPost { get; set; }
    public required User User { get; set; }
}