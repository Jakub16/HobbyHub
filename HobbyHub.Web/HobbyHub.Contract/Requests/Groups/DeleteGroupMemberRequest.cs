namespace HobbyHub.Contract.Requests.Groups;

public class DeleteGroupMemberRequest
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
}