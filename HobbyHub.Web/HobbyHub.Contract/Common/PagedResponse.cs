namespace HobbyHub.Contract.Common;

public record PagedResponse<T>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalRecords { get; init; }
    public required T Data { get; init; }
}