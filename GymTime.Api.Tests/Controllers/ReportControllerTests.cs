using GymTime.Api.Controllers;
using GymTime.Application.Dtos.Report;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Tests.Controllers;

public class ReportControllerTests
{
    private class FakeReportService : IReportService
    {
        public Guid? LastGymMemberId { get; private set; }
        public ReportDto? Return { get; set; }
        public Task<ReportDto?> GetGymMemberReportAsync(Guid gymMemberId)
        {
            LastGymMemberId = gymMemberId;
            return Task.FromResult(Return);
        }
    }

    [Fact]
    public async Task GetGymMemberReport_ReturnsOk_WhenFound()
    {
        var fake = new FakeReportService();
        var dto = new ReportDto { GymMemberId = Guid.NewGuid(), GymMemberName = "Tester", PlanType = "Monthly", TotalBookingsThisMonth = 3, FavoriteClassTypes = ["Yoga"] };
        fake.Return = dto;
        var controller = new ReportController(fake);

        var result = await controller.GetGymMemberReport(dto.GymMemberId);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
        Assert.Equal(dto.GymMemberId, fake.LastGymMemberId);
    }

    [Fact]
    public async Task GetGymMemberReport_ReturnsNotFound_WhenMissing()
    {
        var fake = new FakeReportService
        {
            Return = null
        };
        var controller = new ReportController(fake);

        var id = Guid.NewGuid();
        var result = await controller.GetGymMemberReport(id);
        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Gym member not found.", GetMessageFromValue(notFound.Value));
        Assert.Equal(id, fake.LastGymMemberId);
    }

    private static string? GetMessageFromValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string s)
        {
            return s;
        }

        var type = value.GetType();
        var prop = type.GetProperty("message") ?? type.GetProperty("Message");
        if (prop != null)
        {
            return prop.GetValue(value)?.ToString();
        }

        return value.ToString();
    }
}
