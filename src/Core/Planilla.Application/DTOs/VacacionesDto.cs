using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

public record VacacionesDto(
    int Id,
    int EmpleadoId,
    string EmpleadoNombre,
    DateTime FechaInicio,
    DateTime FechaFin,
    int DiasVacaciones,
    decimal DiasProporcionales,
    EstadoVacaciones Estado,
    string EstadoNombre,
    DateTime FechaSolicitud,
    string? AprobadoPor,
    DateTime? FechaAprobacion,
    string? MotivoRechazo
);
