using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <--- USANDO AÑADIDO
using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Domain.Entities;                         // <--- USANDO AÑADIDO

namespace Vorluno.Planilla.Infrastructure.Data;

// CAMBIO CLAVE: Heredamos de IdentityDbContext<AppUser> en lugar de solo DbContext
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<ReciboDeSueldo> RecibosDeSueldo { get; set; }

    // Phase A: Configuración de planilla (tasas CSS, SE, ISR)
    public DbSet<PayrollTaxConfiguration> PayrollTaxConfigurations { get; set; }
    public DbSet<TaxBracket> TaxBrackets { get; set; }

    // Phase D: Workflow de planilla
    public DbSet<PayrollHeader> PayrollHeaders { get; set; }
    public DbSet<PayrollDetail> PayrollDetails { get; set; }

    // Organización: Departamentos y Posiciones
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Posicion> Posiciones { get; set; }

    // Conceptos de Nómina: Préstamos, Deducciones y Anticipos
    public DbSet<Prestamo> Prestamos { get; set; }
    public DbSet<DeduccionFija> DeduccionesFijas { get; set; }
    public DbSet<Anticipo> Anticipos { get; set; }
    public DbSet<PagoPrestamo> PagosPrestamos { get; set; }

    // Asistencia: Horas Extra, Ausencias y Vacaciones
    public DbSet<HoraExtra> HorasExtra { get; set; }
    public DbSet<Ausencia> Ausencias { get; set; }
    public DbSet<SolicitudVacaciones> SolicitudesVacaciones { get; set; }
    public DbSet<SaldoVacaciones> SaldosVacaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Esta línea es crucial al heredar de IdentityDbContext
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Empleado>()
            .HasIndex(e => e.NumeroIdentificacion)
            .IsUnique();

        // Phase A: Configuración de PayrollTaxConfiguration
        modelBuilder.Entity<PayrollTaxConfiguration>(entity =>
        {
            // Índice compuesto para búsquedas por compañía y fecha efectiva
            entity.HasIndex(p => new { p.CompanyId, p.EffectiveStartDate })
                .HasDatabaseName("IX_PayrollTaxConfiguration_CompanyId_EffectiveStartDate");

            // Configuración de precisión para campos decimales (moneda)
            entity.Property(p => p.CssEmployeeRate).HasPrecision(5, 2);
            entity.Property(p => p.CssEmployerBaseRate).HasPrecision(5, 2);
            entity.Property(p => p.CssRiskRateLow).HasPrecision(5, 2);
            entity.Property(p => p.CssRiskRateMedium).HasPrecision(5, 2);
            entity.Property(p => p.CssRiskRateHigh).HasPrecision(5, 2);
            entity.Property(p => p.CssMaxContributionBaseStandard).HasPrecision(18, 2);
            entity.Property(p => p.CssMaxContributionBaseIntermediate).HasPrecision(18, 2);
            entity.Property(p => p.CssMaxContributionBaseHigh).HasPrecision(18, 2);
            entity.Property(p => p.CssIntermediateMinAvgSalary).HasPrecision(18, 2);
            entity.Property(p => p.CssHighMinAvgSalary).HasPrecision(18, 2);
            entity.Property(p => p.EducationalInsuranceEmployeeRate).HasPrecision(5, 2);
            entity.Property(p => p.EducationalInsuranceEmployerRate).HasPrecision(5, 2);
            entity.Property(p => p.DependentDeductionAmount).HasPrecision(18, 2);

            // Phase E: Global query filter para multi-tenancy
            entity.HasQueryFilter(p =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)p.CompanyId == _currentUserService.CompanyId);
        });

        // Phase A: Configuración de TaxBracket
        modelBuilder.Entity<TaxBracket>(entity =>
        {
            // Índice compuesto para búsquedas por compañía y año fiscal
            entity.HasIndex(t => new { t.CompanyId, t.Year })
                .HasDatabaseName("IX_TaxBracket_CompanyId_Year");

            // Configuración de precisión para campos decimales (moneda)
            entity.Property(t => t.MinIncome).HasPrecision(18, 2);
            entity.Property(t => t.MaxIncome).HasPrecision(18, 2);
            entity.Property(t => t.Rate).HasPrecision(5, 2);
            entity.Property(t => t.FixedAmount).HasPrecision(18, 2);

            // Phase E: Global query filter para multi-tenancy
            entity.HasQueryFilter(t =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)t.CompanyId == _currentUserService.CompanyId);
        });

        // Phase D: Configuración de PayrollHeader
        modelBuilder.Entity<PayrollHeader>(entity =>
        {
            // Índice único compuesto: PayrollNumber debe ser único por compañía
            entity.HasIndex(p => new { p.CompanyId, p.PayrollNumber })
                .IsUnique()
                .HasDatabaseName("IX_PayrollHeader_CompanyId_PayrollNumber");

            // Índice para búsquedas por estado
            entity.HasIndex(p => new { p.CompanyId, p.Status })
                .HasDatabaseName("IX_PayrollHeader_CompanyId_Status");

            // RowVersion como concurrency token
            entity.Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // Configuración de precisión para campos decimales (moneda)
            entity.Property(p => p.TotalGrossPay).HasPrecision(18, 2);
            entity.Property(p => p.TotalDeductions).HasPrecision(18, 2);
            entity.Property(p => p.TotalNetPay).HasPrecision(18, 2);
            entity.Property(p => p.TotalEmployerCost).HasPrecision(18, 2);

            // Relación 1:N con PayrollDetail
            entity.HasMany(p => p.Details)
                .WithOne(d => d.PayrollHeader)
                .HasForeignKey(d => d.PayrollHeaderId)
                .OnDelete(DeleteBehavior.Cascade); // Borrar detalles si se borra el header

            // Phase E: Global query filter para multi-tenancy
            entity.HasQueryFilter(p =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)p.CompanyId == _currentUserService.CompanyId);
        });

        // Phase D: Configuración de PayrollDetail
        modelBuilder.Entity<PayrollDetail>(entity =>
        {
            // Índice único compuesto: Un empleado solo puede aparecer una vez por planilla
            entity.HasIndex(d => new { d.PayrollHeaderId, d.EmpleadoId })
                .IsUnique()
                .HasDatabaseName("IX_PayrollDetail_PayrollHeaderId_EmpleadoId");

            // Configuración de precisión para campos decimales (moneda)
            entity.Property(d => d.GrossPay).HasPrecision(18, 2);
            entity.Property(d => d.BaseSalary).HasPrecision(18, 2);
            entity.Property(d => d.OvertimePay).HasPrecision(18, 2);
            entity.Property(d => d.Bonuses).HasPrecision(18, 2);
            entity.Property(d => d.Commissions).HasPrecision(18, 2);
            entity.Property(d => d.CssEmployee).HasPrecision(18, 2);
            entity.Property(d => d.CssEmployer).HasPrecision(18, 2);
            entity.Property(d => d.RiskContribution).HasPrecision(18, 2);
            entity.Property(d => d.EducationalInsuranceEmployee).HasPrecision(18, 2);
            entity.Property(d => d.EducationalInsuranceEmployer).HasPrecision(18, 2);
            entity.Property(d => d.IncomeTax).HasPrecision(18, 2);
            entity.Property(d => d.OtherDeductions).HasPrecision(18, 2);
            entity.Property(d => d.DeduccionesFijas).HasPrecision(18, 2);
            entity.Property(d => d.Prestamos).HasPrecision(18, 2);
            entity.Property(d => d.Anticipos).HasPrecision(18, 2);
            entity.Property(d => d.TotalDeductions).HasPrecision(18, 2);
            entity.Property(d => d.NetPay).HasPrecision(18, 2);
            entity.Property(d => d.EmployerCost).HasPrecision(18, 2);
            // Asistencia: Horas Extra
            entity.Property(d => d.HorasExtraDiurnas).HasPrecision(5, 2);
            entity.Property(d => d.HorasExtraNocturnas).HasPrecision(5, 2);
            entity.Property(d => d.HorasExtraDomingoFeriado).HasPrecision(5, 2);
            entity.Property(d => d.MontoHorasExtra).HasPrecision(18, 2);
            // Asistencia: Ausencias
            entity.Property(d => d.DiasAusenciaInjustificada).HasPrecision(5, 2);
            entity.Property(d => d.MontoDescuentoAusencias).HasPrecision(18, 2);
            // Asistencia: Vacaciones
            entity.Property(d => d.DiasVacaciones).HasPrecision(5, 2);
            entity.Property(d => d.MontoVacaciones).HasPrecision(18, 2);

            // Relación N:1 con Empleado
            entity.HasOne(d => d.Empleado)
                .WithMany()
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // NO borrar empleado si tiene detalles de planilla
        });

        // Organización: Configuración de Departamento
        modelBuilder.Entity<Departamento>(entity =>
        {
            // Índice único compuesto: Código debe ser único por compañía
            entity.HasIndex(d => new { d.CompanyId, d.Codigo })
                .IsUnique()
                .HasDatabaseName("IX_Departamento_CompanyId_Codigo");

            // Relación con Manager (jefe del departamento) - opcional
            entity.HasOne(d => d.Manager)
                .WithMany()
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra el manager, poner NULL

            // Relación 1:N con Empleados
            entity.HasMany(d => d.Empleados)
                .WithOne(e => e.Departamento)
                .HasForeignKey(e => e.DepartamentoId)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra departamento, poner NULL en empleados

            // Relación 1:N con Posiciones
            entity.HasMany(d => d.Posiciones)
                .WithOne(p => p.Departamento)
                .HasForeignKey(p => p.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict); // NO borrar departamento si tiene posiciones

            // Global query filter para multi-tenancy
            entity.HasQueryFilter(d =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)d.CompanyId == _currentUserService.CompanyId);
        });

        // Organización: Configuración de Posicion
        modelBuilder.Entity<Posicion>(entity =>
        {
            // Índice único compuesto: Código debe ser único por compañía
            entity.HasIndex(p => new { p.CompanyId, p.Codigo })
                .IsUnique()
                .HasDatabaseName("IX_Posicion_CompanyId_Codigo");

            // Configuración de precisión para campos decimales (salarios)
            entity.Property(p => p.SalarioMinimo).HasPrecision(18, 2);
            entity.Property(p => p.SalarioMaximo).HasPrecision(18, 2);

            // Relación 1:N con Empleados
            entity.HasMany(p => p.Empleados)
                .WithOne(e => e.Posicion)
                .HasForeignKey(e => e.PosicionId)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra posición, poner NULL en empleados

            // Global query filter para multi-tenancy
            entity.HasQueryFilter(p =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)p.CompanyId == _currentUserService.CompanyId);
        });

        // Conceptos de Nómina: Configuración de Prestamo
        modelBuilder.Entity<Prestamo>(entity =>
        {
            // Índice compuesto para búsquedas por empleado y estado
            entity.HasIndex(p => new { p.EmpleadoId, p.Estado })
                .HasDatabaseName("IX_Prestamo_EmpleadoId_Estado");

            // Configuración de precisión para campos decimales (moneda)
            entity.Property(p => p.MontoOriginal).HasPrecision(18, 2);
            entity.Property(p => p.MontoPendiente).HasPrecision(18, 2);
            entity.Property(p => p.CuotaMensual).HasPrecision(18, 2);
            entity.Property(p => p.TasaInteres).HasPrecision(5, 2);

            // Relación N:1 con Empleado
            entity.HasOne(p => p.Empleado)
                .WithMany()
                .HasForeignKey(p => p.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // NO borrar empleado si tiene préstamos

            // Relación 1:N con PagosPrestamo
            entity.HasMany(p => p.PagosPrestamo)
                .WithOne(pp => pp.Prestamo)
                .HasForeignKey(pp => pp.PrestamoId)
                .OnDelete(DeleteBehavior.Cascade); // Borrar pagos si se borra el préstamo

            // Global query filter para multi-tenancy
            entity.HasQueryFilter(p =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)p.CompanyId == _currentUserService.CompanyId);
        });

        // Conceptos de Nómina: Configuración de DeduccionFija
        modelBuilder.Entity<DeduccionFija>(entity =>
        {
            // Índice compuesto para búsquedas por empleado y estado
            entity.HasIndex(d => new { d.EmpleadoId, d.EstaActivo })
                .HasDatabaseName("IX_DeduccionFija_EmpleadoId_EstaActivo");

            // Índice para búsquedas por tipo
            entity.HasIndex(d => d.TipoDeduccion)
                .HasDatabaseName("IX_DeduccionFija_TipoDeduccion");

            // Configuración de precisión para campos decimales
            entity.Property(d => d.Monto).HasPrecision(18, 2);
            entity.Property(d => d.Porcentaje).HasPrecision(5, 2);

            // Relación N:1 con Empleado
            entity.HasOne(d => d.Empleado)
                .WithMany()
                .HasForeignKey(d => d.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // NO borrar empleado si tiene deducciones

            // Global query filter para multi-tenancy
            entity.HasQueryFilter(d =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)d.CompanyId == _currentUserService.CompanyId);
        });

        // Conceptos de Nómina: Configuración de Anticipo
        modelBuilder.Entity<Anticipo>(entity =>
        {
            // Índice compuesto para búsquedas por empleado y estado
            entity.HasIndex(a => new { a.EmpleadoId, a.Estado })
                .HasDatabaseName("IX_Anticipo_EmpleadoId_Estado");

            // Índice para búsquedas por fecha de descuento
            entity.HasIndex(a => new { a.FechaDescuento, a.Estado })
                .HasDatabaseName("IX_Anticipo_FechaDescuento_Estado");

            // Configuración de precisión para campos decimales
            entity.Property(a => a.Monto).HasPrecision(18, 2);

            // Relación N:1 con Empleado
            entity.HasOne(a => a.Empleado)
                .WithMany()
                .HasForeignKey(a => a.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict); // NO borrar empleado si tiene anticipos

            // Global query filter para multi-tenancy
            entity.HasQueryFilter(a =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)a.CompanyId == _currentUserService.CompanyId);
        });

        // Conceptos de Nómina: Configuración de PagoPrestamo
        modelBuilder.Entity<PagoPrestamo>(entity =>
        {
            // Índice para búsquedas por préstamo
            entity.HasIndex(pp => pp.PrestamoId)
                .HasDatabaseName("IX_PagoPrestamo_PrestamoId");

            // Configuración de precisión para campos decimales
            entity.Property(pp => pp.MontoPagado).HasPrecision(18, 2);
            entity.Property(pp => pp.SaldoAnterior).HasPrecision(18, 2);
            entity.Property(pp => pp.SaldoNuevo).HasPrecision(18, 2);
        });

        // Asistencia: Configuración de HoraExtra
        modelBuilder.Entity<HoraExtra>(entity =>
        {
            entity.HasIndex(h => new { h.EmpleadoId, h.Fecha })
                .HasDatabaseName("IX_HoraExtra_EmpleadoId_Fecha");

            entity.HasIndex(h => new { h.EstaAprobada, h.Fecha })
                .HasDatabaseName("IX_HoraExtra_EstaAprobada_Fecha");

            entity.Property(h => h.CantidadHoras).HasPrecision(5, 2);
            entity.Property(h => h.FactorMultiplicador).HasPrecision(4, 2);
            entity.Property(h => h.MontoCalculado).HasPrecision(18, 2);

            entity.HasOne(h => h.Empleado)
                .WithMany()
                .HasForeignKey(h => h.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(h =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)h.CompanyId == _currentUserService.CompanyId);
        });

        // Asistencia: Configuración de Ausencia
        modelBuilder.Entity<Ausencia>(entity =>
        {
            entity.HasIndex(a => new { a.EmpleadoId, a.FechaInicio })
                .HasDatabaseName("IX_Ausencia_EmpleadoId_FechaInicio");

            entity.Property(a => a.DiasAusencia).HasPrecision(5, 2);
            entity.Property(a => a.MontoDescontado).HasPrecision(18, 2);

            entity.HasOne(a => a.Empleado)
                .WithMany()
                .HasForeignKey(a => a.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(a =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)a.CompanyId == _currentUserService.CompanyId);
        });

        // Asistencia: Configuración de SolicitudVacaciones
        modelBuilder.Entity<SolicitudVacaciones>(entity =>
        {
            entity.HasIndex(v => new { v.EmpleadoId, v.Estado })
                .HasDatabaseName("IX_SolicitudVacaciones_EmpleadoId_Estado");

            entity.HasIndex(v => new { v.FechaInicio, v.FechaFin })
                .HasDatabaseName("IX_SolicitudVacaciones_Fechas");

            entity.Property(v => v.DiasProporcionales).HasPrecision(5, 2);

            entity.HasOne(v => v.Empleado)
                .WithMany()
                .HasForeignKey(v => v.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(v =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)v.CompanyId == _currentUserService.CompanyId);
        });

        // Asistencia: Configuración de SaldoVacaciones
        modelBuilder.Entity<SaldoVacaciones>(entity =>
        {
            // Índice único: un empleado solo puede tener un saldo de vacaciones
            entity.HasIndex(s => new { s.CompanyId, s.EmpleadoId })
                .IsUnique()
                .HasDatabaseName("IX_SaldoVacaciones_CompanyId_EmpleadoId");

            entity.Property(s => s.DiasAcumulados).HasPrecision(6, 2);
            entity.Property(s => s.DiasTomados).HasPrecision(6, 2);
            entity.Property(s => s.DiasDisponibles).HasPrecision(6, 2);

            entity.HasOne(s => s.Empleado)
                .WithMany()
                .HasForeignKey(s => s.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(s =>
                _currentUserService == null ||
                _currentUserService.CompanyId == null ||
                (int?)s.CompanyId == _currentUserService.CompanyId);
        });
    }
}