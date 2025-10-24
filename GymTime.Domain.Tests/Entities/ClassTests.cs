using GymTime.Domain.Entities;

namespace GymTime.Domain.Tests.Entities;

public class ClassTests
{
    [Fact]
    public void Class_Initialize_ShouldHaveEmptyCollections()
    {
        // Arrange & Act
        var classEntity = new Class();

        // Assert
        Assert.NotNull(classEntity.Sessions);
        Assert.Empty(classEntity.Sessions);
        Assert.NotNull(classEntity.Bookings);
        Assert.Empty(classEntity.Bookings);
    }

    [Fact]
    public void Class_SetProperties_ShouldSetCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var classType = "Yoga";
        var maxCapacity = 20;

        // Act
        var classEntity = new Class
        {
            Id = id,
            ClassType = classType,
            MaxCapacity = maxCapacity
        };

        // Assert
        Assert.Equal(id, classEntity.Id);
        Assert.Equal(classType, classEntity.ClassType);
        Assert.Equal(maxCapacity, classEntity.MaxCapacity);
    }

    [Fact]
    public void Class_AddSessions_ShouldAddToCollection()
    {
        // Arrange
        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            ClassType = "Pilates",
            MaxCapacity = 15
        };

        var session1 = new ClassSession { Id = Guid.NewGuid(), ClassId = classEntity.Id };
        var session2 = new ClassSession { Id = Guid.NewGuid(), ClassId = classEntity.Id };

        // Act
        classEntity.Sessions.Add(session1);
        classEntity.Sessions.Add(session2);

        // Assert
        Assert.Equal(2, classEntity.Sessions.Count);
        Assert.Contains(session1, classEntity.Sessions);
        Assert.Contains(session2, classEntity.Sessions);
    }

    [Fact]
    public void Class_AddBookings_ShouldAddToCollection()
    {
        // Arrange
        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            ClassType = "CrossFit",
            MaxCapacity = 10
        };

        var booking1 = new Booking { Id = Guid.NewGuid(), ClassId = classEntity.Id };
        var booking2 = new Booking { Id = Guid.NewGuid(), ClassId = classEntity.Id };

        // Act
        classEntity.Bookings.Add(booking1);
        classEntity.Bookings.Add(booking2);

        // Assert
        Assert.Equal(2, classEntity.Bookings.Count);
        Assert.Contains(booking1, classEntity.Bookings);
        Assert.Contains(booking2, classEntity.Bookings);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public void Class_SetMaxCapacity_ShouldAcceptVariousValues(int capacity)
    {
        // Arrange & Act
        var classEntity = new Class
        {
            Id = Guid.NewGuid(),
            ClassType = "Spinning",
            MaxCapacity = capacity
        };

        // Assert
        Assert.Equal(capacity, classEntity.MaxCapacity);
    }
}
