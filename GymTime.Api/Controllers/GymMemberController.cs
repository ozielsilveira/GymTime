using GymTime.Application.Dtos.GymMembers;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Endpoints to manage gym members (GymMember).
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GymMemberController(IGymMemberService service) : ControllerBase
{
    private readonly IGymMemberService _service = service;

    /// <summary>
    /// Returns all gym members.
    /// </summary>
    /// <returns>List of gym members</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GymMemberDto>), 200)]
    public async Task<ActionResult<IEnumerable<GymMemberDto>>> GetAll()
    {
        var members = await _service.GetAllAsync();
        return Ok(members);
    }

    /// <summary>
    /// Returns a gym member by id.
    /// </summary>
    /// <param name="id">Gym member id (GUID)</param>
    /// <returns>The found gym member</returns>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GymMemberDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GymMemberDto>> GetById(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null)
        {
            return NotFound();
        }

        return Ok(dto);
    }

    /// <summary>
    /// Creates a new gym member.
    /// </summary>
    /// <param name="request">Gym member data to create</param>
    /// <remarks>
    /// Business rules:
    /// - The Name field is required and must be at most 100 characters.
    /// - The PlanType field must be one of the enum values: Monthly (1), Quarterly (2) or Annual (3).
    /// </remarks>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GymMemberDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<GymMemberDto>> Create([FromBody] CreateGymMemberRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Updates an existing gym member.
    /// </summary>
    /// <param name="id">Gym member id (GUID)</param>
    /// <param name="request">Fields to update</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGymMemberRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _service.UpdateAsync(id, request);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a gym member by id.
    /// </summary>
    /// <param name="id">Gym member id (GUID)</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
