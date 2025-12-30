using Vorluno.Planilla.Domain.Entities;

namespace Vorluno.Planilla.Application.Interfaces
{
    /// <summary>
    /// Define el contrato para el patrón de Unidad de Trabajo (Unit of Work).
    /// Centraliza el acceso a todos los repositorios y gestiona las transacciones de la base de datos para asegurar la integridad de los datos. 
                /// </summary>
                /// <remarks>
                /// Este patrón permite realizar una serie de operaciones en diferentes repositorios y luego confirmar todos los cambios
                /// en una sola transacción atómica mediante el método <see cref="CompleteAsync"/>.
                /// </remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Obtiene una instancia del repositorio de empleados para realizar operaciones de datos sobre la entidad <see cref="Empleado"/>.
        /// </summary>
        /// <value>
        /// El repositorio de empleados.
        /// </value>
        IEmpleadoRepository Empleados { get; }

        // Aquí irían otros repositorios, como IReciboDeSueldoRepository

        /// <summary>
        /// Obtiene una instancia de repositorio genérico para cualquier entidad.
        /// </summary>
        /// <typeparam name="T">El tipo de entidad.</typeparam>
        /// <returns>Una instancia del repositorio para la entidad especificada.</returns>
        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Confirma y guarda todos los cambios realizados en el contexto de la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>
        /// Una tarea que representa la operación asíncrona. El resultado de la tarea contiene el número de objetos de estado escritos en la base de datos.
        /// </returns>
        Task<int> CompleteAsync();
    }
}