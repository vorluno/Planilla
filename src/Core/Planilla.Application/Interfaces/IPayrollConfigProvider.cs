// ====================================================================
// Planilla - IPayrollConfigProvider Interface
// Source: Core360 Stage 2
// Portado: 2025-12-26
// Descripción: Proveedor de configuración de planilla para servicios de cálculo
// ====================================================================

using Vorluno.Planilla.Application.DTOs;

namespace Vorluno.Planilla.Application.Interfaces;

/// <summary>
/// Interface para proveer configuración de tasas e impuestos de planilla.
/// Abstrae el acceso a PayrollTaxConfiguration y TaxBracket desde servicios de cálculo.
/// </summary>
public interface IPayrollConfigProvider
{
    /// <summary>
    /// Obtiene la configuración de tasas vigente para una fecha específica.
    /// </summary>
    /// <param name="companyId">ID de la compañía</param>
    /// <param name="effectiveDate">Fecha para determinar configuración vigente</param>
    /// <returns>
    /// Configuración vigente o null si no existe.
    /// Busca config donde effectiveDate >= EffectiveStartDate AND effectiveDate &lt;= EffectiveEndDate
    /// </returns>
    Task<PayrollTaxConfigDto?> GetTaxConfigAsync(int companyId, DateTime effectiveDate);

    /// <summary>
    /// Obtiene los brackets de ISR para un año fiscal específico.
    /// </summary>
    /// <param name="companyId">ID de la compañía</param>
    /// <param name="year">Año fiscal (ej: 2025)</param>
    /// <returns>
    /// Lista de brackets ordenados por Order ASC.
    /// Lista vacía si no existen brackets para ese año.
    /// </returns>
    Task<List<TaxBracketDto>> GetTaxBracketsAsync(int companyId, int year);
}
