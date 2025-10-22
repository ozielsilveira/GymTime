using GymTime.Domain.Entities;

namespace GymTime.Domain.Repositories;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);
    Task<Booking> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetBookingsForGymMemberAsync(Guid gymMemberId);
    Task<IEnumerable<Booking>> GetBookingsForClassAsync(Guid classId);
    Task DeleteAsync(Guid id);
}
