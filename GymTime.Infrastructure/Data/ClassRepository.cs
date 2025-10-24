using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Data;

public class ClassRepository(GymTimeDbContext context) : IClassRepository
{
    private readonly GymTimeDbContext _context = context;

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _context.Classes
            .Include(c => c.Bookings)
            .ThenInclude(b => b.GymMember)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Class>> GetAllAsync()
    {
        return await _context.Classes
            .Include(c => c.Bookings)
            .ToListAsync();
    }

    public async Task AddAsync(Class classEntity)
    {
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Class classEntity)
    {
        _context.Classes.Update(classEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var classEntity = await _context.Classes.FindAsync(id);
        if (classEntity != null)
        {
            _context.Classes.Remove(classEntity);
            await _context.SaveChangesAsync();
        }
    }
}
