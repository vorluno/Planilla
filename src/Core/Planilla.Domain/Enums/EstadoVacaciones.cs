namespace Vorluno.Planilla.Domain.Enums;

/// <summary>
/// Estados de una solicitud de vacaciones
/// </summary>
public enum EstadoVacaciones
{
    /// <summary>
    /// Solicitud creada, pendiente de aprobación
    /// </summary>
    Pendiente = 1,

    /// <summary>
    /// Solicitud aprobada por supervisor
    /// </summary>
    Aprobada = 2,

    /// <summary>
    /// Empleado actualmente en vacaciones
    /// </summary>
    EnCurso = 3,

    /// <summary>
    /// Vacaciones completadas
    /// </summary>
    Completada = 4,

    /// <summary>
    /// Solicitud cancelada antes de iniciar
    /// </summary>
    Cancelada = 5,

    /// <summary>
    /// Solicitud rechazada por supervisor
    /// </summary>
    Rechazada = 6
}
