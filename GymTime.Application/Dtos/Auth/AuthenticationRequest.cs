using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Auth;

public class AuthenticationRequest
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
