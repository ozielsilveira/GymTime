using GymTime.Application.Services;
using GymTime.Domain.Entities;
using GymTime.Domain.Enums;
using GymTime.Domain.Repositories;
using GymTime.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GymTime.Api.Tests.Services;

public class BookingServiceTests : IDisposable
{
  private readonly Mock<IGymMemberRepository> _mockGymMemberRepo;
    private readonly Mock<IClassRepository> _mockClassRepo;
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly GymTimeDbContext _context;
    private readonly BookingService _service;

    public BookingServiceTests()
 {
    _mockGymMemberRepo = new Mock<IGymMemberRepository>();
    _mockClassRepo = new Mock<IClassRepository>();
   _mockBookingRepo = new Mock<IBookingRepository>();
        
        var options = new DbContextOptionsBuilder<GymTimeDbContext>()
     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
 .Options;
   _context = new GymTimeDbContext(options);

 _service = new BookingService(
    _mockGymMemberRepo.Object,
       _mockClassRepo.Object,
    _mockBookingRepo.Object,
    _context);
    }

    [Fact]
    public async Task BookClassAsync_GymMemberNotFound_ReturnsErrorMessage()
    {
        // Arrange
     var gymMemberId = Guid.NewGuid();
var classSessionId = Guid.NewGuid();
        _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
 .ReturnsAsync((GymMember?)null);

        // Act
        var result = await _service.BookClassAsync(gymMemberId, classSessionId);

// Assert
    Assert.Equal("Gym member not found.", result);
  }

    [Fact]
    public async Task BookClassAsync_ClassSessionNotFound_ReturnsErrorMessage()
    {
   // Arrange
    var gymMemberId = Guid.NewGuid();
  var classSessionId = Guid.NewGuid();
      var gymMember = new GymMember
 {
     Id = gymMemberId,
 Name = "John Doe",
   PlanType = PlanType.Monthly
  };

     _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
      .ReturnsAsync(gymMember);

 // Act
   var result = await _service.BookClassAsync(gymMemberId, classSessionId);

     // Assert
   Assert.Equal("Class session not found.", result);
    }

    [Fact]
    public async Task BookClassAsync_SessionFull_ReturnsErrorMessage()
    {
        // Arrange
        var gymMemberId = Guid.NewGuid();
        var classSessionId = Guid.NewGuid();
   var classId = Guid.NewGuid();

        var gymMember = new GymMember
    {
       Id = gymMemberId,
 Name = "John Doe",
    PlanType = PlanType.Monthly
        };

        var classEntity = new Class
      {
  Id = classId,
   ClassType = "Yoga",
MaxCapacity = 2
      };

     var classSession = new ClassSession
        {
   Id = classSessionId,
 ClassId = classId,
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
       StartTime = new TimeOnly(10, 0),
  EndTime = new TimeOnly(11, 0),
    Schedule = DateTime.UtcNow.AddDays(1),
      Class = classEntity,
   Bookings = new List<Booking>
     {
 new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid() },
         new() { Id = Guid.NewGuid(), GymMemberId = Guid.NewGuid() }
            }
    };

await _context.Classes.AddAsync(classEntity);
        await _context.Set<ClassSession>().AddAsync(classSession);
    await _context.SaveChangesAsync();

    _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
        .ReturnsAsync(gymMember);

     // Act
   var result = await _service.BookClassAsync(gymMemberId, classSessionId);

