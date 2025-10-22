namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Response DTO for Class.
/// </summary>
public record ClassDto
{
 /// <summary>
 /// Class identifier.
 /// </summary>
 public Guid Id { get; init; }

 /// <summary>
 /// Class type/name.
 /// </summary>
 /// <remarks>
 /// Displays the class name/type, for example: Yoga, Pilates, Crossfit.
 /// </remarks>
 public string ClassType { get; init; } = default!;

 /// <summary>
 /// Scheduled date and time (UTC recommended).
 /// </summary>
 /// <remarks>
 /// Date and time are stored in UTC. Client should convert to the user's timezone for display.
 /// </remarks>
 public DateTime Schedule { get; init; }

 /// <summary>
 /// Maximum participant capacity.
 /// </summary>
 /// <remarks>
 /// Maximum number of participants allowed in this class.
 /// </remarks>
 public int MaxCapacity { get; init; }
}