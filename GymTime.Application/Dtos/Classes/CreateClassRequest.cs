using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para criar uma nova aula.
/// </summary>
public class CreateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigatório, máximo 100 caracteres).
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e horário da aula (obrigatório). Use UTC para consistência.
    /// </summary>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade máxima (obrigatório, mínimo 1).
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}