        // Assert
   Assert.Equal("This class session is already full.", result);
    }

    [Fact]
  public async Task BookClassAsync_BookingLimitReached_ReturnsErrorMessage()
 {
     // Arrange
        var gymMemberId = Guid.NewGuid();
        var classSessionId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var gymMember = new GymMember
        {
   Id = gymMemberId,
      Name = "John Doe",
       PlanType = PlanType.Monthly // Monthly = 12 bookings/month
        };

        var classEntity = new Class
        {
 Id = classId,
            ClassType = "Yoga",
         MaxCapacity = 10
        };

        var classSession = new ClassSession
        {
            Id = classSessionId,
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
          StartTime = new TimeOnly(10, 0),
     EndTime = new TimeOnly(11, 0),
   Schedule = DateTime.UtcNow.AddDays(1),
  Class = classEntity,
      Bookings = new List<Booking>()
      };

     await _context.Classes.AddAsync(classEntity);
        await _context.Set<ClassSession>().AddAsync(classSession);
        await _context.SaveChangesAsync();

    // Member already has 12 bookings this month (limit for Monthly plan)
 var existingBookings = new List<Booking>();
        for (int i = 0; i < 12; i++)
  {
  existingBookings.Add(new Booking
  {
     Id = Guid.NewGuid(),
      GymMemberId = gymMemberId,
        ClassSession = new ClassSession
      {
        Schedule = DateTime.UtcNow
       }
       });
        }

        _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
 .ReturnsAsync(gymMember);

   _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
    .ReturnsAsync(existingBookings);

        // Act
  var result = await _service.BookClassAsync(gymMemberId, classSessionId);

        // Assert
      Assert.Equal("Booking limit reached for your Monthly plan.", result);
    }

    [Fact]
    public async Task BookClassAsync_Success_ReturnsSuccessMessage()
    {
  // Arrange
      var gymMemberId = Guid.NewGuid();
 var classSessionId = Guid.NewGuid();
     var classId = Guid.NewGuid();

        var gymMember = new GymMember
     {
Id = gymMemberId,
      Name = "John Doe",
     PlanType = PlanType.Monthly
        };

        var classEntity = new Class
        {
       Id = classId,
      ClassType = "Yoga",
 MaxCapacity = 10
     };

 var classSession = new ClassSession
      {
 Id = classSessionId,
            ClassId = classId,
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            StartTime = new TimeOnly(10, 0),
     EndTime = new TimeOnly(11, 0),
   Schedule = DateTime.UtcNow.AddDays(1),
            Class = classEntity,
   Bookings = new List<Booking>()
        };

        await _context.Classes.AddAsync(classEntity);
        await _context.Set<ClassSession>().AddAsync(classSession);
        await _context.SaveChangesAsync();

  _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
    .ReturnsAsync(gymMember);

     _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
     .ReturnsAsync(new List<Booking>());

      _mockBookingRepo.Setup(x => x.AddAsync(It.IsAny<Booking>()))
    .Returns(Task.CompletedTask);

    // Act
    var result = await _service.BookClassAsync(gymMemberId, classSessionId);

 // Assert
 Assert.Equal("Class successfully booked!", result);
    _mockBookingRepo.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Once);
 }

    [Fact]
 public async Task CancelBookingAsync_BookingNotFound_ReturnsErrorMessage()
    {
        // Arrange
     var bookingId = Guid.NewGuid();
        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId))
  .ReturnsAsync((Booking?)null);

        // Act
   var result = await _service.CancelBookingAsync(bookingId);

        // Assert
        Assert.Equal("Booking not found.", result);
    }

    [Fact]
    public async Task CancelBookingAsync_Success_ReturnsSuccessMessage()
    {
    // Arrange
        var bookingId = Guid.NewGuid();
      var booking = new Booking
   {
        Id = bookingId,
        GymMemberId = Guid.NewGuid(),
 ClassId = Guid.NewGuid(),
        ClassSessionId = Guid.NewGuid()
   };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId))
        .ReturnsAsync(booking);
        _mockBookingRepo.Setup(x => x.DeleteAsync(bookingId))
         .Returns(Task.CompletedTask);

        // Act
   var result = await _service.CancelBookingAsync(bookingId);

        // Assert
        Assert.Equal("Booking canceled successfully.", result);
   _mockBookingRepo.Verify(x => x.DeleteAsync(bookingId), Times.Once);
    }

    [Fact]
    public async Task GetBookingsForGymMemberAsync_ReturnsBookings()
    {
        // Arrange
  var gymMemberId = Guid.NewGuid();
   var bookings = new List<Booking>
      {
      new()
 {
       Id = Guid.NewGuid(),
    GymMemberId = gymMemberId,
    ClassId = Guid.NewGuid(),
  ClassSessionId = Guid.NewGuid(),
     CreatedAt = DateTime.UtcNow,
      GymMember = new GymMember { Name = "John" },
 Class = new Class { ClassType = "Yoga" },
 ClassSession = new ClassSession { Schedule = DateTime.UtcNow }
     }
 };

        _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetBookingsForGymMemberAsync(gymMemberId);

        // Assert
    Assert.Single(result);
   Assert.Equal(gymMemberId, result.First().GymMemberId);
 }

 [Fact]
    public async Task GetBookingsForClassAsync_ReturnsBookings()
    {
        // Arrange
var classId = Guid.NewGuid();
   var bookings = new List<Booking>
        {
      new()
{
           Id = Guid.NewGuid(),
        GymMemberId = Guid.NewGuid(),
           ClassId = classId,
    ClassSessionId = Guid.NewGuid(),
 CreatedAt = DateTime.UtcNow
    }
   };

      _mockBookingRepo.Setup(x => x.GetBookingsForClassAsync(classId))
        .ReturnsAsync(bookings);

        // Act
     var result = await _service.GetBookingsForClassAsync(classId);

    // Assert
     Assert.Single(result);
    Assert.Equal(classId, result.First().ClassId);
    }

    [Fact]
    public async Task GetBookingByIdAsync_BookingExists_ReturnsBooking()
    {
// Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
    {
            Id = bookingId,
     GymMemberId = Guid.NewGuid(),
      ClassId = Guid.NewGuid(),
   ClassSessionId = Guid.NewGuid(),
      CreatedAt = DateTime.UtcNow
        };

    _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId))
.ReturnsAsync(booking);

        // Act
        var result = await _service.GetBookingByIdAsync(bookingId);

        // Assert
        Assert.NotNull(result);
 Assert.Equal(bookingId, result.Id);
    }

  [Fact]
    public async Task GetBookingByIdAsync_BookingNotFound_ReturnsNull()
    {
        // Arrange
var bookingId = Guid.NewGuid();
    _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId))
       .ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetBookingByIdAsync(bookingId);

      // Assert
   Assert.Null(result);
    }

    [Fact]
  public async Task GetAllAsync_ReturnsAllBookings()
    {
  // Arrange
        var bookings = new List<Booking>
        {
      new()
   {
      Id = Guid.NewGuid(),
      GymMemberId = Guid.NewGuid(),
          ClassId = Guid.NewGuid(),
  ClassSessionId = Guid.NewGuid(),
    CreatedAt = DateTime.UtcNow
 },
     new()
    {
   Id = Guid.NewGuid(),
    GymMemberId = Guid.NewGuid(),
        ClassId = Guid.NewGuid(),
   ClassSessionId = Guid.NewGuid(),
  CreatedAt = DateTime.UtcNow
         }
  };

    _mockBookingRepo.Setup(x => x.GetAllAsync())
 .ReturnsAsync(bookings);

   // Act
        var result = await _service.GetAllAsync();

      // Assert
        Assert.Equal(2, result.Count());
    }

 public void Dispose()
    {
        _context.Database.EnsureDeleted();
  _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
