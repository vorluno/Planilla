// ====================================================================
// Planilla - Ficha anual de ISR
// Reproduce el libro que lleva el contador a mano: una fila por corrida,
// con la proyección del año y el impuesto que se fue reteniendo.
// ====================================================================

namespace Vorluno.Planilla.Application.DTOs;

/// <summary>Una corrida dentro de la ficha anual de ISR de un empleado.</summary>
public class FilaFichaIsrDto
{
    /// <summary>Número de corrida del empleado en el año.</summary>
    public int Periodo { get; set; }

    public DateTime FechaPago { get; set; }

    /// <summary>Descripción de la corrida: "Quincena 3" o "Décimo (abril)".</summary>
    public string Concepto { get; set; } = string.Empty;

    /// <summary>true si la fila es una partida de décimo y no una planilla regular.</summary>
    public bool EsDecimo { get; set; }

    public decimal Bruto { get; set; }
    public decimal SeguroSocial { get; set; }

    /// <summary>Bruto menos Seguro Social: la base sobre la que se proyecta.</summary>
    public decimal Gravable { get; set; }

    public decimal GravableAcumulado { get; set; }
    public decimal DecimoAcumulado { get; set; }

    /// <summary>Períodos corridos incluyendo lo que aporta el décimo ya pagado.</summary>
    public decimal PeriodoEquivalente { get; set; }

    public decimal IngresoAnualProyectado { get; set; }
    public decimal IsrAnualProyectado { get; set; }

    /// <summary>Impuesto que debería llevar retenido a esta fecha.</summary>
    public decimal IsrDebidoAcumulado { get; set; }

    /// <summary>Lo que el motor calcula descontar en esta corrida.</summary>
    public decimal IsrCalculado { get; set; }

    /// <summary>Lo que de verdad se le descontó y quedó guardado en la planilla.</summary>
    public decimal IsrRetenido { get; set; }

    public decimal IsrRetenidoAcumulado { get; set; }
}

/// <summary>Ficha anual de ISR de un empleado.</summary>
public class FichaIsrAnualDto
{
    public int EmpleadoId { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public string? Cedula { get; set; }
    public int Anio { get; set; }

    /// <summary>Frecuencia de pago con la que se hace la proyección.</summary>
    public string Frecuencia { get; set; } = string.Empty;

    /// <summary>Períodos equivalentes del año: 26 en quincenal, 13 en mensual.</summary>
    public decimal PeriodosEquivalentes { get; set; }

    // Saldos que el empleado traía de otro sistema
    public decimal IngresoGravableInicial { get; set; }
    public decimal DecimoInicial { get; set; }
    public decimal IsrRetenidoInicial { get; set; }

    public List<FilaFichaIsrDto> Filas { get; set; } = new();

    // Totales del año
    public decimal TotalGravable { get; set; }
    public decimal TotalDecimo { get; set; }
    public decimal TotalIsrRetenido { get; set; }

    /// <summary>
    /// Impuesto que le corresponde según lo que realmente ganó en el año.
    /// Solo tiene sentido leerlo con el año cerrado.
    /// </summary>
    public decimal IsrDelAnioSegunIngresoReal { get; set; }

    /// <summary>Diferencia entre lo retenido y el impuesto real. Positivo: se retuvo de más.</summary>
    public decimal DiferenciaRetenido { get; set; }
}
