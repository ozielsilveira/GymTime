using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Entities;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Application.Services;

public class ClassService(GymTimeDbContext context) : IClassService
{
    private readonly GymTimeDbContext _context = context;

    public async Task<IEnumerable<ClassDto>> GetAllAsync()
    {
        return await _context.Classes
            .AsNoTracking()
            .Select(c => new ClassDto
            {
                Id = c.Id,
                ClassType = c.ClassType,
                Schedule = c.Schedule,
                MaxCapacity = c.MaxCapacity
            })
            .ToListAsync();
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var c = await _context.Classes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return null;

        return new ClassDto
        {
            Id = c.Id,
            ClassType = c.ClassType,
            Schedule = c.Schedule,
            MaxCapacity = c.MaxCapacity
        };
    }

    public async Task<ClassDto> CreateAsync(CreateClassRequest request)
    {
        var entity = new Class
        {
            Id = Guid.NewGuid(),
            ClassType = request.ClassType,
            Schedule = request.Schedule,
            MaxCapacity = request.MaxCapacity,
            Bookings = new List<Booking>()
        };

        _context.Classes.Add(entity);
        await _context.SaveChangesAsync();

        return new ClassDto
        {
            Id = entity.Id,
            ClassType = entity.ClassType,
            Schedule = entity.Schedule,
            MaxCapacity = entity.MaxCapacity
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateClassRequest request)
    {
        var entity = await _context.Classes.FindAsync(id);
        if (entity == null) return false;

        entity.ClassType = request.ClassType;
        entity.Schedule = request.Schedule;
        entity.MaxCapacity = request.MaxCapacity;

        _context.Classes.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Classes.FindAsync(id);
        if (entity == null) return false;

        _context.Classes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}