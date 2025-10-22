namespace GymTime.Application.Services.Interfaces;

using GymTime.Application.Dtos.Bookings;

public interface IBookingService
{
    Task<string> BookClassAsync(Guid gymMemberId, Guid classId);
    Task<string> CancelBookingAsync(Guid bookingId);
    Task<IEnumerable<BookingDto>> GetBookingsForGymMemberAsync(Guid gymMemberId);
    Task<IEnumerable<BookingDto>> GetBookingsForClassAsync(Guid classId);
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId);
    Task<IEnumerable<BookingDto>> GetAllAsync();
}
