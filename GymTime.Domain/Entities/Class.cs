namespace GymTime.Domain.Entities;

public class Class
{
    public Guid Id { get; set; }
    public string ClassType { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }

    public List<ClassSession> Sessions { get; set; } = [];
    public List<Booking> Bookings { get; set; } = [];
}
