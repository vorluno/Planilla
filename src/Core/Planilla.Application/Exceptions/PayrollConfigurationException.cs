// ====================================================================
// Planilla - PayrollConfigurationException
// Source: Core360 Stage 4
// Portado: 2025-12-26
// Descripción: Excepción para errores de configuración de planilla
// ====================================================================

namespace Vorluno.Planilla.Application.Exceptions;

/// <summary>
/// Excepción lanzada cuando falta o es inválida la configuración de planilla.
/// Se usa cuando no se encuentra configuración de tasas, brackets, etc.
/// </summary>
/// <remarks>
/// IMPORTANTE: Los servicios de cálculo NO deben usar fallbacks silenciosos.
/// Si falta configuración, se debe lanzar esta excepción explícitamente.
/// </remarks>
public class PayrollConfigurationException : InvalidOperationException
{
    /// <summary>
    /// Inicializa una nueva instancia de PayrollConfigurationException con un mensaje de error.
    /// </summary>
    /// <param name="message">Mensaje que describe el error de configuración</param>
    public PayrollConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de PayrollConfigurationException con un mensaje de error
    /// y una referencia a la excepción interna que causó esta excepción.
    /// </summary>
    /// <param name="message">Mensaje que describe el error de configuración</param>
    /// <param name="innerException">Excepción que causó este error</param>
    public PayrollConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
