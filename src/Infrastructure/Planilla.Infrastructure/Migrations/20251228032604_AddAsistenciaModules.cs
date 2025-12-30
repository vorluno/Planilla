using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vorluno.Planilla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAsistenciaModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiasAusenciaInjustificada",
                table: "PayrollDetails",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiasVacaciones",
                table: "PayrollDetails",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasExtraDiurnas",
                table: "PayrollDetails",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasExtraDomingoFeriado",
                table: "PayrollDetails",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasExtraNocturnas",
                table: "PayrollDetails",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoDescuentoAusencias",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoHorasExtra",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoVacaciones",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Ausencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TipoAusencia = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasAusencia = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieneJustificacion = table.Column<bool>(type: "bit", nullable: false),
                    DocumentoReferencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfectaSalario = table.Column<bool>(type: "bit", nullable: false),
                    MontoDescontado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AprobadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanillaDetailId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ausencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ausencias_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HorasExtra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoHoraExtra = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    CantidadHoras = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    FactorMultiplicador = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    MontoCalculado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AprobadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaAprobada = table.Column<bool>(type: "bit", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlanillaDetailId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorasExtra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorasExtra_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaldosVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DiasAcumulados = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    DiasTomados = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    DiasDisponibles = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    UltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaldosVacaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasVacaciones = table.Column<int>(type: "int", nullable: false),
                    DiasProporcionales = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AprobadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RechazadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRechazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanillaDetailId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ausencia_EmpleadoId_FechaInicio",
                table: "Ausencias",
                columns: new[] { "EmpleadoId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_HoraExtra_EmpleadoId_Fecha",
                table: "HorasExtra",
                columns: new[] { "EmpleadoId", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_HoraExtra_EstaAprobada_Fecha",
                table: "HorasExtra",
                columns: new[] { "EstaAprobada", "Fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_SaldosVacaciones_EmpleadoId",
                table: "SaldosVacaciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SaldoVacaciones_CompanyId_EmpleadoId",
                table: "SaldosVacaciones",
                columns: new[] { "CompanyId", "EmpleadoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudVacaciones_EmpleadoId_Estado",
                table: "SolicitudesVacaciones",
                columns: new[] { "EmpleadoId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudVacaciones_Fechas",
                table: "SolicitudesVacaciones",
                columns: new[] { "FechaInicio", "FechaFin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ausencias");

            migrationBuilder.DropTable(
                name: "HorasExtra");

            migrationBuilder.DropTable(
                name: "SaldosVacaciones");

            migrationBuilder.DropTable(
                name: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "DiasAusenciaInjustificada",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "DiasVacaciones",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "HorasExtraDiurnas",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "HorasExtraDomingoFeriado",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "HorasExtraNocturnas",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "MontoDescuentoAusencias",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "MontoHorasExtra",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "MontoVacaciones",
                table: "PayrollDetails");
        }
    }
}
