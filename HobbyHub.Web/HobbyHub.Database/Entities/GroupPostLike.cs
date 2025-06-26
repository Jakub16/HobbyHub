namespace HobbyHub.Database.Entities;

public class GroupPostLike
{
    public int GroupPostLikeId { get; set; }
    public required GroupPost GroupPost { get; set; }
    public required User User { get; set; }
}