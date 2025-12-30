// ====================================================================
// Planilla - EducationalInsuranceServicePortable
// Source: Core360 Stage 4, Sección 4
// Portado: 2025-12-26
// Descripción: Servicio de cálculo de Seguro Educativo (SE) de Panamá
// IMPORTANTE: Seguro Educativo NO tiene tope máximo, se aplica sobre salario total
// Cambios vs Core360:
//   - Eliminados hardcodes de tasas (1.25%, 1.50%)
//   - Agregado IPayrollConfigProvider
//   - Agregado RoundingPolicy
//   - Lanza InvalidOperationException si falta configuración
// ====================================================================

using Vorluno.Planilla.Application.DTOs;
using Vorluno.Planilla.Application.Helpers;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Application.Results;

namespace Vorluno.Planilla.Application.Services;

/// <summary>
/// Servicio de cálculo de Seguro Educativo.
/// NOTA CRÍTICA: El Seguro Educativo NO tiene tope máximo, se aplica sobre el salario total.
/// </summary>
public class EducationalInsuranceServicePortable
{
    private readonly IPayrollConfigProvider _configProvider;

    public EducationalInsuranceServicePortable(IPayrollConfigProvider configProvider)
    {
        _configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
    }

    /// <summary>
    /// Calcula el Seguro Educativo del empleado (deducción de nómina).
    /// </summary>
    /// <param name="companyId">ID de compañía</param>
    /// <param name="grossPay">Salario bruto (sin tope - SE se aplica sobre salario completo)</param>
    /// <param name="isSubjectToInsurance">Indica si el empleado está sujeto a Seguro Educativo</param>
    /// <param name="calculationDate">Fecha de cálculo (para determinar configuración vigente)</param>
    /// <returns>Monto de Seguro Educativo del empleado</returns>
    public async Task<decimal> CalculateEmployeeInsuranceAsync(
        int companyId,
        decimal grossPay,
        bool isSubjectToInsurance,
        DateTime calculationDate)
    {
        // Si no está sujeto a Seguro Educativo, retorna cero
        if (!isSubjectToInsurance)
        {
            return 0;
        }

        // Obtener configuración de tasas
        var config = await _configProvider.GetTaxConfigAsync(companyId, calculationDate);
        if (config == null)
        {
            throw new InvalidOperationException(
                $"No se encontró configuración de Seguro Educativo para companyId={companyId} en fecha {calculationDate:yyyy-MM-dd}. " +
                "Verifique que exista una configuración vigente en PayrollTaxConfigurations.");
        }

        // Seguro Educativo NO tiene tope máximo, se aplica sobre el salario total
        var rate = config.EducationalInsuranceEmployeeRate; // Ej: 1.25%
        var amount = RoundingPolicy.CalculatePercentage(grossPay, rate);

        return amount;
    }

    /// <summary>
    /// Calcula el Seguro Educativo del empleador (costo patronal).
    /// </summary>
    /// <param name="companyId">ID de compañía</param>
    /// <param name="grossPay">Salario bruto (sin tope - SE se aplica sobre salario completo)</param>
    /// <param name="isSubjectToInsurance">Indica si el empleado está sujeto a Seguro Educativo</param>
    /// <param name="calculationDate">Fecha de cálculo (para determinar configuración vigente)</param>
    /// <returns>Monto de Seguro Educativo del empleador</returns>
    public async Task<decimal> CalculateEmployerInsuranceAsync(
        int companyId,
        decimal grossPay,
        bool isSubjectToInsurance,
        DateTime calculationDate)
    {
        // Si no está sujeto a Seguro Educativo, retorna cero
        if (!isSubjectToInsurance)
        {
            return 0;
        }

        // Obtener configuración de tasas
        var config = await _configProvider.GetTaxConfigAsync(companyId, calculationDate);
        if (config == null)
        {
            throw new InvalidOperationException(
                $"No se encontró configuración de Seguro Educativo para companyId={companyId} en fecha {calculationDate:yyyy-MM-dd}. " +
                "Verifique que exista una configuración vigente en PayrollTaxConfigurations.");
        }

        // Seguro Educativo NO tiene tope máximo, se aplica sobre el salario total
        var rate = config.EducationalInsuranceEmployerRate; // Ej: 1.50%
        var amount = RoundingPolicy.CalculatePercentage(grossPay, rate);

        return amount;
    }

    /// <summary>
    /// Calcula el Seguro Educativo completo (empleado + empleador).
    /// Método orquestador que llama a los cálculos individuales.
    /// </summary>
    /// <param name="companyId">ID de compañía</param>
    /// <param name="grossPay">Salario bruto (sin tope - SE se aplica sobre salario completo)</param>
    /// <param name="isSubjectToInsurance">Indica si el empleado está sujeto a Seguro Educativo</param>
    /// <param name="calculationDate">Fecha de cálculo (para determinar configuración vigente)</param>
    /// <returns>Resultado completo de Seguro Educativo</returns>
    public async Task<EducationalInsuranceResult> CalculateFullInsuranceAsync(
        int companyId,
        decimal grossPay,
        bool isSubjectToInsurance,
        DateTime calculationDate)
    {
        // Obtener configuración de tasas (para incluir en el resultado)
        var config = await _configProvider.GetTaxConfigAsync(companyId, calculationDate);
        if (config == null)
        {
            throw new InvalidOperationException(
                $"No se encontró configuración de Seguro Educativo para companyId={companyId} en fecha {calculationDate:yyyy-MM-dd}");
        }

        // Calcular Seguro Educativo empleado
        var employeeDeduction = await CalculateEmployeeInsuranceAsync(
            companyId, grossPay, isSubjectToInsurance, calculationDate);

        // Calcular Seguro Educativo empleador
        var employerContribution = await CalculateEmployerInsuranceAsync(
            companyId, grossPay, isSubjectToInsurance, calculationDate);

        return new EducationalInsuranceResult(
            EmployeeRate: config.EducationalInsuranceEmployeeRate,
            EmployerRate: config.EducationalInsuranceEmployerRate,
            EmployeeDeduction: employeeDeduction,
            EmployerContribution: employerContribution,
            Total: employeeDeduction + employerContribution
        );
    }
}
