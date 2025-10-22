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
    public string ClassType { get; init; } = default!;

    /// <summary>
    /// Scheduled date and time (UTC recommended).
    /// </summary>
    public DateTime Schedule { get; init; }

    /// <summary>
    /// Maximum participant capacity.
    /// </summary>
    public int MaxCapacity { get; init; }
}