// ====================================================================
// Planilla - CssCalculationResult
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Resultado de cálculo CSS individual (empleado o empleador)
// ====================================================================

namespace Vorluno.Planilla.Application.Results;

/// <summary>
/// Resultado de un cálculo de aporte CSS individual.
/// Puede representar el aporte del empleado o del empleador.
/// </summary>
/// <param name="ContributionBase">Base de cotización utilizada (puede estar topeada)</param>
/// <param name="MaxContributionBase">Tope máximo aplicable (B/. 1,500 / 2,000 / 2,500)</param>
/// <param name="TipoTope">Tipo de tope aplicado: "Estándar", "Intermedio", "Alto"</param>
/// <param name="Rate">Tasa porcentual aplicada (ej: 9.75% para empleado)</param>
/// <param name="Amount">Monto calculado del aporte CSS</param>
public record CssCalculationResult(
    decimal ContributionBase,
    decimal MaxContributionBase,
    string TipoTope,
    decimal Rate,
    decimal Amount
);
