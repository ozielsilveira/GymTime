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
public class ClassesController(IClassService service) : ControllerBase
{
    private readonly IClassService _service = service;

    /// <summary>
    /// Returns all classes.
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
    /// Returns a class by id.
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ClassDto>> GetById(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new class.
    /// </summary>
    /// <param name="request">Class data to be created.</param>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ClassDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ClassDto>> Create([FromBody] CreateClassRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var dto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Updates an existing class.
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    /// <param name="request">Fields to be updated.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, request);
        if (!updated) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Removes a class by id.
    /// </summary>
    /// <param name="id">Class id (GUID).</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}