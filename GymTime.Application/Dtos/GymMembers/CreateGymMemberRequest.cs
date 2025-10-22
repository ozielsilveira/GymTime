using GymTime.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.GymMembers
{
    /// <summary>
    /// Request to create a GymMember.
    /// </summary>
    public class CreateGymMemberRequest
    {
        /// <summary>
        /// Gym member name (required, maximum 100 characters).
        /// </summary>
        /// <remarks>
        /// Business rules: must not contain only whitespace; length must be between 1 and 100 characters.
        /// </remarks>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Plan type (1 = Monthly, 2 = Quarterly, 3 = Annual).
        /// </summary>
        /// <remarks>
        /// Business rules: the plan type determines the monthly booking limit (Monthly = 12, Quarterly = 20, Annual = 30).
        /// </remarks>
        [Required]
        [EnumDataType(typeof(PlanType))]
        public PlanType PlanType { get; set; }
    }
}