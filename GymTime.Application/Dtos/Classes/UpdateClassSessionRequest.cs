using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request DTO for updating a ClassSession.
/// </summary>
public record UpdateClassSessionRequest
{
    /// <summary>
    /// Date of the session.
    /// </summary>
    [Required(ErrorMessage = "Date is required")]
    public DateOnly Date { get; init; }

    /// <summary>
    /// Start time of the session.
    /// </summary>
    [Required(ErrorMessage = "StartTime is required")]
    public TimeOnly StartTime { get; init; }

    /// <summary>
    /// End time of the session.
    /// </summary>
    [Required(ErrorMessage = "EndTime is required")]
    public TimeOnly EndTime { get; init; }
}
