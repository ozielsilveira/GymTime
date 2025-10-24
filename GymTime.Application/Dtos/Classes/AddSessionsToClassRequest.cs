using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request to add sessions to an existing class.
/// </summary>
public class AddSessionsToClassRequest
{
    /// <summary>
    /// Start date for session generation.
    /// </summary>
    [Required(ErrorMessage = "StartDate is required")]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// End date for session generation.
    /// </summary>
    [Required(ErrorMessage = "EndDate is required")]
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Session start time.
    /// </summary>
    [Required(ErrorMessage = "StartTime is required")]
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Session end time.
    /// </summary>
    [Required(ErrorMessage = "EndTime is required")]
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Days of the week on which sessions should be created.
    /// </summary>
    /// <remarks>
    /// Array with the days of the week (0 = Sunday, 1 = Monday, ..., 6 = Saturday).
    /// Example: [1, 3, 5] for Monday, Wednesday and Friday.
    /// </remarks>
    [Required(ErrorMessage = "DaysOfWeek is required")]
    [MinLength(1, ErrorMessage = "At least one day of week is required")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = [];
}
