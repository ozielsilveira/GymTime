using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services;
using GymTime.Domain.Entities;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Api.Tests.Services;

public class ClassServiceTests : IDisposable
{
    private readonly GymTimeDbContext _context;
    private readonly ClassService _service;

    public ClassServiceTests()
    {
   var options = new DbContextOptionsBuilder<GymTimeDbContext>()
  .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

        _context = new GymTimeDbContext(options);
      _service = new ClassService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllClasses()
    {
        // Arrange
      await _context.Classes.AddRangeAsync(
          new Class { Id = Guid.NewGuid(), ClassType = "Yoga", MaxCapacity = 10 },
      new Class { Id = Guid.NewGuid(), ClassType = "Pilates", MaxCapacity = 15 }
 );
        await _context.SaveChangesAsync();

        // Act
  var result = await _service.GetAllAsync();

  // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ClassExists_ReturnsClass()
    {
        // Arrange
        var id = Guid.NewGuid();
  await _context.Classes.AddAsync(
            new Class { Id = id, ClassType = "Yoga", MaxCapacity = 10 }
    );
        await _context.SaveChangesAsync();

  // Act
    var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Yoga", result.ClassType);
      Assert.Equal(10, result.MaxCapacity);
    }

    [Fact]
    public async Task GetByIdAsync_ClassNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

  [Fact]
    public async Task CreateAsync_ValidRequest_CreatesClassWithSessions()
    {
        // Arrange
        var request = new CreateClassRequest
        {
    ClassType = "Yoga",
         MaxCapacity = 10,
   StartDate = DateOnly.FromDateTime(DateTime.Today),
        EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
       StartTime = new TimeOnly(10, 0),
    EndTime = new TimeOnly(11, 0),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }
        };

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Yoga", result.ClassType);
   Assert.Equal(10, result.MaxCapacity);
        Assert.NotEmpty(result.Sessions);
    }

    [Fact]
    public async Task CreateAsync_EndTimeBeforeStartTime_ThrowsException()
    {
   // Arrange
        var request = new CreateClassRequest
        {
            ClassType = "Yoga",
   MaxCapacity = 10,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
         StartTime = new TimeOnly(11, 0),
   EndTime = new TimeOnly(10, 0), // Invalid
   DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }
    };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
   async () => await _service.CreateAsync(request)
   );
    }

    [Fact]
    public async Task CreateAsync_EndDateBeforeStartDate_ThrowsException()
    {
      // Arrange
        var request = new CreateClassRequest
        {
     ClassType = "Yoga",
 MaxCapacity = 10,
      StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
          EndDate = DateOnly.FromDateTime(DateTime.Today), // Invalid
       StartTime = new TimeOnly(10, 0),
       EndTime = new TimeOnly(11, 0),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }
      };

// Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
 async () => await _service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task UpdateAsync_ClassExists_UpdatesClass()
    {
      // Arrange
    var id = Guid.NewGuid();
  await _context.Classes.AddAsync(
         new Class { Id = id, ClassType = "Yoga", MaxCapacity = 10 }
  );
    await _context.SaveChangesAsync();

        var updateRequest = new UpdateClassRequest
        {
     ClassType = "Advanced Yoga",
        MaxCapacity = 15
        };

        // Act
        var result = await _service.UpdateAsync(id, updateRequest);

        // Assert
        Assert.True(result);
        var updated = await _context.Classes.FindAsync(id);
        Assert.Equal("Advanced Yoga", updated?.ClassType);
        Assert.Equal(15, updated?.MaxCapacity);
}

    [Fact]
    public async Task UpdateAsync_ReduceCapacityWithBookings_ThrowsException()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        var session = new ClassSession
   {
  Id = sessionId,
     ClassId = classId,
   Date = DateOnly.FromDateTime(DateTime.Today),
  StartTime = new TimeOnly(10, 0),
    EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
     Bookings = new List<Booking>
  {
           new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId },
                new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId },
          new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId }
            }
        };

        classEntity.Sessions.Add(session);
        await _context.Classes.AddAsync(classEntity);
      await _context.SaveChangesAsync();

        var updateRequest = new UpdateClassRequest
     {
      ClassType = "Yoga",
 MaxCapacity = 2 // Less than current bookings
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.UpdateAsync(classId, updateRequest)
);
    }

    [Fact]
    public async Task UpdateAsync_ClassNotFound_ReturnsFalse()
    {
      // Arrange
        var id = Guid.NewGuid();
        var updateRequest = new UpdateClassRequest
  {
            ClassType = "Yoga",
  MaxCapacity = 15
        };

 // Act
        var result = await _service.UpdateAsync(id, updateRequest);

// Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ClassExists_DeletesClass()
    {
        // Arrange
        var id = Guid.NewGuid();
        await _context.Classes.AddAsync(
        new Class { Id = id, ClassType = "Yoga", MaxCapacity = 10 }
    );
    await _context.SaveChangesAsync();

    // Act
        var result = await _service.DeleteAsync(id);

   // Assert
     Assert.True(result);
        var deleted = await _context.Classes.FindAsync(id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_ClassWithBookings_ThrowsException()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        var session = new ClassSession
        {
            Id = sessionId,
ClassId = classId,
        Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
       EndTime = new TimeOnly(11, 0),
      Schedule = DateTime.UtcNow,
     Bookings = new List<Booking>
            {
  new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId }
    }
        };

        classEntity.Sessions.Add(session);
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
  async () => await _service.DeleteAsync(classId)
        );
    }

    [Fact]
    public async Task DeleteAsync_ClassNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetSessionsByClassIdAsync_ReturnsSessions()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        
        var session1 = new ClassSession
    {
          Id = Guid.NewGuid(),
  ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today),
     StartTime = new TimeOnly(10, 0),
   EndTime = new TimeOnly(11, 0),
       Schedule = DateTime.UtcNow
        };

    var session2 = new ClassSession
        {
 Id = Guid.NewGuid(),
            ClassId = classId,
  Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
  StartTime = new TimeOnly(10, 0),
    EndTime = new TimeOnly(11, 0),
        Schedule = DateTime.UtcNow.AddDays(1)
      };

        classEntity.Sessions.Add(session1);
        classEntity.Sessions.Add(session2);
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

        // Act
  var result = await _service.GetSessionsByClassIdAsync(classId);

        // Assert
   Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddSessionsToClassAsync_ValidRequest_AddsSessions()
    {
        // Arrange
        var classId = Guid.NewGuid();
        await _context.Classes.AddAsync(
  new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 }
        );
