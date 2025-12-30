using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Application.DTOs;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Domain.Entities;
using Vorluno.Planilla.Domain.Enums;
using Vorluno.Planilla.Infrastructure.Data;

namespace Vorluno.Planilla.Web.Controllers;

/// <summary>
/// Controlador para gestionar ausencias de empleados
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AusenciasController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public AusenciasController(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    /// <summary>
    /// Obtiene lista de ausencias con filtros
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? empleadoId = null,
        [FromQuery] TipoAusencia? tipo = null,
        [FromQuery] DateTime? fechaInicio = null,
        [FromQuery] DateTime? fechaFin = null)
    {
        var query = _context.Ausencias
            .Include(a => a.Empleado)
            .AsQueryable();

        if (empleadoId.HasValue)
            query = query.Where(a => a.EmpleadoId == empleadoId.Value);

        if (tipo.HasValue)
            query = query.Where(a => a.TipoAusencia == tipo.Value);

        if (fechaInicio.HasValue)
            query = query.Where(a => a.FechaInicio >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(a => a.FechaFin <= fechaFin.Value);

        var ausencias = await query.OrderByDescending(a => a.FechaInicio).ToListAsync();

        var dtos = ausencias.Select(MapToDto);
        return Ok(dtos);
    }

    /// <summary>
    /// Obtiene una ausencia por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ausencia = await _context.Ausencias
            .Include(a => a.Empleado)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (ausencia == null)
            return NotFound();

        return Ok(MapToDto(ausencia));
    }

    /// <summary>
    /// Obtiene ausencias de un empleado
    /// </summary>
    [HttpGet("empleado/{empleadoId}")]
    public async Task<IActionResult> GetByEmpleado(int empleadoId)
    {
        var ausencias = await _context.Ausencias
            .Include(a => a.Empleado)
            .Where(a => a.EmpleadoId == empleadoId)
            .OrderByDescending(a => a.FechaInicio)
            .ToListAsync();

        var dtos = ausencias.Select(MapToDto);
        return Ok(dtos);
    }

    /// <summary>
    /// Obtiene tipos de ausencia
    /// </summary>
    [HttpGet("tipos")]
    public IActionResult GetTipos()
    {
        var tipos = new[]
        {
            new { Id = (int)TipoAusencia.Injustificada, Nombre = "Injustificada", AfectaSalario = true },
            new { Id = (int)TipoAusencia.Enfermedad, Nombre = "Enfermedad", AfectaSalario = false },
            new { Id = (int)TipoAusencia.Permiso, Nombre = "Permiso", AfectaSalario = false },
            new { Id = (int)TipoAusencia.Licencia, Nombre = "Licencia", AfectaSalario = false },
            new { Id = (int)TipoAusencia.Suspension, Nombre = "Suspensión", AfectaSalario = true }
        };

        return Ok(tipos);
    }

    /// <summary>
    /// Crea una nueva ausencia
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateAusenciaRequest request)
    {
        var empleado = await _unitOfWork.Repository<Empleado>().GetByIdAsync(request.EmpleadoId);
        if (empleado == null)
            return BadRequest(new { message = "El empleado especificado no existe." });

        // Calcular días de ausencia
        var diasAusencia = CalcularDiasAusencia(request.FechaInicio, request.FechaFin);

        // Determinar si afecta salario según el tipo
        var afectaSalario = request.TipoAusencia == TipoAusencia.Injustificada ||
                           request.TipoAusencia == TipoAusencia.Suspension;

        var ausencia = new Ausencia
        {
            EmpleadoId = request.EmpleadoId,
            TipoAusencia = request.TipoAusencia,
            FechaInicio = request.FechaInicio.Date,
            FechaFin = request.FechaFin.Date,
            DiasAusencia = diasAusencia,
            Motivo = request.Motivo,
            TieneJustificacion = request.TieneJustificacion,
            DocumentoReferencia = request.DocumentoReferencia,
            AfectaSalario = afectaSalario,
            CompanyId = 1,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Ausencia>().AddAsync(ausencia);
        await _unitOfWork.CompleteAsync();

        // Recargar con navegación
        ausencia = await _context.Ausencias
            .Include(a => a.Empleado)
            .FirstOrDefaultAsync(a => a.Id == ausencia.Id);

        return CreatedAtAction(nameof(GetById), new { id = ausencia!.Id }, MapToDto(ausencia));
    }

    /// <summary>
    /// Actualiza una ausencia
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateAusenciaRequest request)
    {
        var ausencia = await _unitOfWork.Repository<Ausencia>().GetByIdAsync(id);
        if (ausencia == null)
            return NotFound();

        if (ausencia.PlanillaDetailId != null)
            return BadRequest(new { message = "No se puede modificar una ausencia ya procesada en planilla." });

        var diasAusencia = CalcularDiasAusencia(request.FechaInicio, request.FechaFin);
        var afectaSalario = request.TipoAusencia == TipoAusencia.Injustificada ||
                           request.TipoAusencia == TipoAusencia.Suspension;

        ausencia.TipoAusencia = request.TipoAusencia;
        ausencia.FechaInicio = request.FechaInicio.Date;
        ausencia.FechaFin = request.FechaFin.Date;
        ausencia.DiasAusencia = diasAusencia;
        ausencia.Motivo = request.Motivo;
        ausencia.TieneJustificacion = request.TieneJustificacion;
        ausencia.DocumentoReferencia = request.DocumentoReferencia;
        ausencia.AfectaSalario = afectaSalario;
        ausencia.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Ausencia>().Update(ausencia);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina una ausencia
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ausencia = await _context.Ausencias.FindAsync(id);
        if (ausencia == null)
            return NotFound();

        if (ausencia.PlanillaDetailId != null)
            return BadRequest(new { message = "No se puede eliminar una ausencia ya procesada en planilla." });

        _context.Ausencias.Remove(ausencia);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Métodos privados
    private decimal CalcularDiasAusencia(DateTime inicio, DateTime fin)
    {
        var dias = (fin.Date - inicio.Date).Days + 1;
        return dias;
    }

    private string ObtenerNombreTipo(TipoAusencia tipo)
    {
        return tipo switch
        {
            TipoAusencia.Injustificada => "Injustificada",
            TipoAusencia.Enfermedad => "Enfermedad",
            TipoAusencia.Permiso => "Permiso",
            TipoAusencia.Licencia => "Licencia",
            TipoAusencia.Suspension => "Suspensión",
            _ => "Desconocido"
        };
    }

    private AusenciaDto MapToDto(Ausencia ausencia)
    {
        return new AusenciaDto(
            ausencia.Id,
            ausencia.EmpleadoId,
            $"{ausencia.Empleado.Nombre} {ausencia.Empleado.Apellido}",
            ausencia.TipoAusencia,
            ObtenerNombreTipo(ausencia.TipoAusencia),
            ausencia.FechaInicio,
            ausencia.FechaFin,
            ausencia.DiasAusencia,
            ausencia.TieneJustificacion,
            ausencia.AfectaSalario,
            ausencia.MontoDescontado,
            ausencia.Motivo,
            ausencia.DocumentoReferencia
        );
    }
}
