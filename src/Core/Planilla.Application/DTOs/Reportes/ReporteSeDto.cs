// ====================================================================
// Planilla - DTOs para Reporte Seguro Educativo
// Creado: 2025-12-28
// Descripción: DTOs para generar reportes de Seguro Educativo
// ====================================================================

namespace Vorluno.Planilla.Application.DTOs.Reportes;

/// <summary>
/// DTO principal para el reporte de Seguro Educativo
/// </summary>
public record ReporteSeDto(
    string NombreEmpresa,
    string Ruc,
    string Periodo,
    DateTime FechaGeneracion,
    List<EmpleadoSeDto> Empleados,
    TotalesSeDto Totales
);

/// <summary>
/// DTO para los datos de SE de un empleado
/// </summary>
public record EmpleadoSeDto(
    string Cedula,
    string NombreCompleto,
    decimal SalarioBruto,
    decimal SeEmpleado,
    decimal SePatrono,
    decimal TotalSe
);

/// <summary>
/// DTO para los totales del reporte SE
/// </summary>
public record TotalesSeDto(
    decimal TotalSalarios,
    decimal TotalSeEmpleado,
    decimal TotalSePatrono,
    decimal GranTotal
);
