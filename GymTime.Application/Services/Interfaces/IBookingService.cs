
namespace GymTime.Application.Services.Interfaces;

public interface IBookingService
{
    Task<string> BookClassAsync(Guid gymMemberId, Guid classId);
    Task<string> CancelBookingAsync(Guid bookingId);
}
