using GymTime.Domain.Enums;

namespace GymTime.Application.Dtos.GymMembers
{
    /// <summary>
    /// Response DTO for GymMember.
    /// </summary>
    public record GymMemberDto
    {
        /// <summary>
        /// Student identifier.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Student name.
        /// </summary>
        public string Name { get; init; } = default!;

        /// <summary>
        /// Student plan type.
        /// </summary>
        public PlanType PlanType { get; init; }
    }
}