// ====================================================================
// Planilla - AcumuladoFiscalService
// Arma el acumulado fiscal del año de un empleado para el motor de ISR.
//
// Decisión de diseño: lo acumulado se DERIVA de las planillas guardadas en
// lugar de llevarse en un contador que se va sumando corrida a corrida. Un
// contador mutable obliga a revertir con exactitud cada recálculo y cada
// anulación, y basta un camino que no revierta para que el empleado quede con
// el ISR del año descuadrado. Derivándolo, el acumulado siempre refleja lo que
// de verdad hay guardado.
//
// Los saldos iniciales de migración sí se guardan: no hay planillas de las que
// derivarlos porque se generaron en el sistema anterior.
// ====================================================================

using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Application.Services;
using Vorluno.Planilla.Domain.Enums;
using Vorluno.Planilla.Infrastructure.Data;

namespace Vorluno.Planilla.Infrastructure.Services;

/// <inheritdoc cref="IAcumuladoFiscalService"/>
public class AcumuladoFiscalService : IAcumuladoFiscalService
{
    private readonly ApplicationDbContext _context;

    public AcumuladoFiscalService(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AcumuladoIsr> ObtenerAcumuladoAsync(
        int empleadoId,
        int anio,
        int? excluirPayrollHeaderId = null,
        int? excluirPlanillaDecimoId = null,
        CancellationToken cancellationToken = default)
    {
        // Saldos que trae de otro sistema. Puede no existir el registro: un
        // empleado que nació en esta plataforma arranca todo en cero.
        var saldos = await _context.AcumuladosFiscalesEmpleados
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Anio == anio, cancellationToken);

        // Planillas regulares del año. La base gravable es el bruto menos el
        // Seguro Social: es la única deducción que resta, criterio confirmado
        // por el contador de la empresa.
        var regulares = await ConsultaRegulares(empleadoId, anio, excluirPayrollHeaderId)
            .Select(d => new { d.GrossPay, d.CssEmployee, d.IncomeTax })
            .ToListAsync(cancellationToken);

        var ingresoGravable = regulares.Sum(d => d.GrossPay - d.CssEmployee);
        var isrRegular = regulares.Sum(d => d.IncomeTax);

        // Partidas de décimo del año. Van aparte porque el motor las trata como
        // un ingreso propio dentro del reparto de períodos equivalentes.
        var decimos = await ConsultaDecimos(empleadoId, anio, excluirPlanillaDecimoId)
            .Select(d => new { d.MontoDecimo, d.CssEmpleado, d.ISR })
            .ToListAsync(cancellationToken);

        var decimoGravable = decimos.Sum(d => d.MontoDecimo - d.CssEmpleado);
        var isrDecimo = decimos.Sum(d => d.ISR);

        return new AcumuladoIsr
        {
            IngresoGravableInicial = saldos?.IngresoGravableInicial ?? 0m,
            DecimoInicial = saldos?.DecimoInicial ?? 0m,
            IsrRetenidoInicial = saldos?.IsrRetenidoInicial ?? 0m,
            IngresoGravableProcesado = ingresoGravable,
            DecimoProcesado = decimoGravable,
            IsrRegularProcesado = isrRegular,
            IsrDecimoProcesado = isrDecimo
        };
    }

    public async Task<int> ObtenerNumeroPeriodoAsync(
        int empleadoId,
        int anio,
        int? excluirPayrollHeaderId = null,
        CancellationToken cancellationToken = default)
    {
        var corridasPrevias = await ConsultaRegulares(empleadoId, anio, excluirPayrollHeaderId)
            .CountAsync(cancellationToken);

        // La corrida que se está calculando todavía no está guardada, por eso el +1.
        return corridasPrevias + 1;
    }

    /// <summary>
    /// Detalles de planilla regular del empleado en el año, sin las anuladas y
    /// sin la que se esté recalculando.
    /// </summary>
    private IQueryable<Domain.Entities.PayrollDetail> ConsultaRegulares(
        int empleadoId, int anio, int? excluirPayrollHeaderId)
    {
        var query = _context.PayrollDetails
            .AsNoTracking()
            .Where(d => d.EmpleadoId == empleadoId
                     && d.PayrollHeader!.PayDate.Year == anio
                     && d.PayrollHeader.Status != PayrollStatus.Cancelled);

        if (excluirPayrollHeaderId.HasValue)
            query = query.Where(d => d.PayrollHeaderId != excluirPayrollHeaderId.Value);

        return query;
    }

    /// <summary>Partidas de décimo del empleado en el año, sin la que se esté recalculando.</summary>
    private IQueryable<Domain.Entities.DetalleDecimo> ConsultaDecimos(
        int empleadoId, int anio, int? excluirPlanillaDecimoId)
    {
        var query = _context.DetallesDecimo
            .AsNoTracking()
            .Where(d => d.EmpleadoId == empleadoId
                     && d.PlanillaDecimo!.FechaPago.Year == anio);

        if (excluirPlanillaDecimoId.HasValue)
            query = query.Where(d => d.PlanillaDecimoId != excluirPlanillaDecimoId.Value);

        return query;
    }
}
