// ====================================================================
// Planilla - CssFullCalculationResult
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Resultado completo de cálculo CSS (empleado + empleador + riesgo)
// ====================================================================

namespace Vorluno.Planilla.Application.Results;

/// <summary>
/// Resultado completo del cálculo de aportes CSS.
/// Incluye contribución del empleado, empleador y riesgo profesional.
/// </summary>
/// <param name="EmployeeCss">Aporte CSS del empleado (deducción)</param>
/// <param name="EmployerCss">Aporte CSS base del empleador (costo patronal)</param>
/// <param name="RiskContribution">Aporte de riesgo profesional del empleador</param>
/// <param name="RiskRate">Tasa de riesgo aplicada (0.56% / 2.50% / 5.39%)</param>
/// <param name="TotalEmployerCost">Costo total del empleador (CSS base + riesgo)</param>
public record CssFullCalculationResult(
    CssCalculationResult EmployeeCss,
    CssCalculationResult EmployerCss,
    decimal RiskContribution,
    decimal RiskRate,
    decimal TotalEmployerCost
);
