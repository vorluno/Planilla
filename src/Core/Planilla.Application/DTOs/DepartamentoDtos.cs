using System.ComponentModel.DataAnnotations;

namespace Vorluno.Planilla.Application.DTOs;

/// <summary>
/// DTO para visualizar los datos de un departamento.
/// </summary>
public record DepartamentoVerDto(
    int Id,
    string Nombre,
    string Codigo,
    string? Descripcion,
    bool EstaActivo,
    int? ManagerId,
    string? ManagerNombre,
    int CantidadEmpleados,
    int CantidadPosiciones
);

/// <summary>
/// DTO utilizado para crear un nuevo departamento.
/// </summary>
public record DepartamentoCrearDto(
    [Required]
    [StringLength(100)]
    string Nombre,

    [Required]
    [StringLength(20)]
    string Codigo,

    [StringLength(500)]
    string? Descripcion,

    int? ManagerId
);

/// <summary>
/// DTO utilizado para actualizar un departamento existente.
/// </summary>
public record DepartamentoActualizarDto(
    [Required]
    [StringLength(100)]
    string Nombre,

    [Required]
    [StringLength(20)]
    string Codigo,

    [StringLength(500)]
    string? Descripcion,

    int? ManagerId,

    bool EstaActivo
);
