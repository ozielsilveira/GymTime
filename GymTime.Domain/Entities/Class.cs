namespace GymTime.Domain.Entities;

public class Class
{
    public Guid Id { get; set; }
    public string ClassType { get; set; } = string.Empty;
    public DateTime Schedule { get; set; }
    public int MaxCapacity { get; set; }

    public List<Booking> Bookings { get; set; } = new();

    public bool HasAvailableSlots()
    {
        return Bookings.Count < MaxCapacity;
    }
}
