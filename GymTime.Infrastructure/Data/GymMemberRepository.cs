using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Data;

public class GymMemberRepository(GymTimeDbContext context) : IGymMemberRepository
{
    private readonly GymTimeDbContext _context = context;

    public async Task<GymMember?> GetByIdAsync(Guid id)
    {
        return await _context.GymMembers
            .Include(s => s.Bookings)
            .ThenInclude(b => b.Class)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<GymMember>> GetAllAsync()
    {
        return await _context.GymMembers
            .Include(s => s.Bookings)
            .ToListAsync();
    }

    public async Task AddAsync(GymMember GymMember)
    {
        await _context.GymMembers.AddAsync(GymMember);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(GymMember GymMember)
    {
        _context.GymMembers.Update(GymMember);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var GymMember = await _context.GymMembers.FindAsync(id);
        if (GymMember != null)
        {
            _context.GymMembers.Remove(GymMember);
            await _context.SaveChangesAsync();
        }
    }
}
