using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vorluno.Planilla.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartamentosYPosiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "Empleados",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PosicionId",
                table: "Empleados",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departamentos_Empleados_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Posiciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstaActivo = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    DepartamentoId = table.Column<int>(type: "int", nullable: false),
                    SalarioMinimo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalarioMaximo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NivelRiesgo = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posiciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posiciones_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_PosicionId",
                table: "Empleados",
                column: "PosicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Departamento_CompanyId_Codigo",
                table: "Departamentos",
                columns: new[] { "CompanyId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_ManagerId",
                table: "Departamentos",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Posicion_CompanyId_Codigo",
                table: "Posiciones",
                columns: new[] { "CompanyId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posiciones_DepartamentoId",
                table: "Posiciones",
                column: "DepartamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Posiciones_PosicionId",
                table: "Empleados",
                column: "PosicionId",
                principalTable: "Posiciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Posiciones_PosicionId",
                table: "Empleados");

            migrationBuilder.DropTable(
                name: "Posiciones");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_PosicionId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "PosicionId",
                table: "Empleados");
        }
    }
}
