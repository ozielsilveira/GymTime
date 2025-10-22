using System.ComponentModel.DataAnnotations;

namespace GymTime.Application.Dtos.Classes;

/// <summary>
/// Request para criar uma nova aula.
/// </summary>
public class CreateClassRequest
{
    /// <summary>
    /// Tipo/nome da aula (obrigat�rio, m�ximo100 caracteres).
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: valor �nico por hor�rio no dom�nio (n�o duplicar aulas do mesmo tipo no mesmo hor�rio) � essa valida��o � feita no servi�o/reposit�rio.
    /// </remarks>
    [Required]
    [StringLength(100)]
    public string ClassType { get; set; } = default!;

    /// <summary>
    /// Data e hor�rio da aula (obrigat�rio). Use UTC para consist�ncia.
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: n�o � permitido criar aulas no passado. Hor�rios conflitantes devem ser tratados pelo servi�o.
    /// </remarks>
    [Required]
    public DateTime Schedule { get; set; }

    /// <summary>
    /// Capacidade m�xima (obrigat�rio, m�nimo1).
    /// </summary>
    /// <remarks>
    /// Regras de neg�cio: maior que0. Limites superiores devem ser impostos conforme pol�tica da academia.
    /// </remarks>
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxCapacity { get; set; }
}