namespace HobbyHub.Contract.Requests.Users;

public record FollowUserRequest
{
    public int UserId { get; set; }
    public int UserToFollowId { get; set; }
}