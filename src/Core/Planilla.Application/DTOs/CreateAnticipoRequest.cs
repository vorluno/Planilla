namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para creación de solicitudes de anticipo.
/// </summary>
public record CreateAnticipoRequest(
    int EmpleadoId,
    decimal Monto,
    DateTime FechaDescuento,
    string Motivo
);
