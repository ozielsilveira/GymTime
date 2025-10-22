using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para criar uma nova aula.
/// </summary>
public class CreateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigatório, máximo100 caracteres).
    /// </summary>
    /// <remarks>
    /// Regras de negócio: valor único por horário no domÍnio (não duplicar aulas do mesmo tipo no mesmo horário) — essa validação é feita no serviço/repositório.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e horário da aula (obrigatório). Use UTC para consistência.
    /// </summary>
    /// <remarks>
    /// Regras de negócio: não é permitido criar aulas no passado. Horários conflitantes devem ser tratados pelo serviço.
    /// </remarks>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade máxima (obrigatório, mínimo1).
    /// </summary>
    /// <remarks>
    /// Regras de negócio: maior que0. Limites superiores devem ser impostos conforme política da academia.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}