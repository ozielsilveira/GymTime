using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;

namespace GymTime.Application.Services;

public class BookingService(
    IGymMemberRepository gymMemberRepository,
    IClassRepository classRepository,
    IBookingRepository bookingRepository) : IBookingService
{
    private readonly IGymMemberRepository _gymMemberRepository = gymMemberRepository;
    private readonly IClassRepository _classRepository = classRepository;
    private readonly IBookingRepository _bookingRepository = bookingRepository;

    public async Task<string> BookClassAsync(Guid gymMemberId, Guid classId)
    {
        // Get gymMember and class
        var gymMember = await _gymMemberRepository.GetByIdAsync(gymMemberId);
        var classEntity = await _classRepository.GetByIdAsync(classId);

        if (gymMember == null)
            return "Gym member not found.";

        if (classEntity == null)
            return "Class not found.";

        // Validate class capacity
        if (!classEntity.HasAvailableSlots())
            return "This class is already full.";

        // Get gymMember’s bookings for this month (mês + ano)
        var gymMemberBookings = await _bookingRepository.GetBookingsForGymMemberAsync(gymMemberId);
        var now = DateTime.UtcNow;
        var currentMonthCount = gymMemberBookings.Count(b => b.CreatedAt.Month == now.Month && b.CreatedAt.Year == now.Year);

        if (!gymMember.CanBook(currentMonthCount))
            return $"Booking limit reached for your {gymMember.PlanType} plan.";

        // Register booking
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GymMemberId = gymMemberId,
            ClassId = classId,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking);

        return "Class successfully booked!";
    }

    public async Task<string> CancelBookingAsync(Guid bookingId)
    {
        var existingBooking = await _bookingRepository.GetByIdAsync(bookingId);
        if (existingBooking == null)
            return "Booking not found.";

        await _bookingRepository.DeleteAsync(bookingId);
        return "Booking canceled successfully.";
    }
}