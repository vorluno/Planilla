namespace Vorluno.Planilla.Domain.Enums;

/// <summary>
/// Tipos de ausencias laborales
/// </summary>
public enum TipoAusencia
{
    /// <summary>
    /// Ausencia sin justificación - Afecta salario
    /// </summary>
    Injustificada = 1,

    /// <summary>
    /// Ausencia por enfermedad con certificado médico
    /// </summary>
    Enfermedad = 2,

    /// <summary>
    /// Permiso autorizado por supervisor
    /// </summary>
    Permiso = 3,

    /// <summary>
    /// Licencia legal (maternidad, paternidad, duelo)
    /// </summary>
    Licencia = 4,

    /// <summary>
    /// Suspensión disciplinaria
    /// </summary>
    Suspension = 5
}
