using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vorluno.Planilla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGastosRepresentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GastoRepresentacion",
                table: "PayrollDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IsrGastoRepresentacion",
                table: "PayrollDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GastoRepresentacionMensual",
                table: "Empleados",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GastoRepresentacionInicial",
                table: "AcumuladosFiscalesEmpleados",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GastoRepresentacionProcesado",
                table: "AcumuladosFiscalesEmpleados",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IsrGastoRepresentacionInicial",
                table: "AcumuladosFiscalesEmpleados",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IsrGastoRepresentacionProcesado",
                table: "AcumuladosFiscalesEmpleados",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GastoRepresentacion",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "IsrGastoRepresentacion",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "GastoRepresentacionMensual",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "GastoRepresentacionInicial",
                table: "AcumuladosFiscalesEmpleados");

            migrationBuilder.DropColumn(
                name: "GastoRepresentacionProcesado",
                table: "AcumuladosFiscalesEmpleados");

            migrationBuilder.DropColumn(
                name: "IsrGastoRepresentacionInicial",
                table: "AcumuladosFiscalesEmpleados");

            migrationBuilder.DropColumn(
                name: "IsrGastoRepresentacionProcesado",
                table: "AcumuladosFiscalesEmpleados");
        }
    }
}
