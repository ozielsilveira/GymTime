using GymTime.Domain.Enums;

namespace GymTime.Application.Dtos.GymMembers;

/// <summary>
/// Response DTO for GymMember.
/// </summary>
public record GymMemberDto
{
    /// <summary>
    /// Gym member identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gym member name.
    /// </summary>
    /// <remarks>
    /// Full name of the gym member. Must follow the same rules as the request (not empty, maximum length).
    /// </remarks>
    public string Name { get; init; } = default!;

    /// <summary>
    /// Gym member plan type.
    /// </summary>
    /// <remarks>
    /// Values: Monthly(1), Quarterly(2), Annual(3). Influences the monthly booking limit.
    /// </remarks>
    public PlanType PlanType { get; init; }
}
