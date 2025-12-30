// ====================================================================
// Planilla - PayrollStateMachine
// Source: Core360 Stage 3
// Creado: 2025-12-26
// Descripción: Máquina de estados para validar transiciones de planilla
// Implementa reglas de negocio del workflow de planilla
// ====================================================================

using Vorluno.Planilla.Domain.Enums;

namespace Vorluno.Planilla.Application.Services;

/// <summary>
/// Máquina de estados que define y valida las transiciones permitidas en el workflow de planilla.
/// Garantiza que las planillas solo puedan cambiar de estado según reglas de negocio establecidas.
/// </summary>
public class PayrollStateMachine
{
    /// <summary>
    /// Diccionario de transiciones válidas desde cada estado.
    /// Key: Estado actual
    /// Value: Lista de estados a los que puede transicionar
    /// </summary>
    private static readonly Dictionary<PayrollStatus, List<PayrollStatus>> _allowedTransitions = new()
    {
        // Draft: Puede calcularse o cancelarse
        [PayrollStatus.Draft] = new()
        {
            PayrollStatus.Calculated,
            PayrollStatus.Cancelled
        },

        // Calculated: Puede re-calcularse, aprobarse o cancelarse
        [PayrollStatus.Calculated] = new()
        {
            PayrollStatus.Calculated,   // Re-cálculo permitido
            PayrollStatus.Approved,
            PayrollStatus.Cancelled
        },

        // Approved: Solo puede pagarse o cancelarse
        [PayrollStatus.Approved] = new()
        {
            PayrollStatus.Paid,
            PayrollStatus.Cancelled
        },

        // Paid: Estado final, sin transiciones
        [PayrollStatus.Paid] = new(),

        // Cancelled: Estado final, sin transiciones
        [PayrollStatus.Cancelled] = new()
    };

    /// <summary>
    /// Verifica si una transición de estado es válida.
    /// </summary>
    /// <param name="currentStatus">Estado actual de la planilla</param>
    /// <param name="targetStatus">Estado al que se desea transicionar</param>
    /// <returns>true si la transición es válida, false si no lo es</returns>
    public bool CanTransition(PayrollStatus currentStatus, PayrollStatus targetStatus)
    {
        // Validar que el estado actual exista en el diccionario
        if (!_allowedTransitions.ContainsKey(currentStatus))
        {
            return false;
        }

        // Verificar si el estado objetivo está en la lista de transiciones permitidas
        return _allowedTransitions[currentStatus].Contains(targetStatus);
    }

    /// <summary>
    /// Valida una transición de estado y lanza excepción si no es válida.
    /// </summary>
    /// <param name="currentStatus">Estado actual de la planilla</param>
    /// <param name="targetStatus">Estado al que se desea transicionar</param>
    /// <exception cref="InvalidOperationException">Si la transición no es válida</exception>
    public void ValidateTransition(PayrollStatus currentStatus, PayrollStatus targetStatus)
    {
        if (!CanTransition(currentStatus, targetStatus))
        {
            throw new InvalidOperationException(
                $"Transición de estado inválida: {currentStatus} → {targetStatus}. " +
                $"Transiciones permitidas desde {currentStatus}: {GetAllowedTransitionsString(currentStatus)}");
        }
    }

    /// <summary>
    /// Obtiene la lista de estados a los que puede transicionar desde un estado dado.
    /// </summary>
    /// <param name="currentStatus">Estado actual de la planilla</param>
    /// <returns>Lista de estados permitidos</returns>
    public List<PayrollStatus> GetAllowedTransitions(PayrollStatus currentStatus)
    {
        if (!_allowedTransitions.ContainsKey(currentStatus))
        {
            return new List<PayrollStatus>();
        }

        return new List<PayrollStatus>(_allowedTransitions[currentStatus]);
    }

    /// <summary>
    /// Obtiene una representación en string de las transiciones permitidas desde un estado.
    /// Útil para mensajes de error.
    /// </summary>
    /// <param name="currentStatus">Estado actual</param>
    /// <returns>String con las transiciones permitidas (ej: "Calculated, Cancelled")</returns>
    private string GetAllowedTransitionsString(PayrollStatus currentStatus)
    {
        var allowed = GetAllowedTransitions(currentStatus);

        if (allowed.Count == 0)
        {
            return "ninguna (estado final)";
        }

        return string.Join(", ", allowed);
    }

    /// <summary>
    /// Verifica si un estado es final (no permite más transiciones).
    /// </summary>
    /// <param name="status">Estado a verificar</param>
    /// <returns>true si es estado final, false si permite transiciones</returns>
    public bool IsFinalState(PayrollStatus status)
    {
        return GetAllowedTransitions(status).Count == 0;
    }

    /// <summary>
    /// Verifica si una planilla en un estado dado puede modificarse.
    /// Solo Draft y Calculated permiten modificaciones.
    /// </summary>
    /// <param name="status">Estado actual de la planilla</param>
    /// <returns>true si puede modificarse, false si no</returns>
    public bool CanModify(PayrollStatus status)
    {
        return status == PayrollStatus.Draft || status == PayrollStatus.Calculated;
    }

    /// <summary>
    /// Verifica si una planilla en un estado dado puede eliminarse.
    /// Solo Draft y Cancelled pueden eliminarse.
    /// </summary>
    /// <param name="status">Estado actual de la planilla</param>
    /// <returns>true si puede eliminarse, false si no</returns>
    public bool CanDelete(PayrollStatus status)
    {
        return status == PayrollStatus.Draft || status == PayrollStatus.Cancelled;
    }
}
