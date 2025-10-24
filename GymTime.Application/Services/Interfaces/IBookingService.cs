
using GymTime.Application.Dtos.Bookings;

namespace GymTime.Application.Services.Interfaces;
public interface IBookingService
{
    Task<string> BookClassAsync(Guid gymMemberId, Guid classSessionId);
    Task<string> CancelBookingAsync(Guid bookingId);
    Task<IEnumerable<BookingDto>> GetBookingsForGymMemberAsync(Guid gymMemberId);
    Task<IEnumerable<BookingDto>> GetBookingsForClassAsync(Guid classId);
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId);
    Task<IEnumerable<BookingDto>> GetAllAsync();
}
