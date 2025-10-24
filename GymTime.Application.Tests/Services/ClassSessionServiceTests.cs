using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services;
using GymTime.Domain.Entities;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GymTime.Api.Tests.Services;

public class ClassSessionServiceTests : IDisposable
{
    private readonly GymTimeDbContext _context;
    private readonly ClassSessionService _service;

    public ClassSessionServiceTests()
    {
        var options = new DbContextOptionsBuilder<GymTimeDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new GymTimeDbContext(options);
 _service = new ClassSessionService(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSessions()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

        await _context.Set<ClassSession>().AddRangeAsync(
          new ClassSession
        {
         Id = Guid.NewGuid(),
             ClassId = classId,
             Date = DateOnly.FromDateTime(DateTime.Today),
     StartTime = new TimeOnly(10, 0),
   EndTime = new TimeOnly(11, 0),
   Schedule = DateTime.UtcNow
        },
       new ClassSession
          {
                Id = Guid.NewGuid(),
       ClassId = classId,
    Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
      StartTime = new TimeOnly(10, 0),
              EndTime = new TimeOnly(11, 0),
  Schedule = DateTime.UtcNow.AddDays(1)
      }
        );
        await _context.SaveChangesAsync();

   // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_SessionExists_ReturnsSession()
    {
  // Arrange
        var classId = Guid.NewGuid();
   var sessionId = Guid.NewGuid();
     
      var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
        
      var session = new ClassSession
  {
     Id = sessionId,
      ClassId = classId,
        Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
          EndTime = new TimeOnly(11, 0),
    Schedule = DateTime.UtcNow
        };
        await _context.Set<ClassSession>().AddAsync(session);
        await _context.SaveChangesAsync();

        // Act
 var result = await _service.GetByIdAsync(sessionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sessionId, result.Id);
     Assert.Equal("Yoga", result.ClassType);
    }

    [Fact]
    public async Task GetByIdAsync_SessionNotFound_ReturnsNull()
  {
        // Arrange
    var sessionId = Guid.NewGuid();

        // Act
        var result = await _service.GetByIdAsync(sessionId);

      // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByClassIdAsync_ReturnsSessions()
    {
        // Arrange
        var classId = Guid.NewGuid();
   var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
     await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

        await _context.Set<ClassSession>().AddRangeAsync(
   new ClassSession
    {
                Id = Guid.NewGuid(),
  ClassId = classId,
                Date = DateOnly.FromDateTime(DateTime.Today),
    StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(11, 0),
        Schedule = DateTime.UtcNow
            },
            new ClassSession
    {
         Id = Guid.NewGuid(),
        ClassId = classId,
          Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(10, 0),
     EndTime = new TimeOnly(11, 0),
 Schedule = DateTime.UtcNow.AddDays(1)
    }
   );
        await _context.SaveChangesAsync();

 // Act
  var result = await _service.GetByClassIdAsync(classId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Equal(classId, s.ClassId));
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesSession()
    {
     // Arrange
        var classId = Guid.NewGuid();
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

   var request = new CreateClassSessionRequest
        {
            ClassId = classId,
        Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
 StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0)
        };

        // Act
  var result = await _service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
      Assert.Equal(classId, result.ClassId);
        Assert.Equal("Yoga", result.ClassType);
        Assert.Equal(10, result.MaxCapacity);
      Assert.Equal(0, result.CurrentBookings);
    }

    [Fact]
    public async Task CreateAsync_ClassNotFound_ThrowsException()
    {
        // Arrange
        var classId = Guid.NewGuid();
  var request = new CreateClassSessionRequest
 {
ClassId = classId,
       Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0)
};

     // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
     async () => await _service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task CreateAsync_EndTimeBeforeStartTime_ThrowsException()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
        await _context.SaveChangesAsync();

  var request = new CreateClassSessionRequest
{
  ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
    StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(10, 0) // Invalid
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task UpdateAsync_SessionExists_UpdatesSession()
    {
        // Arrange
 var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        
    var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
   
        var session = new ClassSession
  {
       Id = sessionId,
       ClassId = classId,
         Date = DateOnly.FromDateTime(DateTime.Today),
          StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow
        };
   await _context.Set<ClassSession>().AddAsync(session);
  await _context.SaveChangesAsync();

     var updateRequest = new UpdateClassSessionRequest
    {
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
        StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(15, 0)
        };

        // Act
        var result = await _service.UpdateAsync(sessionId, updateRequest);

        // Assert
        Assert.True(result);
        var updated = await _context.Set<ClassSession>().FindAsync(sessionId);
        Assert.Equal(updateRequest.Date, updated?.Date);
        Assert.Equal(updateRequest.StartTime, updated?.StartTime);
   Assert.Equal(updateRequest.EndTime, updated?.EndTime);
    }

  [Fact]
    public async Task UpdateAsync_SessionNotFound_ReturnsFalse()
    {
      // Arrange
        var sessionId = Guid.NewGuid();
        var updateRequest = new UpdateClassSessionRequest
    {
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
    StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(15, 0)
        };

        // Act
   var result = await _service.UpdateAsync(sessionId, updateRequest);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_EndTimeBeforeStartTime_ThrowsException()
    {
        // Arrange
      var classId = Guid.NewGuid();
    var sessionId = Guid.NewGuid();
    
        var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
 
        var session = new ClassSession
      {
  Id = sessionId,
            ClassId = classId,
          Date = DateOnly.FromDateTime(DateTime.Today),
        StartTime = new TimeOnly(10, 0),
       EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow
        };
        await _context.Set<ClassSession>().AddAsync(session);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateClassSessionRequest
        {
   Date = DateOnly.FromDateTime(DateTime.Today),
         StartTime = new TimeOnly(15, 0),
   EndTime = new TimeOnly(14, 0) // Invalid
        };

  // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(
         async () => await _service.UpdateAsync(sessionId, updateRequest)
      );
  }

    [Fact]
    public async Task DeleteAsync_SessionExists_DeletesSession()
    {
        // Arrange
   var classId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        
      var classEntity = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 };
        await _context.Classes.AddAsync(classEntity);
        
        var session = new ClassSession
        {
            Id = sessionId,
        ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today),
  StartTime = new TimeOnly(10, 0),
     EndTime = new TimeOnly(11, 0),
          Schedule = DateTime.UtcNow
        };
    await _context.Set<ClassSession>().AddAsync(session);
      await _context.SaveChangesAsync();

        // Act
    var result = await _service.DeleteAsync(sessionId);

  // Assert
   Assert.True(result);
      var deleted = await _context.Set<ClassSession>().FindAsync(sessionId);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_SessionNotFound_ReturnsFalse()
    {
        // Arrange
        var sessionId = Guid.NewGuid();

 // Act
        var result = await _service.DeleteAsync(sessionId);

        // Assert
        Assert.False(result);
    }

  public void Dispose()
    {
        _context.Database.EnsureDeleted();
     _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
