using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Planilla.Domain.Entities;

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

    // Propiedad de navegación: un empleado puede tener muchos recibos de sueldo.
    // La clase ReciboDeSueldo ya está implementada y representa cada uno de ellos.
    public virtual ICollection<ReciboDeSueldo> RecibosDeSueldo { get; set; } = new List<ReciboDeSueldo>();
}