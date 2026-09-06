// ====================================================================
// Planilla - IAcumuladoFiscalService
// Reúne lo que un empleado lleva acumulado en el año fiscal para que el
// motor de ISR pueda calcular por el método acumulativo.
// ====================================================================

using Vorluno.Planilla.Application.Services;

namespace Vorluno.Planilla.Application.Interfaces;

/// <summary>
/// Provee el acumulado fiscal del año de un empleado y el número de corrida
/// que le corresponde, que son las dos entradas de estado del motor de ISR.
/// </summary>
public interface IAcumuladoFiscalService
{
    /// <summary>
    /// Acumulado del empleado en el año: los saldos cargados al migrar más lo
    /// que ya generó esta plataforma.
    ///
    /// Lo procesado se DERIVA de las planillas guardadas, no de un contador que
    /// se va sumando. Así un recálculo de la misma planilla no duplica nada y
    /// anular una planilla la saca del acumulado sin necesidad de revertir.
    /// </summary>
    /// <param name="empleadoId">Empleado.</param>
    /// <param name="anio">Año fiscal.</param>
    /// <param name="excluirPayrollHeaderId">
    /// Planilla regular que se está calculando ahora mismo; se excluye para que
    /// un recálculo no se cuente a sí mismo.
    /// </param>
    /// <param name="excluirPlanillaDecimoId">Ídem para una corrida de décimo.</param>
    Task<AcumuladoIsr> ObtenerAcumuladoAsync(
        int empleadoId,
        int anio,
        int? excluirPayrollHeaderId = null,
        int? excluirPlanillaDecimoId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Número de corrida del empleado dentro del año (1, 2, 3…): las planillas
    /// regulares que ya tiene más la que se está calculando.
    ///
    /// Cuenta las corridas del EMPLEADO, no las de la empresa: quien entró a
    /// mitad de año va por su período 1 aunque la empresa vaya por el 14.
    /// </summary>
    Task<int> ObtenerNumeroPeriodoAsync(
        int empleadoId,
        int anio,
        int? excluirPayrollHeaderId = null,
        CancellationToken cancellationToken = default);
}
