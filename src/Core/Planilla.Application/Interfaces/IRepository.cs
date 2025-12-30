using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vorluno.Planilla.Application.Interfaces
{
    /// <summary>
    /// Define un contrato genérico para el patrón de repositorio, abstrayendo las operaciones de persistencia de datos.
    /// NOTA: Este es el contrato básico. Para operaciones avanzadas (includes, filtros), use la implementación concreta Repository<T>.
    /// </summary>
    /// <typeparam name="T">El tipo de la entidad para la cual este repositorio opera. Debe ser una clase.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene una entidad por su identificador único (ID) de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la entidad a buscar.</param>
        /// <returns>Una tarea que retorna la entidad encontrada, o null si no se encuentra.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene todas las entidades de un tipo específico de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que retorna una colección de todas las entidades.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Agrega una nueva entidad al conjunto de datos de forma asíncrona.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Marca una entidad existente como modificada.
        /// </summary>
        /// <remarks>
        /// Esta operación solo marca el estado de la entidad en el rastreador de cambios.
        /// Los cambios se persistirán en la base de datos al llamar a CompleteAsync() en la Unidad de Trabajo.
        /// </remarks>
        /// <param name="entity">La entidad a actualizar.</param>
        void Update(T entity);

        /// <summary>
        /// Marca una entidad existente como eliminada.
        /// </summary>
        /// <remarks>
        /// Esta operación solo marca el estado de la entidad en el rastreador de cambios.
        /// Los cambios se persistirán en la base de datos al llamar a CompleteAsync() en la Unidad de Trabajo.
        /// </remarks>
        /// <param name="entity">La entidad a eliminar.</param>
        void Remove(T entity);
    }
}