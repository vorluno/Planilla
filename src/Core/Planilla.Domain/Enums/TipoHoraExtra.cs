namespace Vorluno.Planilla.Domain.Enums;

/// <summary>
/// Tipos de horas extra según el Código de Trabajo de Panamá
/// </summary>
public enum TipoHoraExtra
{
    /// <summary>
    /// Hora extra diurna - Factor 1.25x (6am-6pm días normales)
    /// </summary>
    Diurna = 1,

    /// <summary>
    /// Hora extra nocturna - Factor 1.50x (6pm-6am)
    /// </summary>
    Nocturna = 2,

    /// <summary>
    /// Hora extra en domingo o feriado (diurna) - Factor 1.50x
    /// </summary>
    DomingoFeriado = 3,

    /// <summary>
    /// Hora extra nocturna en domingo o feriado - Factor 1.75x
    /// </summary>
    NocturnaDomingoFeriado = 4
}
