// ====================================================================
// Planilla - PayrollCalculationResult
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Resultado completo de cálculo de planilla para un empleado
// ====================================================================

namespace Vorluno.Planilla.Application.Results;

/// <summary>
/// Resultado completo del cálculo de planilla para un empleado.
/// Incluye todas las deducciones del empleado y costos del empleador.
/// </summary>
/// <param name="GrossPay">Salario bruto del empleado</param>
/// <param name="CssEmployee">Aporte CSS del empleado (deducción)</param>
/// <param name="EducationalInsuranceEmployee">Seguro Educativo del empleado (deducción)</param>
/// <param name="IncomeTax">Impuesto sobre la renta del empleado (deducción)</param>
/// <param name="TotalDeductions">Total de deducciones al empleado</param>
/// <param name="NetPay">Salario neto a pagar al empleado (GrossPay - TotalDeductions)</param>
/// <param name="CssEmployer">Aporte CSS del empleador (costo patronal)</param>
/// <param name="EducationalInsuranceEmployer">Seguro Educativo del empleador (costo patronal)</param>
/// <param name="RiskContribution">Riesgo profesional (costo patronal)</param>
/// <param name="TotalEmployerCost">Costo total del empleador (CSS + SE + Riesgo)</param>
public record PayrollCalculationResult(
    decimal GrossPay,
    decimal CssEmployee,
    decimal EducationalInsuranceEmployee,
    decimal IncomeTax,
    decimal TotalDeductions,
    decimal NetPay,
    decimal CssEmployer,
    decimal EducationalInsuranceEmployer,
    decimal RiskContribution,
    decimal TotalEmployerCost
);
