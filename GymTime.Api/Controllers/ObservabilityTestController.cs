using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymTime.Api.Controllers;

/// <summary>
/// Controller for testing observability features
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ObservabilityTestController(ILogger<ObservabilityTestController> logger) : ControllerBase
{
    private readonly ILogger<ObservabilityTestController> _logger = logger;

    /// <summary>
    /// Generates various log levels for testing
    /// </summary>
    [HttpGet("test-logs")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public IActionResult TestLogs()
    {
        _logger.LogTrace("This is a TRACE log message");
        _logger.LogDebug("This is a DEBUG log message");
        _logger.LogInformation("This is an INFORMATION log message");
        _logger.LogWarning("This is a WARNING log message");
        _logger.LogError("This is an ERROR log message");
        _logger.LogCritical("This is a CRITICAL log message");

        _logger.LogInformation("Test logs generated successfully at {Timestamp}", DateTime.UtcNow);

        return Ok(new
        {
            message = "Logs generated successfully",
            timestamp = DateTime.UtcNow,
            logLevels = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" }
        });
    }

    /// <summary>
    /// Simulates a slow request for testing performance metrics
    /// </summary>
    [HttpGet("slow-request")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SlowRequest([FromQuery] int delayMs = 1000)
    {
        _logger.LogInformation("Slow request started with delay of {DelayMs}ms", delayMs);

        await Task.Delay(delayMs);

        _logger.LogInformation("Slow request completed after {DelayMs}ms", delayMs);

        return Ok(new
        {
            message = "Request completed",
            delayMs,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Simulates an error for testing error logging
    /// </summary>
    [HttpGet("simulate-error")]
    [AllowAnonymous]
    [ProducesResponseType(500)]
    public IActionResult SimulateError()
    {
        _logger.LogWarning("Simulating an error...");

        try
        {
            throw new InvalidOperationException("This is a simulated error for testing observability");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Simulated error occurred: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Generates structured log with custom properties
    /// </summary>
    [HttpPost("structured-log")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public IActionResult StructuredLog([FromBody] StructuredLogRequest request)
    {
        _logger.LogInformation(
            "Structured log: User {UserId} performed action {Action} with result {Result}",
            request.UserId,
       request.Action,
       request.Result);

        return Ok(new
        {
            message = "Structured log created",
            userId = request.UserId,
            action = request.Action,
            result = request.Result,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Returns current system metrics
    /// </summary>
    [HttpGet("system-metrics")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    public IActionResult GetSystemMetrics()
    {
        var process = System.Diagnostics.Process.GetCurrentProcess();

        var metrics = new
        {
            timestamp = DateTime.UtcNow,
            memory = new
            {
                workingSetBytes = process.WorkingSet64,
                privateMemoryBytes = process.PrivateMemorySize64,
                virtualMemoryBytes = process.VirtualMemorySize64
            },
            cpu = new
            {
                totalProcessorTime = process.TotalProcessorTime.TotalSeconds,
                userProcessorTime = process.UserProcessorTime.TotalSeconds
            },
            threads = process.Threads.Count,
            handles = process.HandleCount
        };

        _logger.LogInformation(
              "System metrics: Memory={WorkingSetMB}MB, Threads={Threads}, Handles={Handles}",
      metrics.memory.workingSetBytes / 1024 / 1024,
      metrics.threads,
      metrics.handles);

        return Ok(metrics);
    }
}

/// <summary>
/// Request model for structured log endpoint
/// </summary>
public class StructuredLogRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
}
