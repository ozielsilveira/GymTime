using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GymTime.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    /// <summary>
    /// Books a class for the specified student.
    /// </summary>
    /// <param name="gymMemberId">Student id (GUID) — required.</param>
    /// <param name="classId">Class id (GUID) — required.</param>
    /// <returns>Message indicating the result of the operation (string returned by BookingService).</returns>
    /// <response code="200">Booking successful. Returns a message (string).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">Student or class not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("{gymMemberId:guid}/{classId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Book(
        [FromRoute(Name = "gymMemberId")][Required(ErrorMessage = "gymMemberId is required")] Guid gymMemberId,
        [FromRoute(Name = "classId")][Required(ErrorMessage = "classId is required")] Guid classId)
    {
        var result = await _bookingService.BookClassAsync(gymMemberId, classId);
        return Ok(result);
    }

    /// <summary>
    /// Cancels an existing booking.
    /// </summary>
    /// <param name="bookingId">Booking id (GUID) — required.</param>
    /// <returns>Message indicating the result of the operation (string returned by BookingService).</returns>
    /// <response code="200">Cancellation successful. Returns a message (string).</response>
    /// <response code="400">Invalid parameters (e.g.: empty GUID or invalid format).</response>
    /// <response code="404">Booking not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{bookingId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel(Guid bookingId)
    {
        var result = await _bookingService.CancelBookingAsync(bookingId);
        return Ok(result);
    }
}