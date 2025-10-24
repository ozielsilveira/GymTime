using System.ComponentModel.DataAnnotations;
using GymTime.Application.Dtos.Bookings;
using GymTime.Application.Dtos.Common;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    /// <summary>
    /// Books a class session for the specified student.
    /// </summary>
    /// <param name="gymMemberId">Student id (GUID) — required.</param>
    /// <param name="classSessionId">Class session id (GUID) — required.</param>
    /// <returns>Message indicating the result of the operation (string returned by BookingService).</returns>
    /// <remarks>
    /// Business rules:
    /// - Student must exist.
    /// - Class session must exist and have available capacity.
    /// - A student cannot book the same class session more than once.
    /// - Student cannot exceed the monthly booking limit determined by their plan (Monthly=12, Quarterly=20, Annual=30).
    /// - Bookings are counted using UTC month/year.
    /// </remarks>
    /// <response code="200">Booking successful or a descriptive message explaining why it failed (e.g., class session full, booking limit reached, duplicate booking).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">Student or class session not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("{gymMemberId:guid}/{classSessionId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDto))]
    public async Task<IActionResult> Book(
    [FromRoute(Name = "gymMemberId")][Required(ErrorMessage = "gymMemberId is required")] Guid gymMemberId,
    [FromRoute(Name = "classSessionId")][Required(ErrorMessage = "classSessionId is required")] Guid classSessionId)
    {
        var result = await _bookingService.BookClassAsync(gymMemberId, classSessionId);
        return Ok(result);
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="bookingId">Booking id (GUID) — required.</param>
    /// <returns>Message indicating the result of the operation (string returned by BookingService).</returns>
    /// <remarks>
    /// Business rules:
    /// - Booking must exist.
    /// - Cancellation immediately frees a slot in the class for other students.
    /// </remarks>
    /// <response code="200">Cancellation successful. Returns a message (string).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">Booking not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{bookingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDto))]
    public async Task<IActionResult> Cancel(Guid bookingId)
    {
        var result = await _bookingService.CancelBookingAsync(bookingId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves bookings for a specific gym member.
    /// </summary>
    /// <param name="gymMemberId">Student id (GUID) — required.</param>
    /// <returns>List of bookings for the specified gym member.</returns>
    /// <response code="200">Returns the list of bookings (array of objects).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">No bookings found for the specified gym member.</response>
    [HttpGet("member/{gymMemberId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    public async Task<IActionResult> GetByMember([FromRoute][Required] Guid gymMemberId)
    {
        var bookings = await _bookingService.GetBookingsForGymMemberAsync(gymMemberId);
        if (bookings == null || !bookings.Any())
        {
            return NotFound(new ErrorResponseDto { Message = "No bookings found for the specified gym member." });
        }

        return Ok(bookings);
    }

    /// <summary>
    /// Retrieves bookings for a specific class.
    /// </summary>
    /// <param name="classId">Class id (GUID) — required.</param>
    /// <returns>List of bookings for the specified class.</returns>
    /// <response code="200">Returns the list of bookings (array of objects).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">No bookings found for the specified class.</response>
    [HttpGet("class/{classId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    public async Task<IActionResult> GetByClass([FromRoute][Required] Guid classId)
    {
        var bookings = await _bookingService.GetBookingsForClassAsync(classId);
        if (bookings == null || !bookings.Any())
        {
            return NotFound(new ErrorResponseDto { Message = "No bookings found for the specified class." });
        }

        return Ok(bookings);
    }

    /// <summary>
    /// Retrieves details of a specific booking.
    /// </summary>
    /// <param name="bookingId">Booking id (GUID) — required.</param>
    /// <returns>Details of the specified booking.</returns>
    /// <response code="200">Returns the booking details (object).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">Booking not found.</response>
    [HttpGet("{bookingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookingDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    public async Task<IActionResult> GetById([FromRoute][Required] Guid bookingId)
    {
        var booking = await _bookingService.GetBookingByIdAsync(bookingId);
        if (booking == null)
        {
            return NotFound(new ErrorResponseDto { Message = "Booking not found." });
        }

        return Ok(booking);
    }

    /// <summary>
    /// Retrieves all bookings.
    /// </summary>
    /// <returns>List of all bookings.</returns>
    /// <response code="200">Returns list of bookings.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BookingDto>))]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _bookingService.GetAllAsync();
        return Ok(bookings);
    }
}
