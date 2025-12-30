namespace Vorluno.Planilla.Application.DTOs;

public record CreateVacacionesRequest(
    int EmpleadoId,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Observaciones
);
