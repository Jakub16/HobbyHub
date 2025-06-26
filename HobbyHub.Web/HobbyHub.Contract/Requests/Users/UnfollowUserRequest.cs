namespace HobbyHub.Contract.Requests.Users;

public record UnfollowUserRequest
{
    public int UserId { get; set; }
    public int UserToUnfollowId { get; set; }
}