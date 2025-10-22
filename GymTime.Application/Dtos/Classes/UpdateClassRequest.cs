using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para atualizar uma aula existente.
/// </summary>
public class UpdateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigatório, máximo 100 caracteres).
    /// </summary>
    /// <remarks>
    /// Regras de negócio: alteração do tipo deve considerar histórico de bookings; evite renomear aulas com bookings existentes sem validação adicional.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e horário da aula (obrigatório). Use UTC para consistência.
    /// </summary>
    /// <remarks>
    /// Regras de negócio: alterar para uma data passada não é permitido; mudança de horário que cause conflito com outras aulas deve ser validada no serviço.
    /// </remarks>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade máxima (obrigatório, mínimo 1).
    /// </summary>
    /// <remarks>
    /// Regras de negócio: reduzir a capacidade abaixo do número atual de bookings não é permitido sem cancelamentos prévios.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}