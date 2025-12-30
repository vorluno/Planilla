using Vorluno.Planilla.Domain.Entities;

namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para visualización de anticipos de salario.
/// </summary>
public record AnticipoDto(
    int Id,
    int EmpleadoId,
    string EmpleadoNombre,
    decimal Monto,
    DateTime FechaSolicitud,
    DateTime? FechaAprobacion,
    DateTime FechaDescuento,
    EstadoAnticipo Estado,
    string EstadoNombre,  // Nombre legible
    string Motivo,
    string? AprobadoPor
);
