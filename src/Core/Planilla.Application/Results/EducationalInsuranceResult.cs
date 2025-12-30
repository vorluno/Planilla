// ====================================================================
// Planilla - EducationalInsuranceResult
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Resultado de cálculo de Seguro Educativo (SE)
// IMPORTANTE: Seguro Educativo NO tiene tope máximo
// ====================================================================

namespace Vorluno.Planilla.Application.Results;

/// <summary>
/// Resultado del cálculo de Seguro Educativo.
/// El Seguro Educativo se calcula sobre el salario completo (SIN tope).
/// </summary>
/// <param name="EmployeeRate">Tasa del empleado (1.25%)</param>
/// <param name="EmployerRate">Tasa del empleador (1.50%)</param>
/// <param name="EmployeeDeduction">Monto deducido al empleado</param>
/// <param name="EmployerContribution">Monto aportado por el empleador</param>
/// <param name="Total">Total de Seguro Educativo (empleado + empleador)</param>
public record EducationalInsuranceResult(
    decimal EmployeeRate,
    decimal EmployerRate,
    decimal EmployeeDeduction,
    decimal EmployerContribution,
    decimal Total
);
