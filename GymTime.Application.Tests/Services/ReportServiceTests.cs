using GymTime.Application.Services;
using GymTime.Domain.Entities;
using GymTime.Domain.Enums;
using GymTime.Domain.Repositories;
using Moq;

namespace GymTime.Api.Tests.Services;

public class ReportServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<IGymMemberRepository> _mockGymMemberRepo;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
      _mockBookingRepo = new Mock<IBookingRepository>();
  _mockGymMemberRepo = new Mock<IGymMemberRepository>();
   _service = new ReportService(_mockBookingRepo.Object, _mockGymMemberRepo.Object);
    }

    [Fact]
    public async Task GetGymMemberReportAsync_MemberNotFound_ReturnsNull()
    {
        // Arrange
     var gymMemberId = Guid.NewGuid();
     _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
     .ReturnsAsync((GymMember?)null);

  // Act
     var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
  public async Task GetGymMemberReportAsync_NoBookings_ReturnsReportWithZeroBookings()
    {
        // Arrange
        var gymMemberId = Guid.NewGuid();
   var gymMember = new GymMember
  {
   Id = gymMemberId,
       Name = "John Doe",
     PlanType = PlanType.Monthly
        };

   _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
            .ReturnsAsync(gymMember);
        _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
         .ReturnsAsync(new List<Booking>());

  // Act
  var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
 Assert.NotNull(result);
     Assert.Equal(gymMemberId, result.GymMemberId);
  Assert.Equal("John Doe", result.GymMemberName);
 Assert.Equal("Monthly", result.PlanType);
        Assert.Equal(0, result.TotalBookingsThisMonth);
        Assert.Empty(result.FavoriteClassTypes);
    }

 [Fact]
    public async Task GetGymMemberReportAsync_WithBookings_ReturnsCorrectMonthlyCount()
    {
   // Arrange
        var gymMemberId = Guid.NewGuid();
      var gymMember = new GymMember
        {
    Id = gymMemberId,
     Name = "John Doe",
  PlanType = PlanType.Annual
        };

        var now = DateTime.UtcNow;
        var bookings = new List<Booking>
        {
    new()
         {
   Id = Guid.NewGuid(),
                GymMemberId = gymMemberId,
      ClassSession = new ClassSession
                {
            Schedule = new DateTime(now.Year, now.Month, 15, 10, 0, 0, DateTimeKind.Utc)
    },
     Class = new Class { ClassType = "Yoga" }
        },
       new()
{
       Id = Guid.NewGuid(),
      GymMemberId = gymMemberId,
                ClassSession = new ClassSession
                {
 Schedule = new DateTime(now.Year, now.Month, 20, 10, 0, 0, DateTimeKind.Utc)
          },
        Class = new Class { ClassType = "Pilates" }
          },
            new()
  {
    Id = Guid.NewGuid(),
         GymMemberId = gymMemberId,
        ClassSession = new ClassSession
      {
 // Last month - should not count
     Schedule = now.AddMonths(-1)
             },
                Class = new Class { ClassType = "Zumba" }
            }
     };

        _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
       .ReturnsAsync(gymMember);
    _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
         .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
     Assert.NotNull(result);
        Assert.Equal(2, result.TotalBookingsThisMonth);
        Assert.Equal(2, result.FavoriteClassTypes.Count);
 }

    [Fact]
    public async Task GetGymMemberReportAsync_CalculatesFavoriteClassTypes()
    {
  // Arrange
     var gymMemberId = Guid.NewGuid();
        var gymMember = new GymMember
   {
        Id = gymMemberId,
            Name = "Jane Doe",
     PlanType = PlanType.Quarterly
        };

        var now = DateTime.UtcNow;
        var bookings = new List<Booking>
{
            new()
{
     Id = Guid.NewGuid(),
         GymMemberId = gymMemberId,
      ClassSession = new ClassSession
           {
       Schedule = new DateTime(now.Year, now.Month, 1, 10, 0, 0, DateTimeKind.Utc)
                },
     Class = new Class { ClassType = "Yoga" }
            },
            new()
            {
    Id = Guid.NewGuid(),
 GymMemberId = gymMemberId,
         ClassSession = new ClassSession
       {
            Schedule = new DateTime(now.Year, now.Month, 2, 10, 0, 0, DateTimeKind.Utc)
     },
        Class = new Class { ClassType = "Yoga" }
      },
            new()
       {
      Id = Guid.NewGuid(),
         GymMemberId = gymMemberId,
       ClassSession = new ClassSession
       {
           Schedule = new DateTime(now.Year, now.Month, 3, 10, 0, 0, DateTimeKind.Utc)
            },
      Class = new Class { ClassType = "Yoga" }
     },
            new()
   {
      Id = Guid.NewGuid(),
           GymMemberId = gymMemberId,
     ClassSession = new ClassSession
                {
  Schedule = new DateTime(now.Year, now.Month, 4, 10, 0, 0, DateTimeKind.Utc)
          },
    Class = new Class { ClassType = "Pilates" }
            },
            new()
         {
          Id = Guid.NewGuid(),
      GymMemberId = gymMemberId,
          ClassSession = new ClassSession
       {
   Schedule = new DateTime(now.Year, now.Month, 5, 10, 0, 0, DateTimeKind.Utc)
    },
      Class = new Class { ClassType = "Pilates" }
            },
     new()
         {
    Id = Guid.NewGuid(),
      GymMemberId = gymMemberId,
        ClassSession = new ClassSession
         {
   Schedule = new DateTime(now.Year, now.Month, 6, 10, 0, 0, DateTimeKind.Utc)
    },
                Class = new Class { ClassType = "Zumba" }
            }
        };

        _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
 .ReturnsAsync(gymMember);
        _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
            .ReturnsAsync(bookings);

        // Act
  var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
      Assert.NotNull(result);
     Assert.Equal(6, result.TotalBookingsThisMonth);
      Assert.Equal(3, result.FavoriteClassTypes.Count);
  Assert.Equal("Yoga", result.FavoriteClassTypes[0]); // Most frequent
        Assert.Equal("Pilates", result.FavoriteClassTypes[1]); // Second most frequent
        Assert.Equal("Zumba", result.FavoriteClassTypes[2]); // Third most frequent
    }

    [Fact]
    public async Task GetGymMemberReportAsync_HandlesNullClassReferences()
    {
        // Arrange
        var gymMemberId = Guid.NewGuid();
        var gymMember = new GymMember
        {
    Id = gymMemberId,
            Name = "John Doe",
    PlanType = PlanType.Monthly
        };

     var now = DateTime.UtcNow;
   var bookings = new List<Booking>
        {
        new()
   {
        Id = Guid.NewGuid(),
    GymMemberId = gymMemberId,
     ClassSession = new ClassSession
    {
          Schedule = new DateTime(now.Year, now.Month, 15, 10, 0, 0, DateTimeKind.Utc)
       },
  Class = null // Null Class
            },
            new()
         {
    Id = Guid.NewGuid(),
        GymMemberId = gymMemberId,
      ClassSession = new ClassSession
    {
                Schedule = new DateTime(now.Year, now.Month, 20, 10, 0, 0, DateTimeKind.Utc)
            },
       Class = new Class { ClassType = "Yoga" }
   }
        };

 _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
      .ReturnsAsync(gymMember);
        _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
            .ReturnsAsync(bookings);

        // Act
 var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalBookingsThisMonth);
        Assert.Single(result.FavoriteClassTypes); // Only "Yoga" should be counted
        Assert.Equal("Yoga", result.FavoriteClassTypes[0]);
    }

    [Fact]
    public async Task GetGymMemberReportAsync_HandlesNullClassSession()
    {
        // Arrange
        var gymMemberId = Guid.NewGuid();
  var gymMember = new GymMember
   {
    Id = gymMemberId,
    Name = "John Doe",
      PlanType = PlanType.Monthly
  };

     var now = DateTime.UtcNow;
        var bookings = new List<Booking>
        {
  new()
     {
             Id = Guid.NewGuid(),
           GymMemberId = gymMemberId,
        ClassSession = null, // Null ClassSession
         Class = new Class { ClassType = "Yoga" }
    },
    new()
            {
      Id = Guid.NewGuid(),
         GymMemberId = gymMemberId,
 ClassSession = new ClassSession
{
         Schedule = new DateTime(now.Year, now.Month, 20, 10, 0, 0, DateTimeKind.Utc)
 },
    Class = new Class { ClassType = "Pilates" }
            }
  };

        _mockGymMemberRepo.Setup(x => x.GetByIdAsync(gymMemberId))
     .ReturnsAsync(gymMember);
        _mockBookingRepo.Setup(x => x.GetBookingsForGymMemberAsync(gymMemberId))
     .ReturnsAsync(bookings);

     // Act
    var result = await _service.GetGymMemberReportAsync(gymMemberId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalBookingsThisMonth); // Only booking with valid ClassSession
        Assert.Single(result.FavoriteClassTypes);
      Assert.Equal("Pilates", result.FavoriteClassTypes[0]);
    }
}
