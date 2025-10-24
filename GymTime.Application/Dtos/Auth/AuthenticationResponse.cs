namespace GymTime.Application.Dtos.Auth;

/// <summary>
/// Response containing authentication token and expiration information.
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// JWT authentication token.
    /// </summary>
    public string Token { get; set; } = default!;
    
    /// <summary>
    /// Token expiration date and time (UTC).
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
