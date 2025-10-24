using GymTime.Application.Dtos.GymMembers;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Entities;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Application.Services;

/// <summary>
/// Implementação de GymMember CRUD usando EF Core.
/// </summary>
public class GymMemberService : IGymMemberService
{
    private readonly GymTimeDbContext _context;

    public GymMemberService(GymTimeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GymMemberDto>> GetAllAsync()
    {
        return await _context.GymMembers
            .AsNoTracking()
            .Select(g => new GymMemberDto
            {
                Id = g.Id,
                Name = g.Name,
                PlanType = g.PlanType
            })
            .ToListAsync();
    }

    public async Task<GymMemberDto?> GetByIdAsync(Guid id)
    {
        var gymMember = await _context.GymMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (gymMember == null)
        {
            return null;
        }

        return new GymMemberDto { Id = gymMember.Id, Name = gymMember.Name, PlanType = gymMember.PlanType };
    }

    public async Task<GymMemberDto> CreateAsync(CreateGymMemberRequest request)
    {
        var entity = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            PlanType = request.PlanType,
            Bookings = []
        };

        _context.GymMembers.Add(entity);
        await _context.SaveChangesAsync();

        return new GymMemberDto { Id = entity.Id, Name = entity.Name, PlanType = entity.PlanType };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateGymMemberRequest request)
    {
        var entity = await _context.GymMembers.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        entity.Name = request.Name;
        entity.PlanType = request.PlanType;

        _context.GymMembers.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.GymMembers.FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.GymMembers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
