namespace GymTime.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public Guid GymMemberId { get; set; }

    public Guid ClassId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public GymMember? GymMember { get; set; }

    public Class? Class { get; set; }
}
