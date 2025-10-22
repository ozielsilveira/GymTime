using GymTime.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.GymMembers
{
    /// <summary>
    /// Request para atualizar GymMember.
    /// </summary>
    public class UpdateGymMemberRequest
    {
        /// <summary>
        /// Nome do aluno (obrigatório, máximo 100 caracteres).
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Tipo de plano (1 = Monthly, 2 = Quarterly, 3 = Annual).
        /// </summary>
        [Required]
        [EnumDataType(typeof(PlanType))]
        public PlanType PlanType { get; set; }
    }
}