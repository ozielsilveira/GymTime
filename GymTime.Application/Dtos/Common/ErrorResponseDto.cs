namespace GymTime.Application.Dtos.Common;

/// <summary>
/// Standard error response containing a message for clients.
/// </summary>
public class ErrorResponseDto
{
    /// <summary>
    /// Human readable error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
