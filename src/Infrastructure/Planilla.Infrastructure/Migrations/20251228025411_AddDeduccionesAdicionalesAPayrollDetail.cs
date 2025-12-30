using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vorluno.Planilla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeduccionesAdicionalesAPayrollDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Anticipos",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeduccionesFijas",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OtrasDeduccionesDetalle",
                table: "PayrollDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Prestamos",
                table: "PayrollDetails",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Anticipos",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "DeduccionesFijas",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "OtrasDeduccionesDetalle",
                table: "PayrollDetails");

            migrationBuilder.DropColumn(
                name: "Prestamos",
                table: "PayrollDetails");
        }
    }
}
