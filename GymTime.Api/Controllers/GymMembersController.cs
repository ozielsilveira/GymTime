using GymTime.Application.Dtos.GymMembers;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Endpoints para gerenciar alunos (GymMember).
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GymMembersController(IGymMemberService service) : ControllerBase
{
    private readonly IGymMemberService _service = service;

    /// <summary>
    /// Retorna todos os alunos.
    /// </summary>
    /// <returns>Lista de alunos</returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<GymMemberDto>), 200)]
    public async Task<ActionResult<IEnumerable<GymMemberDto>>> GetAll()
    {
        var members = await _service.GetAllAsync();
        return Ok(members);
    }

    /// <summary>
    /// Retorna um aluno pelo id.
    /// </summary>
    /// <param name="id">Id do aluno (GUID)</param>
    /// <returns>Aluno encontrado</returns>
    [HttpGet("{id:guid}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GymMemberDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<GymMemberDto>> GetById(Guid id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Cria um novo aluno.
    /// </summary>
    /// <param name="request">Dados do aluno a ser criado</param>
    /// <returns>Aluno criado</returns>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GymMemberDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<GymMemberDto>> Create([FromBody] CreateGymMemberRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var dto = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Atualiza um aluno existente.
    /// </summary>
    /// <param name="id">Id do aluno (GUID)</param>
    /// <param name="request">Campos que serão atualizados</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGymMemberRequest request)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var updated = await _service.UpdateAsync(id, request);
        if (!updated) return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Remove um aluno pelo id.
    /// </summary>
    /// <param name="id">Id do aluno (GUID)</param>
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