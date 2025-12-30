// ====================================================================
// Planilla - PayrollStatus
// Source: Core360 Stage 3
// Creado: 2025-12-26
// Descripción: Estados del workflow de planilla
// ====================================================================

namespace Vorluno.Planilla.Domain.Enums;

/// <summary>
/// Estados posibles de una planilla en el workflow de procesamiento.
/// Transiciones válidas:
/// - Draft → Calculated (CALCULATE)
/// - Calculated → Calculated (RE-CALCULATE)
/// - Calculated → Approved (APPROVE)
/// - Approved → Paid (PAY)
/// - Draft/Calculated/Approved → Cancelled (CANCEL)
/// </summary>
public enum PayrollStatus
{
    /// <summary>
    /// Planilla en borrador, sin cálculos.
    /// Estado inicial al crear una planilla.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Planilla calculada, con deducciones y netos.
    /// Puede re-calcularse si hay cambios.
    /// </summary>
    Calculated = 1,

    /// <summary>
    /// Planilla aprobada, lista para pago.
    /// No puede modificarse ni re-calcularse.
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Planilla pagada y cerrada.
    /// Estado final, no reversible.
    /// </summary>
    Paid = 3,

    /// <summary>
    /// Planilla cancelada.
    /// No puede procesarse ni pagarse.
    /// </summary>
    Cancelled = 4
}
