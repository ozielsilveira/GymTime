using GymTime.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.GymMembers
{
 /// <summary>
 /// Request to update a GymMember.
 /// </summary>
 public class UpdateGymMemberRequest
 {
 /// <summary>
 /// Gym member name (required, maximum100 characters).
 /// </summary>
 [Required]
 [StringLength(100)]
 public string Name { get; set; } = default!;

 /// <summary>
 /// Plan type (1 = Monthly,2 = Quarterly,3 = Annual).
 /// </summary>
 [Required]
 [EnumDataType(typeof(PlanType))]
 public PlanType PlanType { get; set; }
 }
}