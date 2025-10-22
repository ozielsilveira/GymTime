namespace GymTime.Application.Dtos.Auth;

public class AuthenticationResponse
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}