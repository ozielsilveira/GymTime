using GymTime.Domain.Entities;
using GymTime.Domain.Enums;

namespace GymTime.Domain.Tests.Entities;

public class GymMemberTests
{
    [Fact]
    public void GetMonthlyBookingLimit_MonthlyPlan_Returns12()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Monthly
        };

        // Act
        var limit = gymMember.GetMonthlyBookingLimit();

        // Assert
        Assert.Equal(12, limit);
    }

    [Fact]
    public void GetMonthlyBookingLimit_QuarterlyPlan_Returns20()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Quarterly
        };

        // Act
        var limit = gymMember.GetMonthlyBookingLimit();

        // Assert
        Assert.Equal(20, limit);
    }

    [Fact]
    public void GetMonthlyBookingLimit_AnnualPlan_Returns30()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Annual
        };

        // Act
        var limit = gymMember.GetMonthlyBookingLimit();

        // Assert
        Assert.Equal(30, limit);
    }

    [Fact]
    public void CanBook_BelowLimit_ReturnsTrue()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Monthly // 12 bookings limit
        };

        // Act
        var canBook = gymMember.CanBook(11);

        // Assert
        Assert.True(canBook);
    }

    [Fact]
    public void CanBook_AtLimit_ReturnsFalse()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Monthly // 12 bookings limit
        };

        // Act
        var canBook = gymMember.CanBook(12);

        // Assert
        Assert.False(canBook);
    }

    [Fact]
    public void CanBook_AboveLimit_ReturnsFalse()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Monthly // 12 bookings limit
        };

        // Act
        var canBook = gymMember.CanBook(13);

        // Assert
        Assert.False(canBook);
    }

    [Fact]
    public void CanBook_ZeroBookings_ReturnsTrue()
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            PlanType = PlanType.Monthly
        };

        // Act
        var canBook = gymMember.CanBook(0);

        // Assert
        Assert.True(canBook);
    }

    [Theory]
    [InlineData(PlanType.Monthly, 11, true)]
    [InlineData(PlanType.Monthly, 12, false)]
    [InlineData(PlanType.Quarterly, 19, true)]
    [InlineData(PlanType.Quarterly, 20, false)]
    [InlineData(PlanType.Annual, 29, true)]
    [InlineData(PlanType.Annual, 30, false)]
    public void CanBook_VariousScenarios_ReturnsExpected(PlanType planType, int currentBookings, bool expected)
    {
        // Arrange
        var gymMember = new GymMember
        {
            Id = Guid.NewGuid(),
            Name = "Test Member",
            PlanType = planType
        };

        // Act
        var result = gymMember.CanBook(currentBookings);

        // Assert
        Assert.Equal(expected, result);
    }
}
