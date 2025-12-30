using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vorluno.Planilla.Application.DTOs;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Domain.Entities;
using Vorluno.Planilla.Infrastructure.Data;

namespace Vorluno.Planilla.Web.Controllers;

/// <summary>
/// Controlador de API para gestionar las operaciones CRUD de departamentos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DepartamentosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _context;

    public DepartamentosController(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _context = context;
    }

    /// <summary>
    /// Obtiene una lista de todos los departamentos con conteos de empleados y posiciones.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departamentos = await _context.Departamentos
            .Include(d => d.Manager)
            .Include(d => d.Empleados)
            .Include(d => d.Posiciones)
            .ToListAsync();

        var departamentosDto = departamentos.Select(d => new DepartamentoVerDto(
            d.Id,
            d.Nombre,
            d.Codigo,
            d.Descripcion,
            d.EstaActivo,
            d.ManagerId,
            d.Manager != null ? $"{d.Manager.Nombre} {d.Manager.Apellido}" : null,
            d.Empleados.Count,
            d.Posiciones.Count
        ));

        return Ok(departamentosDto);
    }

    /// <summary>
    /// Obtiene un departamento específico por su ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var departamento = await _context.Departamentos
            .Include(d => d.Manager)
            .Include(d => d.Empleados)
            .Include(d => d.Posiciones)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departamento == null)
        {
            return NotFound();
        }

        var departamentoDto = new DepartamentoVerDto(
            departamento.Id,
            departamento.Nombre,
            departamento.Codigo,
            departamento.Descripcion,
            departamento.EstaActivo,
            departamento.ManagerId,
            departamento.Manager != null ? $"{departamento.Manager.Nombre} {departamento.Manager.Apellido}" : null,
            departamento.Empleados.Count,
            departamento.Posiciones.Count
        );

        return Ok(departamentoDto);
    }

    /// <summary>
    /// Crea un nuevo departamento.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(DepartamentoCrearDto departamentoDto)
    {
        // Verificar que el código sea único para la compañía
        var existe = await _context.Departamentos
            .AnyAsync(d => d.Codigo == departamentoDto.Codigo);

        if (existe)
        {
            return BadRequest(new { message = $"Ya existe un departamento con el código '{departamentoDto.Codigo}'." });
        }

        // Verificar que el manager existe si se proporciona
        if (departamentoDto.ManagerId.HasValue)
        {
            var managerExiste = await _unitOfWork.Empleados.GetByIdAsync(departamentoDto.ManagerId.Value);
            if (managerExiste == null)
            {
                return BadRequest(new { message = "El empleado seleccionado como jefe no existe." });
            }
        }

        var departamento = new Departamento
        {
            Nombre = departamentoDto.Nombre,
            Codigo = departamentoDto.Codigo,
            Descripcion = departamentoDto.Descripcion,
            ManagerId = departamentoDto.ManagerId,
            CompanyId = 1, // TODO: Obtener de ICurrentUserService cuando esté implementado
            CreatedAt = DateTime.UtcNow,
            EstaActivo = true
        };

        await _unitOfWork.Repository<Departamento>().AddAsync(departamento);
        await _unitOfWork.CompleteAsync();

        // Recargar con navegación para el DTO
        departamento = await _context.Departamentos
            .Include(d => d.Manager)
            .FirstOrDefaultAsync(d => d.Id == departamento.Id);

        var departamentoCreadoDto = new DepartamentoVerDto(
            departamento!.Id,
            departamento.Nombre,
            departamento.Codigo,
            departamento.Descripcion,
            departamento.EstaActivo,
            departamento.ManagerId,
            departamento.Manager != null ? $"{departamento.Manager.Nombre} {departamento.Manager.Apellido}" : null,
            0,
            0
        );

        return CreatedAtAction(nameof(GetById), new { id = departamento.Id }, departamentoCreadoDto);
    }

    /// <summary>
    /// Actualiza un departamento existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, DepartamentoActualizarDto departamentoDto)
    {
        var departamento = await _unitOfWork.Repository<Departamento>().GetByIdAsync(id);
        if (departamento == null)
        {
            return NotFound();
        }

        // Verificar que el código sea único (excepto para este departamento)
        var existe = await _context.Departamentos
            .AnyAsync(d => d.Codigo == departamentoDto.Codigo && d.Id != id);

        if (existe)
        {
            return BadRequest(new { message = $"Ya existe otro departamento con el código '{departamentoDto.Codigo}'." });
        }

        // Verificar que el manager existe si se proporciona
        if (departamentoDto.ManagerId.HasValue)
        {
            var managerExiste = await _unitOfWork.Empleados.GetByIdAsync(departamentoDto.ManagerId.Value);
            if (managerExiste == null)
            {
                return BadRequest(new { message = "El empleado seleccionado como jefe no existe." });
            }
        }

        departamento.Nombre = departamentoDto.Nombre;
        departamento.Codigo = departamentoDto.Codigo;
        departamento.Descripcion = departamentoDto.Descripcion;
        departamento.ManagerId = departamentoDto.ManagerId;
        departamento.EstaActivo = departamentoDto.EstaActivo;
        departamento.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Departamento>().Update(departamento);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    /// <summary>
    /// Elimina (desactiva) un departamento.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var departamento = await _context.Departamentos
            .Include(d => d.Posiciones)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (departamento == null)
        {
            return NotFound();
        }

        // Verificar si tiene posiciones activas
        if (departamento.Posiciones.Any(p => p.EstaActivo))
        {
            return BadRequest(new { message = "No se puede eliminar el departamento porque tiene posiciones activas. Desactive primero las posiciones." });
        }

        // Soft delete
        departamento.EstaActivo = false;
        departamento.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Departamento>().Update(departamento);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
