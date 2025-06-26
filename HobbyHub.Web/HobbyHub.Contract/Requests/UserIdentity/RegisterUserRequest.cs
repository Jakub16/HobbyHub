using System.ComponentModel.DataAnnotations;

namespace HobbyHub.Contract.Requests.UserIdentity;

public record RegisterUserRequest
{
    [Required]
    public string Email { get; init; }
    [Required]
    public string Password { get; init; }
    public string Name { get; init; }
    public string Surname { get; init; }
    public DateOnly? DateOfBirth { get; init; }
}