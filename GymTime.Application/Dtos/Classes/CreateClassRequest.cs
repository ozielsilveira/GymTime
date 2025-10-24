using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request to create a new class.
/// </summary>
public class CreateClassRequest
{
    /// <summary>
    /// Class type/name (required, maximum 100 characters).
    /// </summary>
    /// <remarks>
    /// Business rules: unique value per schedule in the domain (do not duplicate classes of the same type at the same time) - this validation is done in the service/repository.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Maximum capacity (required, minimum 1).
    /// </summary>
    /// <remarks>
    /// Business rules: greater than 0. Upper limits must be enforced according to gym policy.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }

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
