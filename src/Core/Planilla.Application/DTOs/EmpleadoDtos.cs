using System.ComponentModel.DataAnnotations;

namespace Vorluno.Planilla.Application.DTOs
{
    /// <summary>
    /// DTO para visualizar los datos de un empleado. Contiene los campos seguros para mostrar al exterior.
    /// </summary>
    public record EmpleadoVerDto(
        int Id,
        string Nombre,
        string Apellido,
        string NumeroIdentificacion,
        decimal SalarioBase,
        DateTime FechaContratacion,
        bool EstaActivo,
        int? DepartamentoId,
        string? DepartamentoNombre,
        int? PosicionId,
        string? PosicionNombre
    );

    /// <summary>
    /// DTO utilizado para crear un nuevo empleado. Solo incluye los campos que el cliente puede proporcionar.
    /// </summary>
    public record EmpleadoCrearDto(
        [Required]
        [StringLength(100)]
        string Nombre,

        [Required]
        [StringLength(100)]
        string Apellido,

        [Required]
        [StringLength(20)]
        string NumeroIdentificacion,

        [Range(0.01, double.MaxValue)]
        decimal SalarioBase,

        int? DepartamentoId,

        int? PosicionId
    );

    /// <summary>
    /// DTO utilizado para actualizar un empleado existente.
    /// </summary>
    public record EmpleadoActualizarDto(
        [Required]
        [StringLength(100)]
        string Nombre,

        [Required]
        [StringLength(100)]
        string Apellido,

        [Range(0.01, double.MaxValue)]
        decimal SalarioBase,

        bool EstaActivo,

        int? DepartamentoId,

        int? PosicionId
    );
}