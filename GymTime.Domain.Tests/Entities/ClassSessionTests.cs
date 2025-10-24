using GymTime.Domain.Entities;

namespace GymTime.Domain.Tests.Entities;

public class ClassSessionTests
{
    [Fact]
    public void HasAvailableSlots_ClassNull_ReturnsFalse()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
            Class = null,
            Bookings = []
        };

        // Act
        var hasSlots = session.HasAvailableSlots();

        // Assert
        Assert.False(hasSlots);
    }

    [Fact]
    public void HasAvailableSlots_BookingsBelowCapacity_ReturnsTrue()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
            Class = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 10 },
            Bookings =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ]
        };

        // Act
        var hasSlots = session.HasAvailableSlots();

        // Assert
        Assert.True(hasSlots);
    }

    [Fact]
    public void HasAvailableSlots_BookingsAtCapacity_ReturnsFalse()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
            Class = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 2 },
            Bookings =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ]
        };

        // Act
        var hasSlots = session.HasAvailableSlots();

        // Assert
        Assert.False(hasSlots);
    }

    [Fact]
    public void HasAvailableSlots_BookingsAboveCapacity_ReturnsFalse()
    {
        // Arrange
        var classId = Guid.NewGuid();
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow,
            Class = new Class { Id = classId, ClassType = "Yoga", MaxCapacity = 2 },
            Bookings =
            [
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            ]
        };

        // Act
        var hasSlots = session.HasAvailableSlots();

        // Assert
        Assert.False(hasSlots);
    }

    [Fact]
    public void GetDurationInMinutes_OneHour_Returns60()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow
        };

        // Act
        var duration = session.GetDurationInMinutes();

        // Assert
        Assert.Equal(60, duration);
    }

    [Fact]
    public void GetDurationInMinutes_ThirtyMinutes_Returns30()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(10, 30),
            Schedule = DateTime.UtcNow
        };

        // Act
        var duration = session.GetDurationInMinutes();

        // Assert
        Assert.Equal(30, duration);
    }

    [Fact]
    public void GetDurationInMinutes_TwoHours_Returns120()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(12, 0),
            Schedule = DateTime.UtcNow
        };

        // Act
        var duration = session.GetDurationInMinutes();

        // Assert
        Assert.Equal(120, duration);
    }

    [Fact]
    public void IsFutureSession_FutureDate_ReturnsTrue()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isFuture = session.IsFutureSession();

        // Assert
        Assert.True(isFuture);
    }

    [Fact]
    public void IsFutureSession_PastDate_ReturnsFalse()
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Schedule = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var isFuture = session.IsFutureSession();

        // Assert
        Assert.False(isFuture);
    }

    [Theory]
    [InlineData(10, 0, 11, 0, 60)]
    [InlineData(9, 30, 10, 45, 75)]
    [InlineData(14, 15, 16, 0, 105)]
    [InlineData(8, 0, 8, 45, 45)]
    public void GetDurationInMinutes_VariousTimeRanges_ReturnsCorrectDuration(
        int startHour, int startMinute, int endHour, int endMinute, int expectedMinutes)
    {
        // Arrange
        var session = new ClassSession
        {
            Id = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.Today),
            StartTime = new TimeOnly(startHour, startMinute),
            EndTime = new TimeOnly(endHour, endMinute),
            Schedule = DateTime.UtcNow
        };

        // Act
        var duration = session.GetDurationInMinutes();

        // Assert
        Assert.Equal(expectedMinutes, duration);
    }
}
