using GymTime.Domain.Enums;

namespace GymTime.Domain.Entities;

public class GymMember
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public PlanType PlanType { get; set; }

    public List<Booking> Bookings { get; set; } = [];

    public int GetMonthlyBookingLimit()
    {
        return PlanType switch
        {
            PlanType.Monthly => 12,
            PlanType.Quarterly => 20,
            PlanType.Annual => 30,
            _ => 0
        };
    }

    public bool CanBook(int currentMonthBookings)
    {
        return currentMonthBookings < GetMonthlyBookingLimit();
    }
}

