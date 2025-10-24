namespace GymTime.Domain.Entities;

public class ClassSession
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }

    /// <summary>
    /// Session date (without time)
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Session start time
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Session end time
    /// </summary>
    public TimeOnly EndTime { get; set; }

    /// <summary>
    /// Complete start date and time (UTC) - calculated or stored
    /// </summary>
    public DateTime Schedule { get; set; }

    public Class? Class { get; set; }
    public List<Booking> Bookings { get; set; } = [];

    public bool HasAvailableSlots()
    {
        return Class != null && Bookings.Count < Class.MaxCapacity;
    }

    /// <summary>
    /// Calculates the session duration in minutes
    /// </summary>
    public int GetDurationInMinutes()
    {
        return (int)(EndTime.ToTimeSpan() - StartTime.ToTimeSpan()).TotalMinutes;
    }

    /// <summary>
    /// Checks if the session is scheduled for the future
    /// </summary>
    public bool IsFutureSession()
    {
        return Schedule > DateTime.UtcNow;
    }
}
