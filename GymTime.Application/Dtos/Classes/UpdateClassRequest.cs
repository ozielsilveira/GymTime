using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para atualizar uma aula existente.
/// </summary>
public class UpdateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigat�rio, m�ximo 100 caracteres).
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: altera��o do tipo deve considerar hist�rico de bookings; evite renomear aulas com bookings existentes sem valida��o adicional.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e hor�rio da aula (obrigat�rio). Use UTC para consist�ncia.
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: alterar para uma data passada n�o � permitido; mudan�a de hor�rio que cause conflito com outras aulas deve ser validada no servi�o.
    /// </remarks>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade m�xima (obrigat�rio, m�nimo 1).
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: reduzir a capacidade abaixo do n�mero atual de bookings n�o � permitido sem cancelamentos pr�vios.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}