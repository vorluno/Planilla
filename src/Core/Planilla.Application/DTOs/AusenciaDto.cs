using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.DTOs;

public record AusenciaDto(
    int Id,
    int EmpleadoId,
    string EmpleadoNombre,
    TipoAusencia TipoAusencia,
    string TipoNombre,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal DiasAusencia,
    bool TieneJustificacion,
    bool AfectaSalario,
    decimal? MontoDescontado,
    string Motivo,
    string? DocumentoReferencia
);
