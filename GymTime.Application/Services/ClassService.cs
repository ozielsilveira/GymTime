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
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Bookings)
        .Select(c => new ClassDto
        {
            Id = c.Id,
            ClassType = c.ClassType,
            MaxCapacity = c.MaxCapacity,
            Sessions = c.Sessions.Select(s => new ClassSessionDto
            {
                Id = s.Id,
                ClassId = s.ClassId,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Schedule = s.Schedule,
                ClassType = c.ClassType,
                CurrentBookings = s.Bookings.Count,
                MaxCapacity = c.MaxCapacity,
                DurationInMinutes = s.GetDurationInMinutes()
            }).ToList()
        })
             .ToListAsync();
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        var c = await _context.Classes
            .AsNoTracking()
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Bookings)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null)
        {
            return null;
        }

        return new ClassDto
        {
            Id = c.Id,
            ClassType = c.ClassType,
            MaxCapacity = c.MaxCapacity,
            Sessions = [.. c.Sessions.Select(s => new ClassSessionDto
            {
                Id = s.Id,
                ClassId = s.ClassId,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Schedule = s.Schedule,
                ClassType = c.ClassType,
                CurrentBookings = s.Bookings.Count,
                MaxCapacity = c.MaxCapacity,
                DurationInMinutes = s.GetDurationInMinutes()
            })]
        };
    }

    public async Task<ClassDto> CreateAsync(CreateClassRequest request)
    {
        // Validate times
        if (request.EndTime <= request.StartTime)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        if (request.EndDate < request.StartDate)
        {
            throw new InvalidOperationException("End date must be after or equal to start date.");
        }

        var entity = new Class
        {
            Id = Guid.NewGuid(),
            ClassType = request.ClassType,
            MaxCapacity = request.MaxCapacity,
            Sessions = [],
            Bookings = []
        };

        _context.Classes.Add(entity);
        await _context.SaveChangesAsync();

        // Generate sessions
        var sessions = GenerateSessions(
            entity.Id,
            request.StartDate,
            request.EndDate,
            request.StartTime,
            request.EndTime,
            request.DaysOfWeek
            );

        if (sessions.Count != 0)
        {
            _context.Set<ClassSession>().AddRange(sessions);
            await _context.SaveChangesAsync();
        }

        // Reload with sessions
        var createdClass = await GetByIdAsync(entity.Id);
        return createdClass!;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateClassRequest request)
    {
        var entity = await _context.Classes
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null)
        {
            return false;
        }

        // Verify if capacity reduction is allowed
        if (request.MaxCapacity < entity.MaxCapacity)
        {
            var maxBookingsInAnySession = entity.Sessions
                 .Select(s => s.Bookings.Count)
                 .DefaultIfEmpty(0)
                 .Max();

            if (maxBookingsInAnySession > request.MaxCapacity)
            {
                throw new InvalidOperationException(
                    $"Cannot reduce capacity to {request.MaxCapacity}. " +
                    $"There are sessions with {maxBookingsInAnySession} bookings.");
            }
        }

        entity.ClassType = request.ClassType;
        entity.MaxCapacity = request.MaxCapacity;

        _context.Classes.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ClassDto> UpdateWithSessionsAsync(Guid id, UpdateClassWithSessionsRequest request)
    {
        var entity = await _context.Classes
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("Class not found.");

        // Verify if capacity reduction is allowed
        if (request.MaxCapacity < entity.MaxCapacity)
        {
            var maxBookingsInAnySession = entity.Sessions
                .Select(s => s.Bookings.Count)
                .DefaultIfEmpty(0)
                .Max();

            if (maxBookingsInAnySession > request.MaxCapacity)
            {
                throw new InvalidOperationException(
              $"Cannot reduce capacity to {request.MaxCapacity}. " +
                    $"There are sessions with {maxBookingsInAnySession} bookings.");
            }
        }

        // Update basic class data
        entity.ClassType = request.ClassType;
        entity.MaxCapacity = request.MaxCapacity;

        // Remove specified sessions (only if they have no bookings)
        if (request.SessionIdsToRemove.Any())
        {
            var sessionsToRemove = entity.Sessions
                .Where(s => request.SessionIdsToRemove.Contains(s.Id))
                .ToList();

            foreach (var session in sessionsToRemove)
            {
                if (session.Bookings.Any())
                {
                    throw new InvalidOperationException(
                              $"Cannot remove session {session.Id} because it has {session.Bookings.Count} bookings.");
                }

                _context.Set<ClassSession>().Remove(session);
            }
        }

        await _context.SaveChangesAsync();

        // Add new sessions if specified
        if (request.NewSessions != null)
        {
            // Validate times
            if (request.NewSessions.EndTime <= request.NewSessions.StartTime)
            {
                throw new InvalidOperationException("End time must be after start time.");
            }

            if (request.NewSessions.EndDate < request.NewSessions.StartDate)
            {
                throw new InvalidOperationException("End date must be after or equal to start date.");
            }

            var newSessions = GenerateSessions(
                id,
                request.NewSessions.StartDate,
                request.NewSessions.EndDate,
                request.NewSessions.StartTime,
                request.NewSessions.EndTime,
                request.NewSessions.DaysOfWeek
                );

            if (newSessions.Any())
            {
                _context.Set<ClassSession>().AddRange(newSessions);
                await _context.SaveChangesAsync();
            }
        }

        // Reload with updated sessions
        var updatedClass = await GetByIdAsync(id);
        return updatedClass!;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _context.Classes
            .Include(c => c.Sessions)
            .ThenInclude(s => s.Bookings)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null)
        {
            return false;
        }

        // Verify if there are bookings in any session
        var totalBookings = entity.Sessions.Sum(s => s.Bookings.Count);
        if (totalBookings > 0)
        {
            throw new InvalidOperationException(
                $"Cannot delete class. There are {totalBookings} active bookings across all sessions. " +
                "Please cancel all bookings before deleting the class.");
        }

        _context.Classes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // Methods for session management

    public async Task<IEnumerable<ClassSessionDto>> GetSessionsByClassIdAsync(Guid classId)
    {
        var sessions = await _context.Set<ClassSession>()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Bookings)
            .Where(s => s.ClassId == classId)
            .ToListAsync();

        return sessions.Select(s => new ClassSessionDto
        {
            Id = s.Id,
            ClassId = s.ClassId,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Schedule = s.Schedule,
            ClassType = s.Class?.ClassType,
            CurrentBookings = s.Bookings.Count,
            MaxCapacity = s.Class?.MaxCapacity ?? 0,
            DurationInMinutes = s.GetDurationInMinutes()
        });
    }

    public async Task<ClassSessionDto?> GetSessionByIdAsync(Guid classId, Guid sessionId)
    {
        var session = await _context.Set<ClassSession>()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClassId == classId);

        if (session == null)
        {
            return null;
        }

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

    public async Task<IEnumerable<ClassSessionDto>> AddSessionsToClassAsync(Guid classId, AddSessionsToClassRequest request)
    {
        var classEntity = await _context.Classes.FindAsync(classId) ?? throw new InvalidOperationException("Class not found.");

        // Validate times
        if (request.EndTime <= request.StartTime)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        if (request.EndDate < request.StartDate)
        {
            throw new InvalidOperationException("End date must be after or equal to start date.");
        }

        // Generate sessions
        var sessions = GenerateSessions(
            classId,
            request.StartDate,
            request.EndDate,
            request.StartTime,
            request.EndTime,
            request.DaysOfWeek
            );

        if (sessions.Any())
        {
            _context.Set<ClassSession>().AddRange(sessions);
            await _context.SaveChangesAsync();
        }

        // Return the created sessions
        return sessions.Select(s => new ClassSessionDto
        {
            Id = s.Id,
            ClassId = s.ClassId,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Schedule = s.Schedule,
            ClassType = classEntity.ClassType,
            CurrentBookings = 0,
            MaxCapacity = classEntity.MaxCapacity,
            DurationInMinutes = s.GetDurationInMinutes()
        });
    }

    public async Task<bool> DeleteSessionAsync(Guid classId, Guid sessionId)
    {
        var session = await _context.Set<ClassSession>()
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClassId == classId);

        if (session == null)
        {
            return false;
        }

        // Verify if there are bookings
        if (session.Bookings.Any())
        {
            throw new InvalidOperationException(
                $"Cannot delete session. There are {session.Bookings.Count} active bookings. " +
                "Please cancel all bookings before deleting the session.");
        }

        _context.Set<ClassSession>().Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateSessionAsync(Guid classId, Guid sessionId, UpdateClassSessionRequest request)
    {
        var session = await _context.Set<ClassSession>()
            .Include(s => s.Bookings)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.ClassId == classId);

        if (session == null)
        {
            return false;
        }

        // Validate that EndTime is after StartTime
        if (request.EndTime <= request.StartTime)
        {
            throw new InvalidOperationException("End time must be after start time.");
        }

        // Verify if there are bookings and the change is significant
        if (session.Bookings.Any())
        {
            var hasDateChange = session.Date != request.Date;
            var hasTimeChange = session.StartTime != request.StartTime || session.EndTime != request.EndTime;

            if (hasDateChange || hasTimeChange)
            {
                throw new InvalidOperationException(
                    $"Cannot modify session date/time. There are {session.Bookings.Count} active bookings. " +
                    "Please cancel all bookings before modifying the session schedule.");
            }
        }

        session.Date = request.Date;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;

        // Recalculate Schedule
        var scheduleDateTime = request.Date.ToDateTime(request.StartTime);
        session.Schedule = DateTime.SpecifyKind(scheduleDateTime, DateTimeKind.Utc);

        _context.Set<ClassSession>().Update(session);
        await _context.SaveChangesAsync();
        return true;
    }

    // Helper method to generate sessions
    private List<ClassSession> GenerateSessions(
        Guid classId,
        DateOnly startDate,
        DateOnly endDate,
        TimeOnly startTime,
        TimeOnly endTime,
        List<DayOfWeek> daysOfWeek)
    {
        var sessions = new List<ClassSession>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            // Check if the day of the week is in the list
            if (daysOfWeek.Contains(currentDate.DayOfWeek))
            {
                var scheduleDateTime = currentDate.ToDateTime(startTime);

                sessions.Add(new ClassSession
                {
                    Id = Guid.NewGuid(),
                    ClassId = classId,
                    Date = currentDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Schedule = DateTime.SpecifyKind(scheduleDateTime, DateTimeKind.Utc),
                    Bookings = []
                });
            }

            currentDate = currentDate.AddDays(1);
        }

        return sessions;
    }
}
