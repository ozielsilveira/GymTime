using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request to update an existing class.
/// </summary>
public class UpdateClassRequest
{
    /// <summary>
    /// Class type/name (required, maximum 100 characters).
    /// </summary>
    /// <remarks>
    /// Business rules: type change must consider booking history; avoid renaming classes with existing bookings without additional validation.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Maximum capacity (required, minimum 1).
    /// </summary>
    /// <remarks>
    /// Business rules: reducing capacity below the current number of bookings is not allowed without prior cancellations.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}
