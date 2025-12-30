using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Domain.Entities;
using Vorluno.Planilla.Infrastructure.Data;

namespace Vorluno.Planilla.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para la entidad <see cref="Empleado"/>.
    /// Hereda la funcionalidad CRUD genérica de la clase <see cref="Repository{T}"/> 
    /// y es el lugar para implementar cualquier método de acceso a datos personalizado para los empleados.
    /// </summary>
    public class EmpleadoRepository : Repository<Empleado>, IEmpleadoRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EmpleadoRepository"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos, que se pasa a la clase base.</param>
        public EmpleadoRepository(ApplicationDbContext context) : base(context)
        {
        }

        // En el futuro, la implementación de métodos como
        // public async Task<Empleado?> GetByNumeroIdentificacionAsync(string numeroIdentificacion)
        // iría aquí.
    }
}