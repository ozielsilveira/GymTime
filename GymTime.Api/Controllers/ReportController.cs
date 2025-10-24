using GymTime.Application.Dtos.Common;
using GymTime.Application.Dtos.Report;
using GymTime.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Endpoints for generating and retrieving reports related to gym members.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    /// <summary>
    /// Returns the report for the gym member identified by the provided GUID.
    /// </summary>
    /// <param name="gymMemberId">Unique identifier of the gym member (GUID). Must be provided in the route.</param>
    /// <returns>
    /// 200 — Returns the <see cref="ReportDto"/> object with the report data;
    /// 404 — When the gym member is not found;
    /// 400 — Invalid request (for example invalid GUID);
    /// 500 — Internal server error.
    /// </returns>
    [HttpGet("{gymMemberId}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponseDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResponseDto))]
    public async Task<ActionResult<ReportDto>> GetGymMemberReport([FromRoute] Guid gymMemberId)
    {
        var report = await _reportService.GetGymMemberReportAsync(gymMemberId);

        if (report == null)
        {
            return NotFound(new ErrorResponseDto { Message = "Gym member not found." });
        }

        return Ok(report);
    }
}
