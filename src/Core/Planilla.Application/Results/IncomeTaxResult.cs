// ====================================================================
// Planilla - IncomeTaxResult
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Resultado de cálculo de Impuesto Sobre la Renta (ISR)
// ====================================================================

namespace Vorluno.Planilla.Application.Results;

/// <summary>
/// Resultado del cálculo de Impuesto Sobre la Renta (ISR).
/// El ISR se calcula sobre proyección anual con brackets progresivos.
/// </summary>
/// <param name="TaxableIncome">Ingreso anual proyectado</param>
/// <param name="DependentDeduction">Deducción por dependientes (B/. 800 × cantidad, máx 3)</param>
/// <param name="NetTaxableIncome">Ingreso neto gravable (TaxableIncome - DependentDeduction)</param>
/// <param name="TaxAmount">Impuesto anual calculado según brackets</param>
/// <param name="EffectiveTaxRate">Tasa efectiva de impuesto (TaxAmount / TaxableIncome × 100)</param>
public record IncomeTaxResult(
    decimal TaxableIncome,
    decimal DependentDeduction,
    decimal NetTaxableIncome,
    decimal TaxAmount,
    decimal EffectiveTaxRate
);
