// ====================================================================
// Tests de AcumuladoFiscalService
//
// Lo que importa acá no es la aritmética (esa la cubre MotorIsrPanamaTests)
// sino QUÉ entra al acumulado: el servicio lo deriva de las planillas
// guardadas, así que un recálculo, una anulación o una planilla de otro año
// no deben ensuciar el año fiscal del empleado.
// ====================================================================

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Domain.Entities;
using Vorluno.Planilla.Domain.Enums;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Infrastructure.Data;
using Vorluno.Planilla.Infrastructure.Services;
using Xunit;

namespace Vorluno.Planilla.Web.IntegrationTests.Services;

public class AcumuladoFiscalServiceTests
{
    private const int TenantId = 1;
    private const int EmpleadoId = 10;
    private const int Anio = 2026;

    private static ApplicationDbContext NuevoContexto() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"acumulado-{Guid.NewGuid()}")
                .Options,
            currentUserService: null,
            tenantContext: new BypassTenantContext());

    /// <summary>Crea una planilla con un detalle para el empleado indicado.</summary>
    private static void SembrarPlanilla(
        ApplicationDbContext db,
        int headerId,
        DateTime fechaPago,
        decimal bruto,
        decimal css,
        decimal isr,
        PayrollStatus estado = PayrollStatus.Approved,
        int empleadoId = EmpleadoId)
    {
        db.PayrollHeaders.Add(new PayrollHeader
        {
            Id = headerId,
            TenantId = TenantId,
            PayrollNumber = $"P-{headerId}",
            PeriodStartDate = fechaPago.AddDays(-15),
            PeriodEndDate = fechaPago,
            PayDate = fechaPago,
            PayPeriodType = PayPeriodType.Quincenal,
            Status = estado
        });

        db.PayrollDetails.Add(new PayrollDetail
        {
            Id = headerId * 100,
            TenantId = TenantId,
            PayrollHeaderId = headerId,
            EmpleadoId = empleadoId,
            GrossPay = bruto,
            CssEmployee = css,
            IncomeTax = isr
        });
    }

    [Fact]
    public async Task EmpleadoNuevo__AcumuladoEnCeroYPrimerPeriodo()
    {
        using var db = NuevoContexto();
        var servicio = new AcumuladoFiscalService(db);

        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);
        var periodo = await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio);

        acumulado.IngresoGravableTotal.Should().Be(0m);
        acumulado.IsrRetenidoTotal.Should().Be(0m);
        periodo.Should().Be(1, "la corrida que se está calculando es la primera del año");
    }

    [Fact]
    public async Task ConPlanillasDelAnio__SumaBrutoMenosSeguroSocialYElIsrRetenido()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio, 1, 15), bruto: 1000m, css: 97.50m, isr: 20m);
        SembrarPlanilla(db, 2, new DateTime(Anio, 1, 31), bruto: 1000m, css: 97.50m, isr: 20m);
        SembrarPlanilla(db, 3, new DateTime(Anio, 2, 15), bruto: 1200m, css: 117m, isr: 25m);
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);

        // La base gravable resta únicamente el Seguro Social.
        acumulado.IngresoGravableTotal.Should().Be(3200m - 312m);
        acumulado.IsrRetenidoTotal.Should().Be(65m);
        (await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio)).Should().Be(4);
    }

    [Fact]
    public async Task AlRecalcularUnaPlanilla__NoSeCuentaASiMisma()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio, 1, 15), bruto: 1000m, css: 97.50m, isr: 20m);
        SembrarPlanilla(db, 2, new DateTime(Anio, 1, 31), bruto: 1000m, css: 97.50m, isr: 20m);
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio, excluirPayrollHeaderId: 2);
        var periodo = await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio, excluirPayrollHeaderId: 2);

        acumulado.IngresoGravableTotal.Should().Be(1000m - 97.50m);
        acumulado.IsrRetenidoTotal.Should().Be(20m);
        periodo.Should().Be(2, "recalcular la segunda planilla la deja siendo la segunda, no la tercera");
    }

    [Fact]
    public async Task PlanillaAnulada__QuedaFueraDelAcumulado()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio, 1, 15), bruto: 1000m, css: 97.50m, isr: 20m);
        SembrarPlanilla(db, 2, new DateTime(Anio, 1, 31), bruto: 1000m, css: 97.50m, isr: 20m,
            estado: PayrollStatus.Cancelled);
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);

        acumulado.IsrRetenidoTotal.Should().Be(20m);
        (await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio)).Should().Be(2);
    }

    [Fact]
    public async Task PlanillasDeOtroAnio__NoEntranAlAnioFiscal()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio - 1, 12, 31), bruto: 5000m, css: 487.50m, isr: 300m);
        SembrarPlanilla(db, 2, new DateTime(Anio, 1, 15), bruto: 1000m, css: 97.50m, isr: 20m);
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);

        acumulado.IsrRetenidoTotal.Should().Be(20m);
        (await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio)).Should().Be(2);
    }

    [Fact]
    public async Task PlanillasDeOtroEmpleado__NoAfectanElContadorDelEmpleado()
    {
        using var db = NuevoContexto();
        // La empresa lleva tres corridas del año, pero este empleado entró ahora.
        SembrarPlanilla(db, 1, new DateTime(Anio, 1, 15), 1000m, 97.50m, 20m, empleadoId: 99);
        SembrarPlanilla(db, 2, new DateTime(Anio, 1, 31), 1000m, 97.50m, 20m, empleadoId: 99);
        SembrarPlanilla(db, 3, new DateTime(Anio, 2, 15), 1000m, 97.50m, 20m, empleadoId: 99);
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);

        (await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio))
            .Should().Be(1, "el contador cuenta las corridas del empleado, no las de la empresa");
        (await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio)).IngresoGravableTotal.Should().Be(0m);
    }

    [Fact]
    public async Task SaldosDeMigracion__SeSumanALoProcesadoPorElSistema()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio, 7, 15), bruto: 1000m, css: 97.50m, isr: 20m);
        db.AcumuladosFiscalesEmpleados.Add(new AcumuladoFiscalEmpleado
        {
            TenantId = TenantId,
            EmpleadoId = EmpleadoId,
            Anio = Anio,
            IngresoGravableInicial = 9_000m,
            DecimoInicial = 500m,
            IsrRetenidoInicial = 150m
        });
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);

        acumulado.IngresoGravableTotal.Should().Be(9_000m + 902.50m);
        acumulado.DecimoTotal.Should().Be(500m);
        acumulado.IsrRetenidoTotal.Should().Be(170m, "lo retenido antes de migrar no se le vuelve a cobrar");
    }

    [Fact]
    public async Task PartidasDeDecimo__SeAcumulanApartemDelSalario()
    {
        using var db = NuevoContexto();
        SembrarPlanilla(db, 1, new DateTime(Anio, 4, 15), bruto: 1000m, css: 97.50m, isr: 20m);

        db.PlanillasDecimo.Add(new PlanillaDecimo
        {
            Id = 1,
            TenantId = TenantId,
            Numero = "D-1",
            PeriodoDesde = new DateTime(Anio, 1, 1),
            PeriodoHasta = new DateTime(Anio, 4, 15),
            FechaPago = new DateTime(Anio, 4, 15),
            Estado = EstadoDecimo.Pagada
        });
        db.DetallesDecimo.Add(new DetalleDecimo
        {
            Id = 1,
            TenantId = TenantId,
            PlanillaDecimoId = 1,
            EmpleadoId = EmpleadoId,
            MontoDecimo = 333.33m,
            CssEmpleado = 24.17m,
            ISR = 7.69m
        });
        await db.SaveChangesAsync();

        var servicio = new AcumuladoFiscalService(db);
        var acumulado = await servicio.ObtenerAcumuladoAsync(EmpleadoId, Anio);

        acumulado.IngresoGravableTotal.Should().Be(902.50m, "el décimo no se mezcla con el salario");
        acumulado.DecimoTotal.Should().Be(333.33m - 24.17m);
        acumulado.IsrRetenidoTotal.Should().Be(27.69m, "el ISR del décimo también cuenta como retenido");
        (await servicio.ObtenerNumeroPeriodoAsync(EmpleadoId, Anio))
            .Should().Be(2, "el décimo no suma una corrida de salario al contador");
    }

    private class BypassTenantContext : ITenantContext
    {
        public int TenantId => 0;
        public TenantRole TenantRole => TenantRole.User;
        public string? UserId => null;
        public bool IsSystemAdmin => false;
        public bool HasTenant => false;
        public Task SetTenantAsync(int tenantId) => Task.CompletedTask;
        public Task<Tenant?> GetCurrentTenantAsync() => Task.FromResult<Tenant?>(null);
        public bool HasRole(TenantRole role) => false;
        public bool IsAdminOrOwner() => false;
        public void Clear() { }
    }
}
