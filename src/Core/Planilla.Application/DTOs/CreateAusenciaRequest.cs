using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

public record CreateAusenciaRequest(
    int EmpleadoId,
    TipoAusencia TipoAusencia,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Motivo,
    bool TieneJustificacion,
    string? DocumentoReferencia
);
