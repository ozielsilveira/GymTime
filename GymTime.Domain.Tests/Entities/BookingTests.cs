using GymTime.Domain.Entities;

namespace GymTime.Domain.Tests.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_Initialize_ShouldSetCreatedAtToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var booking = new Booking();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.InRange(booking.CreatedAt, beforeCreation, afterCreation);
    }

    [Fact]
    public void Booking_SetProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var gymMemberId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var classSessionId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var booking = new Booking
        {
            Id = id,
            GymMemberId = gymMemberId,
            ClassId = classId,
            ClassSessionId = classSessionId,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, booking.Id);
        Assert.Equal(gymMemberId, booking.GymMemberId);
        Assert.Equal(classId, booking.ClassId);
        Assert.Equal(classSessionId, booking.ClassSessionId);
        Assert.Equal(createdAt, booking.CreatedAt);
    }

    [Fact]
    public void Booking_SetNavigationProperties_ShouldSetCorrectly()
    {
        // Arrange
        var gymMember = new GymMember { Id = Guid.NewGuid(), Name = "John Doe" };
        var classEntity = new Class { Id = Guid.NewGuid(), ClassType = "Yoga" };
        var classSession = new ClassSession { Id = Guid.NewGuid() };

        // Act
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GymMember = gymMember,
            Class = classEntity,
            ClassSession = classSession
        };

        // Assert
        Assert.NotNull(booking.GymMember);
        Assert.Equal(gymMember, booking.GymMember);
        Assert.NotNull(booking.Class);
        Assert.Equal(classEntity, booking.Class);
        Assert.NotNull(booking.ClassSession);
        Assert.Equal(classSession, booking.ClassSession);
    }

    [Fact]
    public void Booking_NavigationPropertiesCanBeNull()
    {
        // Arrange & Act
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            GymMemberId = Guid.NewGuid(),
            ClassId = Guid.NewGuid(),
            ClassSessionId = Guid.NewGuid()
        };

        // Assert
        Assert.Null(booking.GymMember);
        Assert.Null(booking.Class);
        Assert.Null(booking.ClassSession);
    }

    [Fact]
    public void Booking_MultipleBookings_ShouldHaveDifferentIds()
    {
        // Arrange & Act
        var booking1 = new Booking { Id = Guid.NewGuid() };
        var booking2 = new Booking { Id = Guid.NewGuid() };
        var booking3 = new Booking { Id = Guid.NewGuid() };

        // Assert
        Assert.NotEqual(booking1.Id, booking2.Id);
        Assert.NotEqual(booking1.Id, booking3.Id);
        Assert.NotEqual(booking2.Id, booking3.Id);
    }
}
