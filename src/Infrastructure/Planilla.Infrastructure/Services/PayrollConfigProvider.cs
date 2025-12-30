// ====================================================================
// Planilla - PayrollConfigProvider
// Source: Core360 Stage 2
// Portado: 2025-12-26
// Descripción: Proveedor de configuración de planilla desde base de datos
// ====================================================================

using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Application.DTOs;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Infrastructure.Data;

namespace Vorluno.Planilla.Infrastructure.Services;

/// <summary>
/// Implementación de IPayrollConfigProvider que obtiene configuración desde ApplicationDbContext.
/// Utiliza AsNoTracking() para queries de solo lectura (mejor performance).
/// </summary>
public class PayrollConfigProvider : IPayrollConfigProvider
{
    private readonly ApplicationDbContext _context;

    public PayrollConfigProvider(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene la configuración de tasas vigente para una fecha específica.
    /// Busca config donde effectiveDate esté entre EffectiveStartDate y EffectiveEndDate (o End sea null).
    /// </summary>
    /// <param name="companyId">ID de la compañía</param>
    /// <param name="effectiveDate">Fecha para determinar configuración vigente</param>
    /// <returns>Configuración vigente o null si no existe</returns>
    public async Task<PayrollTaxConfigDto?> GetTaxConfigAsync(int companyId, DateTime effectiveDate)
    {
        var config = await _context.PayrollTaxConfigurations
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId
                     && c.IsActive
                     && c.EffectiveStartDate <= effectiveDate
                     && (c.EffectiveEndDate == null || c.EffectiveEndDate >= effectiveDate))
            .Select(c => new PayrollTaxConfigDto(
                c.Id,
                c.CompanyId,
                c.EffectiveStartDate,
                c.EffectiveEndDate,
                c.CssEmployeeRate,
                c.CssEmployerBaseRate,
                c.CssRiskRateLow,
                c.CssRiskRateMedium,
                c.CssRiskRateHigh,
                c.CssMaxContributionBaseStandard,
                c.CssMaxContributionBaseIntermediate,
                c.CssMaxContributionBaseHigh,
                c.CssIntermediateMinYears,
                c.CssIntermediateMinAvgSalary,
                c.CssHighMinYears,
                c.CssHighMinAvgSalary,
                c.EducationalInsuranceEmployeeRate,
                c.EducationalInsuranceEmployerRate,
                c.DependentDeductionAmount,
                c.MaxDependents
            ))
            .FirstOrDefaultAsync();

        return config;
    }

    /// <summary>
    /// Obtiene los brackets de ISR para un año fiscal específico.
    /// Retorna brackets ordenados por Order ASC para cálculo secuencial.
    /// </summary>
    /// <param name="companyId">ID de la compañía</param>
    /// <param name="year">Año fiscal (ej: 2025)</param>
    /// <returns>Lista de brackets ordenados. Lista vacía si no existen brackets.</returns>
    public async Task<List<TaxBracketDto>> GetTaxBracketsAsync(int companyId, int year)
    {
        var brackets = await _context.TaxBrackets
            .AsNoTracking()
            .Where(b => b.CompanyId == companyId
                     && b.Year == year
                     && b.IsActive)
            .OrderBy(b => b.Order)
            .Select(b => new TaxBracketDto(
                b.Id,
                b.CompanyId,
                b.Year,
                b.Order,
                b.Description,
                b.MinIncome,
                b.MaxIncome,
                b.Rate,
                b.FixedAmount
            ))
            .ToListAsync();

        return brackets;
    }
}
