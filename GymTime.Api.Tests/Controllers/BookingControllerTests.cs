using GymTime.Api.Controllers;
using GymTime.Application.Dtos.Bookings;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Tests.Controllers;

public class BookingControllerTests
{
    private class FakeBookingService : IBookingService
    {
        public Guid? LastGymMemberId { get; private set; }
        public Guid? LastClassId { get; private set; }
        public Guid? LastBookingId { get; private set; }

        public Task<string> BookClassAsync(Guid gymMemberId, Guid classId)
        {
            LastGymMemberId = gymMemberId;
            LastClassId = classId;
            return Task.FromResult("Booked");
        }

        public Task<string> CancelBookingAsync(Guid bookingId)
        {
            LastBookingId = bookingId;
            return Task.FromResult("Cancelled");
        }

        public Task<IEnumerable<BookingDto>> GetBookingsForGymMemberAsync(Guid gymMemberId)
        {
            LastGymMemberId = gymMemberId;
            return Task.FromResult<IEnumerable<BookingDto>>(new List<BookingDto>());
        }

        public Task<IEnumerable<BookingDto>> GetBookingsForClassAsync(Guid classId)
        {
            LastClassId = classId;
            return Task.FromResult<IEnumerable<BookingDto>>(new List<BookingDto>());
        }

        public Task<BookingDto?> GetBookingByIdAsync(Guid bookingId)
        {
            LastBookingId = bookingId;
            return Task.FromResult<BookingDto?>(null);
        }

        public Task<IEnumerable<BookingDto>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<BookingDto>>(new List<BookingDto>());
        }
    }

    [Fact]
    public async Task Book_ReturnsOkAndCallsService()
    {
        var fakeService = new FakeBookingService();
        var controller = new BookingController(fakeService);

        var gymMemberId = Guid.NewGuid();
        var classId = Guid.NewGuid();

        var result = await controller.Book(gymMemberId, classId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Booked", ok.Value);
        Assert.Equal(gymMemberId, fakeService.LastGymMemberId);
        Assert.Equal(classId, fakeService.LastClassId);
    }

    [Fact]
    public async Task Cancel_ReturnsOkAndCallsService()
    {
        var fakeService = new FakeBookingService();
        var controller = new BookingController(fakeService);

        var bookingId = Guid.NewGuid();

        var result = await controller.Cancel(bookingId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Cancelled", ok.Value);
        Assert.Equal(bookingId, fakeService.LastBookingId);
    }
}
