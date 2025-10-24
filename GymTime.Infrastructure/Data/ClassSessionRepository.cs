using GymTime.Domain.Entities;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Infrastructure.Data;

public class ClassSessionRepository(GymTimeDbContext context) : IClassSessionRepository
{
    private readonly GymTimeDbContext _context = context;

    public async Task AddAsync(ClassSession classSession)
    {
        await _context.ClassSessions.AddAsync(classSession);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.ClassSessions.FindAsync(id);
        if (entity != null)
        {
            _context.ClassSessions.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ClassSession>> GetAllAsync()
    {
        return await _context.ClassSessions
            .Include(cs => cs.Class)
            .Include(cs => cs.Bookings)
            .ToListAsync();
    }

    public async Task<ClassSession?> GetByIdAsync(Guid id)
    {
        return await _context.ClassSessions
            .Include(cs => cs.Class)
            .Include(cs => cs.Bookings)
            .FirstOrDefaultAsync(cs => cs.Id == id);
    }

    public async Task<IEnumerable<ClassSession>> GetByClassIdAsync(Guid classId)
    {
        return await _context.ClassSessions
            .Include(cs => cs.Class)
            .Include(cs => cs.Bookings)
            .Where(cs => cs.ClassId == classId)
            .ToListAsync();
    }

    public async Task UpdateAsync(ClassSession classSession)
    {
        _context.ClassSessions.Update(classSession);
        await _context.SaveChangesAsync();
    }
}
