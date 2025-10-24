using GymTime.Application.Dtos.Bookings;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Application.Services;

public class BookingService(
    IGymMemberRepository gymMemberRepository,
    IClassRepository classRepository,
    IBookingRepository bookingRepository,
    GymTimeDbContext context) : IBookingService
{
    private readonly IGymMemberRepository _gymMemberRepository = gymMemberRepository;
    private readonly IClassRepository _classRepository = classRepository;
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly GymTimeDbContext _context = context;

    public async Task<string> BookClassAsync(Guid gymMemberId, Guid classSessionId)
    {
        // Get gym member and class session
        var gymMember = await _gymMemberRepository.GetByIdAsync(gymMemberId);
        var classSession = await _context.Set<ClassSession>()
            .Include(cs => cs.Class)
            .Include(cs => cs.Bookings)
            .FirstOrDefaultAsync(cs => cs.Id == classSessionId);

        if (gymMember == null)
        {
            return "Gym member not found.";
        }

        if (classSession == null)
        {
            return "Class session not found.";
        }

        if (classSession.Class == null)
        {
            return "Class not found.";
        }

        // Validate if the gym member already has a booking for this session
        bool existingBooking = await _context.Bookings
            .AnyAsync(b => b.GymMemberId == gymMemberId && b.ClassSessionId == classSessionId);

        if (existingBooking)
        {
            return "You already have a booking for this class session.";
        }

        // Validate class session capacity
        if (!classSession.HasAvailableSlots())
        {
            return "This class session is already full.";
        }

        // Get gym member's bookings and count those scheduled for this month (based on session date, not booking creation date)
        var gymMemberBookings = await _bookingRepository.GetBookingsForGymMemberAsync(gymMemberId);
        var now = DateTime.UtcNow;
        var currentMonthCount = gymMemberBookings.Count(b =>
            b.ClassSession != null &&
            b.ClassSession.Schedule.Month == now.Month &&
            b.ClassSession.Schedule.Year == now.Year);

        if (!gymMember.CanBook(currentMonthCount))
        {
            return $"Booking limit reached for your {gymMember.PlanType} plan.";
        }

        // Register booking
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GymMemberId = gymMemberId,
            ClassId = classSession.ClassId,
            ClassSessionId = classSessionId,
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepository.AddAsync(booking);

        return "Class successfully booked!";
    }

    public async Task<string> CancelBookingAsync(Guid bookingId)
    {
        var existingBooking = await _bookingRepository.GetByIdAsync(bookingId);
        if (existingBooking == null)
        {
            return "Booking not found.";
        }

        await _bookingRepository.DeleteAsync(bookingId);
        return "Booking canceled successfully.";
    }

    private static BookingDto MapToDto(Booking b)
    {
        return new BookingDto
        {
            Id = b.Id,
            GymMemberId = b.GymMemberId,
            ClassId = b.ClassId,
            ClassSessionId = b.ClassSessionId,
            CreatedAt = b.CreatedAt,
            GymMemberName = b.GymMember?.Name,
            ClassType = b.Class?.ClassType,
            SessionSchedule = b.ClassSession?.Schedule
        };
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsForGymMemberAsync(Guid gymMemberId)
    {
        var list = await _bookingRepository.GetBookingsForGymMemberAsync(gymMemberId);
        return list.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsForClassAsync(Guid classId)
    {
        var list = await _bookingRepository.GetBookingsForClassAsync(classId);
        return list.Select(MapToDto);
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId)
    {
        var b = await _bookingRepository.GetByIdAsync(bookingId);
        if (b == null)
        {
            return null;
        }

        return MapToDto(b);
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync()
    {
        var list = await _bookingRepository.GetAllAsync();
        return list.Select(MapToDto);
    }
}
