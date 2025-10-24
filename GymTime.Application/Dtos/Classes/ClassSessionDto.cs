namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Response DTO for ClassSession.
/// </summary>
public record ClassSessionDto
{
    /// <summary>
    /// Session identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Class identifier.
    /// </summary>
    public Guid ClassId { get; init; }

    /// <summary>
    /// Date of the session.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Start time of the session.
    /// </summary>
    public TimeOnly StartTime { get; init; }

    /// <summary>
    /// End time of the session.
    /// </summary>
    public TimeOnly EndTime { get; init; }

    /// <summary>
    /// Scheduled date and time (UTC recommended).
    /// </summary>
    /// <remarks>
    /// Date and time are stored in UTC. Client should convert to the user's timezone for display.
    /// </remarks>
    public DateTime Schedule { get; init; }

    /// <summary>
    /// Class type/name (optional, for convenience).
    /// </summary>
    public string? ClassType { get; init; }

    /// <summary>
    /// Current number of bookings for this session.
    /// </summary>
    public int CurrentBookings { get; init; }

    /// <summary>
    /// Maximum capacity (from parent Class).
    /// </summary>
    public int MaxCapacity { get; init; }

    /// <summary>
    /// Duration of the session in minutes.
    /// </summary>
    public int DurationInMinutes { get; init; }
}