await _context.SaveChangesAsync();

     var request = new AddSessionsToClassRequest
        {
       StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
StartTime = new TimeOnly(10, 0),
     EndTime = new TimeOnly(11, 0),
            DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Wednesday }
    };

   // Act
        var result = await _service.AddSessionsToClassAsync(classId, request);

        // Assert
        Assert.NotEmpty(result);
  }

    [Fact]
 public async Task AddSessionsToClassAsync_ClassNotFound_ThrowsException()
    {
 // Arrange
        var classId = Guid.NewGuid();
        var request = new AddSessionsToClassRequest
        {
    StartDate = DateOnly.FromDateTime(DateTime.Today),
     EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
       StartTime = new TimeOnly(10, 0),
      EndTime = new TimeOnly(11, 0),
        DaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday }
    };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.AddSessionsToClassAsync(classId, request)
      );
    }

    [Fact]
    public async Task DeleteSessionAsync_SessionExists_DeletesSession()
    {
        // Arrange
        var classId = Guid.NewGuid();
   var sessionId = Guid.NewGuid();
        
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
     var session = new ClassSession
        {
  Id = sessionId,
            ClassId = classId,
    Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
  EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow
 };

    classEntity.Sessions.Add(session);
    await _context.Classes.AddAsync(classEntity);
   await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteSessionAsync(classId, sessionId);

        // Assert
        Assert.True(result);
        var deleted = await _context.Set<ClassSession>().FindAsync(sessionId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteSessionAsync_SessionWithBookings_ThrowsException()
    {
        // Arrange
 var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        var session = new ClassSession
        {
   Id = sessionId,
  ClassId = classId,
    Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
   EndTime = new TimeOnly(11, 0),
      Schedule = DateTime.UtcNow,
    Bookings = new List<Booking>
{
new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId }
          }
        };

        classEntity.Sessions.Add(session);
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.DeleteSessionAsync(classId, sessionId)
   );
    }

    [Fact]
    public async Task UpdateSessionAsync_ValidRequest_UpdatesSession()
    {
    // Arrange
        var classId = Guid.NewGuid();
   var sessionId = Guid.NewGuid();
    
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        var session = new ClassSession
     {
       Id = sessionId,
     ClassId = classId,
Date = DateOnly.FromDateTime(DateTime.Today),
 StartTime = new TimeOnly(10, 0),
    EndTime = new TimeOnly(11, 0),
Schedule = DateTime.UtcNow
      };

        classEntity.Sessions.Add(session);
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

    var updateRequest = new UpdateClassSessionRequest
        {
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(14, 0),
  EndTime = new TimeOnly(15, 0)
  };

        // Act
        var result = await _service.UpdateSessionAsync(classId, sessionId, updateRequest);

   // Assert
        Assert.True(result);
        var updated = await _context.Set<ClassSession>().FindAsync(sessionId);
        Assert.Equal(updateRequest.Date, updated?.Date);
        Assert.Equal(updateRequest.StartTime, updated?.StartTime);
        Assert.Equal(updateRequest.EndTime, updated?.EndTime);
    }

[Fact]
 public async Task UpdateSessionAsync_SessionWithBookings_ThrowsException()
  {
        // Arrange
        var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
   
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        var session = new ClassSession
    {
         Id = sessionId,
     ClassId = classId,
     Date = DateOnly.FromDateTime(DateTime.Today),
   StartTime = new TimeOnly(10, 0),
   EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
            Bookings = new List<Booking>
   {
    new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid(), ClassId = classId, ClassSessionId = sessionId }
 }
        };

classEntity.Sessions.Add(session);
     await _context.Classes.AddAsync(classEntity);
 await _context.SaveChangesAsync();

var updateRequest = new UpdateClassSessionRequest
        {
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), // Date change with bookings
      StartTime = new TimeOnly(10, 0),
          EndTime = new TimeOnly(11, 0)
        };

     // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
      async () => await _service.UpdateSessionAsync(classId, sessionId, updateRequest)
        );
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
     GC.SuppressFinalize(this);
  }
}
