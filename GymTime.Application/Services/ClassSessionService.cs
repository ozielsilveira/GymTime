using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services.Interfaces;
using GymTime.Domain.Entities;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Application.Services;

public class ClassSessionService(GymTimeDbContext context) : IClassSessionService
{
    private readonly GymTimeDbContext _context = context;

    public async Task<IEnumerable<ClassSessionDto>> GetAllAsync()
    {
        return await _context.Set<ClassSession>()
     .AsNoTracking()
     .Include(s => s.Class)
.Include(s => s.Bookings)
    .Select(s => new ClassSessionDto
    {
        Id = s.Id,
        ClassId = s.ClassId,
        Date = s.Date,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        Schedule = s.Schedule,
        ClassType = s.Class != null ? s.Class.ClassType : null,
        CurrentBookings = s.Bookings.Count,
        MaxCapacity = s.Class != null ? s.Class.MaxCapacity : 0,
        DurationInMinutes = s.GetDurationInMinutes()
    })
  .ToListAsync();
    }

    public async Task<ClassSessionDto?> GetByIdAsync(Guid id)
    {
        var session = await _context.Set<ClassSession>()
      .AsNoTracking()
      .Include(s => s.Class)
      .Include(s => s.Bookings)
    .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) return null;

        return new ClassSessionDto
        {
            Id = session.Id,
            ClassId = session.ClassId,
            Date = session.Date,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Schedule = session.Schedule,
            ClassType = session.Class?.ClassType,
            CurrentBookings = session.Bookings.Count,
            MaxCapacity = session.Class?.MaxCapacity ?? 0,
            DurationInMinutes = session.GetDurationInMinutes()
        };
    }

    public async Task<IEnumerable<ClassSessionDto>> GetByClassIdAsync(Guid classId)
    {
        return await _context.Set<ClassSession>()
       .AsNoTracking()
       .Include(s => s.Class)
        .Include(s => s.Bookings)
   .Where(s => s.ClassId == classId)
      .Select(s => new ClassSessionDto
      {
          Id = s.Id,
          ClassId = s.ClassId,
          Date = s.Date,
          StartTime = s.StartTime,
          EndTime = s.EndTime,
          Schedule = s.Schedule,
          ClassType = s.Class != null ? s.Class.ClassType : null,
          CurrentBookings = s.Bookings.Count,
          MaxCapacity = s.Class != null ? s.Class.MaxCapacity : 0,
          DurationInMinutes = s.GetDurationInMinutes()
      })
        .ToListAsync();
    }

    public async Task<ClassSessionDto> CreateAsync(CreateClassSessionRequest request)
    {
        var classEntity = await _context.Classes.FindAsync(request.ClassId);
        if (classEntity == null)
            throw new InvalidOperationException("Class not found.");

        // Validar que EndTime é depois de StartTime
        if (request.EndTime <= request.StartTime)
            throw new InvalidOperationException("End time must be after start time.");

        // Calcular o Schedule (DateTime UTC) a partir de Date e StartTime
        // Assumindo que a Date já está no timezone correto ou será tratada pelo cliente
        var scheduleDateTime = request.Date.ToDateTime(request.StartTime);

        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = request.ClassId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Schedule = DateTime.SpecifyKind(scheduleDateTime, DateTimeKind.Utc),
            Bookings = new List<Booking>()
        };

        _context.Set<ClassSession>().Add(session);
        await _context.SaveChangesAsync();

        return new ClassSessionDto
        {
            Id = session.Id,
            ClassId = session.ClassId,
            Date = session.Date,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            Schedule = session.Schedule,
            ClassType = classEntity.ClassType,
            CurrentBookings = 0,
            MaxCapacity = classEntity.MaxCapacity,
            DurationInMinutes = session.GetDurationInMinutes()
        };
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateClassSessionRequest request)
    {
        var session = await _context.Set<ClassSession>().FindAsync(id);
        if (session == null) return false;

        // Validar que EndTime é depois de StartTime
        if (request.EndTime <= request.StartTime)
            throw new InvalidOperationException("End time must be after start time.");

        session.Date = request.Date;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;

        // Recalcular Schedule
        var scheduleDateTime = request.Date.ToDateTime(request.StartTime);
        session.Schedule = DateTime.SpecifyKind(scheduleDateTime, DateTimeKind.Utc);

        _context.Set<ClassSession>().Update(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var session = await _context.Set<ClassSession>().FindAsync(id);
        if (session == null) return false;

        _context.Set<ClassSession>().Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }
}
