// ====================================================================
// Planilla - PlanillaService
// Creado: 2025-12-27
// Descripción: Servicio de lógica de negocio para operaciones de planilla
// Integra cálculos de deducciones adicionales (préstamos, deducciones fijas, anticipos)
// ====================================================================

using Vorluno.Planilla.Application.Interfaces;
using Vorluno.Planilla.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vorluno.Planilla.Application.Services
{
    /// <summary>
    /// Proporciona la lógica de negocio central para las operaciones de la planilla.
    /// Orquesta el uso de repositorios a través de la Unidad de Trabajo para ejecutar los casos de uso.
    /// Integra cálculos de deducciones adicionales con el sistema de planilla.
    /// </summary>
    public class PlanillaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayrollCalculationOrchestratorPortable _orchestrator;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PlanillaService"/>.
        /// </summary>
        /// <param name="unitOfWork">La unidad de trabajo que proporciona acceso a los repositorios.</param>
        /// <param name="orchestrator">Orchestrador de cálculo de planilla</param>
        public PlanillaService(
            IUnitOfWork unitOfWork,
            PayrollCalculationOrchestratorPortable orchestrator)
        {
            _unitOfWork = unitOfWork;
            _orchestrator = orchestrator;
        }

        /// <summary>
        /// Obtiene todos los empleados registrados en el sistema.
        /// </summary>
        /// <returns>Una colección de entidades Empleado.</returns>
        public async Task<IEnumerable<Empleado>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.Empleados.GetAllAsync();
        }

        // Aquí irían otros métodos de negocio complejos, como:
        // public async Task<ReciboDeSueldo> CalcularYGuardarReciboSueldoAsync(int empleadoId, ...)
    }
}