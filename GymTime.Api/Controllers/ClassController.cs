using GymTime.Application.Dtos.Classes;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Endpoints for managing classes.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClassController(IClassService service) : ControllerBase
{
  private readonly IClassService _service = service;

    /// <summary>
    /// Returns all classes with their sessions.
    /// </summary>
    [HttpGet]
 [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<ClassDto>), 200)]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetAll()
    {
   var items = await _service.GetAllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Returns a class by id with all its sessions.
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    /// <returns>Class details with sessions or 404 if not found.</returns>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClassDto>> GetById(Guid id)
  {
   var dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
        return NotFound();
        }

        return Ok(dto);
    }

    /// <summary>
    /// Creates a new class and automatically generates sessions based on the provided schedule.
    /// </summary>
    /// <param name="request">Class data and session generation parameters.</param>
    /// <remarks>
    /// Business rules:
    /// - ClassType is required and max length 100.
    /// - MaxCapacity must be >=1.
    /// - Sessions are automatically created based on StartDate, EndDate, StartTime, EndTime and DaysOfWeek.
    /// - DaysOfWeek: 0=Sunday, 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday.
    /// 
    /// Example request body:
    /// {
    ///   "classType": "Yoga",
    ///   "maxCapacity": 20,
    ///   "startDate": "2024-01-22",
    ///   "endDate": "2024-01-31",
    ///   "startTime": "10:00:00",
    ///   "endTime": "11:00:00",
    ///   "daysOfWeek": [1, 3, 5]
    /// }
    /// 
    /// This will create Yoga sessions every Monday, Wednesday and Friday from Jan 22 to Jan 31,
    /// from 10:00 to 11:00.
    /// </remarks>
    [HttpPost]
  [Produces("application/json")]
    [ProducesResponseType(typeof(ClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
         var dto = await _service.CreateAsync(request);
   return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
       return BadRequest(new { message = ex.Message });
   }
    }

    /// <summary>
    /// Updates an existing class (does not affect sessions).
    /// </summary>
  /// <param name="id">Class id (GUID).</param>
    /// <param name="request">Fields to be updated.</param>
    /// <remarks>
    /// Business rules:
    /// - You cannot reduce MaxCapacity below the current number of bookings in any session.
    /// - To modify sessions, use PUT /api/class/{id}/update-with-sessions or the ClassSession endpoints.
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request)
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
  /// Updates a class and manages its sessions (add/remove).
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    /// <param name="request">Class data and session management operations.</param>
    /// <remarks>
    /// This endpoint allows you to:
    /// - Update class type and max capacity
    /// - Remove specific sessions (only if they have no bookings)
    /// - Add new sessions with specified schedule
    /// 
    /// Example request body:
  /// {
///   "classType": "Advanced Yoga",
  ///   "maxCapacity": 15,
    ///   "sessionIdsToRemove": ["session-guid-1", "session-guid-2"],
    ///   "newSessions": {
    ///     "startDate": "2024-02-01",
    ///     "endDate": "2024-02-29",
    ///     "startTime": "19:00:00",
    ///     "endTime": "20:00:00",
    ///     "daysOfWeek": [1, 3, 5]
    ///   }
    /// }
    /// </remarks>
    [HttpPut("{id:guid}/update-with-sessions")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClassDto>> UpdateWithSessions(Guid id, [FromBody] UpdateClassWithSessionsRequest request)
    {
 if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
      var updatedClass = await _service.UpdateWithSessionsAsync(id, request);
          return Ok(updatedClass);
        }
 catch (InvalidOperationException ex)
        {
 return BadRequest(new { message = ex.Message });
        }
    }

 /// <summary>
  /// Removes a class by id.
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    /// <remarks>
    /// Business rules:
    /// - Cannot delete a class if any session has active bookings.
    /// - All bookings must be cancelled before deleting the class.
    /// - Removing a class will also remove all associated sessions (cascade delete).
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
