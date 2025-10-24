namespace GymTime.Application.Dtos.Bookings;

/// <summary>
/// Response DTO for a booking.
/// </summary>
public record BookingDto
{
    /// <summary>
    /// Booking identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gym member identifier.
    /// </summary>
    public Guid GymMemberId { get; init; }

    /// <summary>
    /// Class identifier.
    /// </summary>
    public Guid ClassId { get; init; }

    /// <summary>
    /// Class session identifier.
    /// </summary>
    public Guid ClassSessionId { get; init; }

    /// <summary>
    /// Booking creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Optional gym member name (when available).
    /// </summary>
    public string? GymMemberName { get; init; }

    /// <summary>
    /// Optional class type/name (when available).
    /// </summary>
    public string? ClassType { get; init; }

    /// <summary>
    /// Optional session schedule (when available).
    /// </summary>
    public DateTime? SessionSchedule { get; init; }
}
