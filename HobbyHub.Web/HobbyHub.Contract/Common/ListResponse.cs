namespace HobbyHub.Contract.Common;

public class ListResponse<T>
{
    public List<T> Items { get; set; }
    public int Count { get; set; }
}