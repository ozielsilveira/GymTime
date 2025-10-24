using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Endpoints for managing class sessions.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassSessionController(IClassSessionService service) : ControllerBase
{
    private readonly IClassSessionService _service = service;

    /// <summary>
    /// Returns all class sessions.
    /// </summary>
    /// <returns>List of all class sessions.</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClassSessionDto>), 200)]
    public async Task<ActionResult<IEnumerable<ClassSessionDto>>> GetAll()
    {
        var sessions = await _service.GetAllAsync();
        return Ok(sessions);
    }

    /// <summary>
    /// Returns a specific class session by id.
    /// </summary>
    /// <param name="id">Session id (GUID).</param>
    /// <returns>Session details or 404 if not found.</returns>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassSessionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClassSessionDto>> GetById(Guid id)
    {
        var session = await _service.GetByIdAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        return Ok(session);
    }

    /// <summary>
    /// Returns all sessions for a specific class.
    /// </summary>
    /// <param name="classId">Class id (GUID).</param>
    /// <returns>List of sessions for the specified class.</returns>
    [HttpGet("by-class/{classId:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClassSessionDto>), 200)]
    public async Task<ActionResult<IEnumerable<ClassSessionDto>>> GetByClassId(Guid classId)
    {
        var sessions = await _service.GetByClassIdAsync(classId);
        return Ok(sessions);
    }

    /// <summary>
    /// Creates a new class session.
    /// </summary>
    /// <param name="request">Session data.</param>
    /// <returns>Created session details.</returns>
    /// <remarks>
    /// Business rules:
    /// - ClassId must exist.
    /// - EndTime must be after StartTime.
    /// - Date cannot be in the past.
    /// 
    /// Example request body:
    /// {
    ///   "classId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///   "date": "2024-02-15",
    ///   "startTime": "10:00:00",
    ///   "endTime": "11:00:00"
    /// }
    /// </remarks>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassSessionDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ClassSessionDto>> Create([FromBody] CreateClassSessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var session = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = session.Id }, session);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates a specific class session.
    /// </summary>
    /// <param name="id">Session id (GUID).</param>
    /// <param name="request">Session data to update.</param>
    /// <returns>No content if successful, 404 if not found, 400 if validation fails.</returns>
    /// <remarks>
    /// Business rules:
    /// - Cannot modify date/time if the session has active bookings.
    /// - EndTime must be after StartTime.
    /// 
    /// Example request body:
    /// {
    ///   "date": "2024-02-15",
    ///   "startTime": "16:00:00",
    ///   "endTime": "17:30:00"
    /// }
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassSessionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updated = await _service.UpdateAsync(id, request);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a specific class session.
    /// </summary>
    /// <param name="id">Session id (GUID).</param>
    /// <returns>No content if successful, 404 if not found, 400 if session has bookings.</returns>
    /// <remarks>
    /// Warning: Cannot delete a session if it has active bookings.
    /// All bookings must be cancelled before deleting the session.
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
