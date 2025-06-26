using HobbyHub.Contract.Responses.Hobbies;

namespace HobbyHub.Contract.Responses.Activities;

public record ActivityResponse
{
    public int ActivityId { get; init; }
    public string ActivityName { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public TimeSpan? Time { get; init; }
    public double? Distance { get; init; }
    public bool IsDistanceAvailable { get; init; }
    public HobbyResponse Hobby { get; init; }
}