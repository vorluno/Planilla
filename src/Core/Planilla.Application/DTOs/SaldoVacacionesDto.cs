namespace Vorluno.Planilla.Application.DTOs;

public record SaldoVacacionesDto(
    int EmpleadoId,
    string EmpleadoNombre,
    decimal DiasAcumulados,
    decimal DiasTomados,
    decimal DiasDisponibles,
    DateTime UltimaActualizacion,
    DateTime PeriodoInicio,
    DateTime PeriodoFin
);
