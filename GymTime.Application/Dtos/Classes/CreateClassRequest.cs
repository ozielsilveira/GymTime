using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para criar uma nova aula.
/// </summary>
public class CreateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigat�rio, m�ximo 100 caracteres).
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e hor�rio da aula (obrigat�rio). Use UTC para consist�ncia.
    /// </summary>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade m�xima (obrigat�rio, m�nimo 1).
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}