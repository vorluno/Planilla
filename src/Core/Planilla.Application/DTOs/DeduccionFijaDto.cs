using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para visualización de deducciones fijas.
/// </summary>
public record DeduccionFijaDto(
    int Id,
    int EmpleadoId,
    string EmpleadoNombre,
    TipoDeduccion TipoDeduccion,
    string TipoDeduccionNombre,  // Nombre legible
    string Descripcion,
    decimal Monto,
    decimal? Porcentaje,
    bool EsPorcentaje,
    DateTime FechaInicio,
    DateTime? FechaFin,
    bool EstaActivo,
    string? Referencia,
    int Prioridad
);
