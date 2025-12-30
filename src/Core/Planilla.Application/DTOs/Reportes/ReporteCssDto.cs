// ====================================================================
// Planilla - DTOs para Reporte CSS
// Creado: 2025-12-28
// Descripción: DTOs para generar reportes de Caja de Seguro Social
// ====================================================================

namespace Vorluno.Planilla.Application.DTOs.Reportes;

/// <summary>
/// DTO principal para el reporte de CSS
/// </summary>
public record ReporteCssDto(
    string NombreEmpresa,
    string Ruc,
    string Periodo,
    DateTime FechaGeneracion,
    List<EmpleadoCssDto> Empleados,
    TotalesCssDto Totales
);

/// <summary>
/// DTO para los datos de CSS de un empleado
/// </summary>
public record EmpleadoCssDto(
    string Cedula,
    string NombreCompleto,
    decimal SalarioBruto,
    decimal BaseCss,
    decimal CssEmpleado,
    decimal CssPatrono,
    decimal RiesgoProfesional,
    decimal TotalCss
);

/// <summary>
/// DTO para los totales del reporte CSS
/// </summary>
public record TotalesCssDto(
    decimal TotalSalarios,
    decimal TotalCssEmpleado,
    decimal TotalCssPatrono,
    decimal TotalRiesgo,
    decimal GranTotal
);
