using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Auth;

/// <summary>
/// Request for user authentication.
/// </summary>
public class AuthenticationRequest
{
    /// <summary>
    /// Username for authentication (required).
    /// </summary>
    [Required]
    public string Username { get; set; } = default!;

    /// <summary>
    /// Password for authentication (required).
    /// </summary>
    [Required]
    public string Password { get; set; } = default!;
}
