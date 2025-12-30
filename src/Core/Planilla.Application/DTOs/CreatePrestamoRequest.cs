namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para creación de nuevos préstamos.
/// </summary>
public record CreatePrestamoRequest(
    int EmpleadoId,
    string Descripcion,
    decimal MontoOriginal,
    decimal CuotaMensual,
    decimal TasaInteres,
    DateTime FechaInicio,
    int NumeroCuotas,
    string? Referencia,
    string? Observaciones
);
