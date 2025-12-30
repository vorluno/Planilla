using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vorluno.Planilla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollFieldsToEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageSalaryLast10Years",
                table: "Empleados",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Empleados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CssRiskPercentage",
                table: "Empleados",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Dependents",
                table: "Empleados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubjectToCss",
                table: "Empleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubjectToEducationalInsurance",
                table: "Empleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubjectToIncomeTax",
                table: "Empleados",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PayFrequency",
                table: "Empleados",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "YearsCotized",
                table: "Empleados",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageSalaryLast10Years",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "CssRiskPercentage",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "Dependents",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "IsSubjectToCss",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "IsSubjectToEducationalInsurance",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "IsSubjectToIncomeTax",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "PayFrequency",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "YearsCotized",
                table: "Empleados");
        }
    }
}
