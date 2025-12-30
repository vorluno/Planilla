using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

public record CreateHoraExtraRequest(
    int EmpleadoId,
    DateTime Fecha,
    TipoHoraExtra TipoHoraExtra,
    TimeSpan HoraInicio,
    TimeSpan HoraFin,
    string Motivo
);
