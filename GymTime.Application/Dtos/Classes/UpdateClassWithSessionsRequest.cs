using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request to update a class and its sessions.
/// </summary>
public class UpdateClassWithSessionsRequest
{
    /// <summary>
    /// Class type/name (required, maximum 100 characters).
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Maximum capacity (required, minimum 1).
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }

    /// <summary>
    /// Session IDs to be removed (optional).
    /// </summary>
    public List<Guid> SessionIdsToRemove { get; set; } = [];

    /// <summary>
    /// New sessions to be added (optional).
    /// </summary>
    public AddSessionsToClassRequest? NewSessions { get; set; }
}
