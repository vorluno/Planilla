// ====================================================================
// Planilla - DTOs para Reporte ISR
// Creado: 2025-12-28
// Descripción: DTOs para generar reportes de Impuesto Sobre la Renta
// ====================================================================

namespace Vorluno.Planilla.Application.DTOs.Reportes;

/// <summary>
/// DTO principal para el reporte de ISR
/// </summary>
public record ReporteIsrDto(
    string NombreEmpresa,
    string Ruc,
    string Periodo,
    int AñoFiscal,
    DateTime FechaGeneracion,
    List<EmpleadoIsrDto> Empleados,
    TotalesIsrDto Totales
);

/// <summary>
/// DTO para los datos de ISR de un empleado
/// </summary>
public record EmpleadoIsrDto(
    string Cedula,
    string NombreCompleto,
    decimal IngresoAnualProyectado,
    int Dependientes,
    decimal DeduccionDependientes,
    decimal BaseGravable,
    decimal IsrAnual,
    decimal IsrPeriodo
);

/// <summary>
/// DTO para los totales del reporte ISR
/// </summary>
public record TotalesIsrDto(
    decimal TotalIngresos,
    decimal TotalDeducciones,
    decimal TotalIsrAnual,
    decimal TotalIsrPeriodo
);
