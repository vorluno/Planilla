using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vorluno.Planilla.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del patrón de Unidad de Trabajo que gestiona el ciclo de vida de los repositorios y las transacciones de la base de datos.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        /// <inheritdoc />
        public IEmpleadoRepository Empleados { get; private set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UnitOfWork"/>.
        /// </summary>
        /// <param name="context">El contexto de la base de datos inyectado por el contenedor de dependencias.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // Inicializa cada repositorio, pasándole el mismo contexto de la base de datos.
            // Esto es crucial para que todos los repositorios compartan la misma transacción.
            Empleados = new EmpleadoRepository(_context);

            // Aquí se inicializarían otros repositorios, como RecibosDeSueldoRepository
        }

        /// <inheritdoc />
        public IRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_context);
            }

            return (IRepository<T>)_repositories[type];
        }

        /// <inheritdoc />
        public async Task<int> CompleteAsync()
        {
            // Guarda todos los cambios pendientes (adds, updates, removes) en la base de datos en una única transacción.
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Libera los recursos no administrados utilizados por el DbContext.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}