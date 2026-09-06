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
using Vorluno.Planilla.Application.DTOs;
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


    public async Task<FichaIsrAnualDto?> ObtenerFichaAnualAsync(
        int empleadoId,
        int anio,
        CancellationToken cancellationToken = default)
    {
        var empleado = await _context.Empleados
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == empleadoId, cancellationToken);

        if (empleado is null) return null;

        var saldos = await _context.AcumuladosFiscalesEmpleados
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.EmpleadoId == empleadoId && a.Anio == anio, cancellationToken);

        var frecuencia = empleado.PayPeriodType;

        var ficha = new FichaIsrAnualDto
        {
            EmpleadoId = empleadoId,
            NombreEmpleado = $"{empleado.Nombre} {empleado.Apellido}".Trim(),
            Cedula = empleado.NumeroIdentificacion,
            Anio = anio,
            Frecuencia = frecuencia.ToString(),
            PeriodosEquivalentes = MotorIsrPanama.ObtenerPeriodosEquivalentesAnuales(frecuencia),
            IngresoGravableInicial = saldos?.IngresoGravableInicial ?? 0m,
            DecimoInicial = saldos?.DecimoInicial ?? 0m,
            IsrRetenidoInicial = saldos?.IsrRetenidoInicial ?? 0m
        };

        // Las dos fuentes de corridas del año, mezcladas en orden de pago: así la
        // ficha muestra el décimo entre las quincenas, que es donde el contador
        // ve saltar el contador de períodos.
        var regulares = await ConsultaRegulares(empleadoId, anio, null)
            .Select(d => new FilaCorrida(
                d.PayrollHeader!.PayDate,
                d.PayrollHeader.PayrollNumber,
                false,
                d.GrossPay,
                d.CssEmployee,
                d.IncomeTax))
            .ToListAsync(cancellationToken);

        var decimos = await ConsultaDecimos(empleadoId, anio, null)
            .Select(d => new FilaCorrida(
                d.PlanillaDecimo!.FechaPago,
                d.PlanillaDecimo.Numero,
                true,
                d.MontoDecimo,
                d.CssEmpleado,
                d.ISR))
            .ToListAsync(cancellationToken);

        var corridas = regulares.Concat(decimos)
            .OrderBy(c => c.FechaPago)
            .ThenBy(c => c.EsDecimo)
            .ToList();

        // Se va reconstruyendo el acumulado corrida por corrida, igual que lo
        // haría el motor si se recalculara el año entero de una sentada.
        var acumulado = new AcumuladoIsr
        {
            IngresoGravableInicial = ficha.IngresoGravableInicial,
            DecimoInicial = ficha.DecimoInicial,
            IsrRetenidoInicial = ficha.IsrRetenidoInicial
        };

        var gravableProcesado = 0m;
        var decimoProcesado = 0m;
        var isrRegularProcesado = 0m;
        var isrDecimoProcesado = 0m;
        var numeroPeriodo = 0;

        foreach (var corrida in corridas)
        {
            var gravable = corrida.Bruto - corrida.Css;

            // El décimo no suma una corrida de salario: entra al reparto por su
            // propio peso, no como un período más del calendario.
            if (!corrida.EsDecimo) numeroPeriodo++;

            var anterior = new AcumuladoIsr
            {
                IngresoGravableInicial = ficha.IngresoGravableInicial,
                DecimoInicial = ficha.DecimoInicial,
                IsrRetenidoInicial = ficha.IsrRetenidoInicial,
                IngresoGravableProcesado = gravableProcesado,
                DecimoProcesado = decimoProcesado,
                IsrRegularProcesado = isrRegularProcesado,
                IsrDecimoProcesado = isrDecimoProcesado
            };

            var resultado = MotorIsrPanama.Calcular(new CorridaIsr
            {
                Frecuencia = frecuencia,
                NumeroPeriodoEmpleado = Math.Max(1, numeroPeriodo),
                AcumuladoAnterior = anterior,
                Movimientos = new[]
                {
                    new MovimientoIsr(
                        corrida.EsDecimo ? TratamientoIsr.DecimoTercerMes : TratamientoIsr.GravableAcumulable,
                        gravable)
                }
            });

            if (corrida.EsDecimo)
            {
                decimoProcesado += gravable;
                isrDecimoProcesado += corrida.Isr;
            }
            else
            {
                gravableProcesado += gravable;
                isrRegularProcesado += corrida.Isr;
            }

            ficha.Filas.Add(new FilaFichaIsrDto
            {
                Periodo = Math.Max(1, numeroPeriodo),
                FechaPago = corrida.FechaPago,
                Concepto = corrida.EsDecimo ? $"Décimo {corrida.Numero}" : corrida.Numero,
                EsDecimo = corrida.EsDecimo,
                Bruto = corrida.Bruto,
                SeguroSocial = corrida.Css,
                Gravable = gravable,
                GravableAcumulado = resultado.IngresoGravableAcumulado,
                DecimoAcumulado = resultado.DecimoAcumulado,
                PeriodoEquivalente = CalcularPeriodoEquivalente(resultado, Math.Max(1, numeroPeriodo)),
                IngresoAnualProyectado = resultado.IngresoAnualProyectado,
                IsrAnualProyectado = resultado.IsrAnualProyectado,
                IsrDebidoAcumulado = resultado.IsrDebidoAcumulado,
                IsrCalculado = resultado.IsrDescontarPeriodo,
                IsrRetenido = corrida.Isr,
                IsrRetenidoAcumulado = ficha.IsrRetenidoInicial + isrRegularProcesado + isrDecimoProcesado
            });
        }

        ficha.TotalGravable = ficha.IngresoGravableInicial + gravableProcesado;
        ficha.TotalDecimo = ficha.DecimoInicial + decimoProcesado;
        ficha.TotalIsrRetenido = ficha.IsrRetenidoInicial + isrRegularProcesado + isrDecimoProcesado;

        ficha.IsrDelAnioSegunIngresoReal =
            MotorIsrPanama.CalcularIsrAnual(ficha.TotalGravable + ficha.TotalDecimo);
        ficha.DiferenciaRetenido = ficha.TotalIsrRetenido - ficha.IsrDelAnioSegunIngresoReal;

        return ficha;
    }

    /// <summary>
    /// Reconstruye el contador de períodos equivalentes que usó el motor, que es
    /// la columna PERIODOS del libro del contador (7.667, 16.333, 25.000, 26.000).
    /// </summary>
    private static decimal CalcularPeriodoEquivalente(ResultadoIsr resultado, int numeroPeriodo)
    {
        if (resultado.IngresoGravableAcumulado <= 0m) return numeroPeriodo;

        var promedio = resultado.IngresoGravableAcumulado / numeroPeriodo;
        return Math.Round(numeroPeriodo + resultado.DecimoAcumulado / promedio, 3,
            MidpointRounding.AwayFromZero);
    }

    /// <summary>Una corrida del año, venga de planilla regular o de décimo.</summary>
    private sealed record FilaCorrida(
        DateTime FechaPago, string Numero, bool EsDecimo, decimal Bruto, decimal Css, decimal Isr);

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
