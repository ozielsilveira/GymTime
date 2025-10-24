using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request DTO for creating a new ClassSession.
/// </summary>
public record CreateClassSessionRequest
{
    /// <summary>
    /// Class identifier.
    /// </summary>
    [Required(ErrorMessage = "ClassId is required")]
    public Guid ClassId { get; init; }

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
