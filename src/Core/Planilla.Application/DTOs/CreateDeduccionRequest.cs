using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para creación de nuevas deducciones fijas.
/// </summary>
public record CreateDeduccionRequest(
    int EmpleadoId,
    TipoDeduccion TipoDeduccion,
    string Descripcion,
    decimal Monto,
    decimal? Porcentaje,
    bool EsPorcentaje,
    DateTime FechaInicio,
    DateTime? FechaFin,
    string? Referencia,
    int Prioridad,
    string? Observaciones
);
