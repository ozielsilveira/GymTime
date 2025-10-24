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
    /// Maximum participant capacity.
    /// </summary>
    /// <remarks>
    /// Maximum number of participants allowed in this class.
    /// </remarks>
    public int MaxCapacity { get; init; }

    /// <summary>
    /// Sessions for this class.
    /// </summary>
    public List<ClassSessionDto> Sessions { get; init; } = new();
}