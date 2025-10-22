using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Data;

public class BookingRepository(GymTimeDbContext context) : IBookingRepository
{
    private readonly GymTimeDbContext _context = context;

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.GymMember)
            .Include(b => b.Class)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetBookingsForGymMemberAsync(Guid gymMemberId)
    {
        return await _context.Bookings
            .Where(b => b.GymMemberId == gymMemberId)
            .Include(b => b.Class)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsForClassAsync(Guid classId)
    {
        return await _context.Bookings
            .Where(b => b.ClassId == classId)
            .Include(b => b.GymMember)
            .ToListAsync();
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.GymMember)
            .Include(b => b.Class)
            .ToListAsync();
    }
}
