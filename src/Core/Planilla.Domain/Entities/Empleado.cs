using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vorluno.Planilla.Domain.Entities;

public class Empleado
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100, ErrorMessage = "El apellido no puede tener más de 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de identificación es obligatorio.")]
    [StringLength(20, ErrorMessage = "El número de identificación no puede tener más de 20 caracteres.")]
    public string NumeroIdentificacion { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    [Range(0, double.MaxValue, ErrorMessage = "El salario base no puede ser negativo.")]
    public decimal SalarioBase { get; set; }

    public DateTime FechaContratacion { get; set; }

    public bool EstaActivo { get; set; } = true;

    // ====================================================================
    // Phase E: Campos para cálculo de planilla
    // ====================================================================

    /// <summary>
    /// ID de compañía para multi-tenancy
    /// </summary>
    public int CompanyId { get; set; } = 1;

    /// <summary>
    /// Departamento al que pertenece el empleado (opcional)
    /// </summary>
    public int? DepartamentoId { get; set; }

    /// <summary>
    /// Posición o cargo del empleado (opcional)
    /// </summary>
    public int? PosicionId { get; set; }

    /// <summary>
    /// Años cotizados en CSS (determina tope CSS: 25 años ? intermedio, 30 años ? alto)
    /// </summary>
    public int YearsCotized { get; set; } = 0;

    /// <summary>
    /// Salario promedio últimos 10 años (para determinar tope CSS alto)
    /// </summary>
    [Column(TypeName = "decimal(18, 2)")]
    public decimal AverageSalaryLast10Years { get; set; } = 0;

    /// <summary>
    /// Porcentaje de riesgo profesional CSS: 0.56 (bajo), 2.50 (medio), 5.39 (alto)
    /// </summary>
    [Column(TypeName = "decimal(5, 2)")]
    public decimal CssRiskPercentage { get; set; } = 0.56m;

    /// <summary>
    /// Frecuencia de pago: "Quincenal", "Mensual", "Semanal"
    /// </summary>
    [StringLength(20)]
    public string PayFrequency { get; set; } = "Quincenal";

    /// <summary>
    /// Número de dependientes declarados (máximo 3 para deducción ISR)
    /// </summary>
    public int Dependents { get; set; } = 0;

    /// <summary>
    /// Indica si el empleado está sujeto a CSS
    /// </summary>
    public bool IsSubjectToCss { get; set; } = true;

    /// <summary>
    /// Indica si el empleado está sujeto a Seguro Educativo
    /// </summary>
    public bool IsSubjectToEducationalInsurance { get; set; } = true;

    /// <summary>
    /// Indica si el empleado está sujeto a Impuesto Sobre la Renta (ISR)
    /// </summary>
    public bool IsSubjectToIncomeTax { get; set; } = true;

    // Propiedad de navegación: un empleado puede tener muchos recibos de sueldo.
    // La clase ReciboDeSueldo ya está implementada y representa cada uno de ellos.
    public virtual ICollection<ReciboDeSueldo> RecibosDeSueldo { get; set; } = new List<ReciboDeSueldo>();

    // Navigation properties para Departamento y Posicion
    public virtual Departamento? Departamento { get; set; }
    public virtual Posicion? Posicion { get; set; }
}