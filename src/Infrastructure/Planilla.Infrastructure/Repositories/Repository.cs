using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Vorluno.Planilla.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación genérica del patrón de repositorio que encapsula la lógica básica de acceso a datos para cualquier entidad.
    /// Utiliza Entity Framework Core para interactuar con la base de datos.
    /// </summary>
    /// <typeparam name="T">El tipo de la entidad para la cual este repositorio opera. Debe ser una clase.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Repository{T}"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos (DbContext) que será utilizado para las operaciones de datos.</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una entidad por su identificador único (ID) de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de la entidad a buscar.</param>
        /// <returns>Una tarea que retorna la entidad encontrada, o null si no se encuentra ninguna entidad con ese ID.</returns>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Obtiene todas las entidades de un tipo específico de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que retorna una colección enumerable de todas las entidades.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Agrega una nueva entidad a la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        /// <summary>
        /// Marca una entidad como eliminada en el contexto. 
        /// Los cambios no se persistirán en la base de datos hasta que se llame a CompleteAsync() en la Unidad de Trabajo.
        /// </summary>
        /// <param name="entity">La entidad a eliminar.</param>
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Marca una entidad como modificada en el contexto.
        /// Los cambios no se persistirán en la base de datos hasta que se llame a CompleteAsync() en la Unidad de Trabajo.
        /// </summary>
        /// <param name="entity">La entidad a actualizar.</param>
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        /// <summary>
        /// Obtiene una entidad por su ID con soporte para eager loading de entidades relacionadas.
        /// </summary>
        /// <param name="id">El ID de la entidad a buscar.</param>
        /// <param name="include">Función para incluir entidades relacionadas.</param>
        /// <returns>La entidad encontrada, o null si no existe.</returns>
        public virtual async Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (include != null)
            {
                query = include(query);
            }

            // Buscar por ID usando reflexión para encontrar la propiedad Id
            var entityType = _context.Model.FindEntityType(typeof(T));
            var primaryKey = entityType?.FindPrimaryKey();
            var keyProperty = primaryKey?.Properties.FirstOrDefault();

            if (keyProperty == null)
            {
                throw new InvalidOperationException($"No se encontró una clave primaria para el tipo {typeof(T).Name}");
            }

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyProperty.Name);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        /// <summary>
        /// Obtiene todas las entidades con soporte para filtrado, eager loading y ordenamiento.
        /// </summary>
        /// <param name="predicate">Expresión de filtrado opcional.</param>
        /// <param name="include">Función para incluir entidades relacionadas.</param>
        /// <param name="orderBy">Función para ordenar los resultados.</param>
        /// <returns>Colección de entidades que cumplen los criterios.</returns>
        public virtual async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Verifica si existe al menos una entidad que cumple la condición especificada.
        /// </summary>
        /// <param name="predicate">Expresión de filtrado.</param>
        /// <returns>True si existe al menos una entidad, False en caso contrario.</returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
    }
}