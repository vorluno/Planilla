namespace Vorluno.Planilla.Domain.Enums;

/// <summary>
/// Estados posibles de un préstamo.
/// </summary>
public enum EstadoPrestamo
{
    /// <summary>
    /// Préstamo activo con descuentos en curso
    /// </summary>
    Activo = 1,

    /// <summary>
    /// Préstamo completamente pagado
    /// </summary>
    Pagado = 2,

    /// <summary>
    /// Préstamo cancelado antes de completar pagos
    /// </summary>
    Cancelado = 3,

    /// <summary>
    /// Préstamo suspendido temporalmente (no se descuenta)
    /// </summary>
    Suspendido = 4
}
