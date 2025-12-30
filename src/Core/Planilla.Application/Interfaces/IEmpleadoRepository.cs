using Vorluno.Planilla.Domain.Entities;

namespace Vorluno.Planilla.Application.Interfaces
{
    /// <summary>
    /// Define el contrato para un repositorio específico de la entidad <see cref="Empleado"/>.
    /// </summary>
    /// <remarks>
    /// Esta interfaz hereda todas las operaciones CRUD estándar de <see cref="IRepository{T}"/>
    /// y sirve como un lugar centralizado para agregar métodos de consulta de datos que son únicos para los empleados.
    /// </remarks>
    public interface IEmpleadoRepository : IRepository<Empleado>
    {
        // Aquí, en el futuro, podemos agregar métodos específicos para Empleados.
        // Ejemplo: Task<Empleado?> GetByNumeroIdentificacionAsync(string numeroIdentificacion);
    }
}