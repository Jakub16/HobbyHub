using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace HobbyHub.Contract.Requests.Activities;

public record EndActivityRequest
{
    public int ActivityId { get; set; }
    public TimeSpan? PauseTime { get; set; }
    public List<IFormFile>? Pictures { get; set; }
    public List<NoteRequest>? Notes { get; set; }
    public double? Distance { get; set; }
    [DefaultValue(false)]
    public bool IsDistanceAvailable { get; set; }
}