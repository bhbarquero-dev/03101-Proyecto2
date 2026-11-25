using System.ComponentModel.DataAnnotations;

namespace _03101_Proyecto2.Contracts.Proyectos;

/// <summary>
/// Datos para crear (POST /api/proyectos) o editar (PUT /api/proyectos/{id}) un proyecto.
/// El estado no viaja aquí: un proyecto nuevo queda «Activo» y se cierra con DELETE.
/// </summary>
public sealed class ProyectoRequest : IValidatableObject
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre admite como máximo 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción admite como máximo 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateOnly? FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha estimada de fin es obligatoria.")]
    public DateOnly? FechaFinEstimada { get; set; }

    [Required(ErrorMessage = "El responsable es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El responsable es obligatorio.")]
    public int? IdResponsable { get; set; }

    /// <summary>Misma regla que CK_Proyectos_Fechas, validada antes de llegar a la base de datos (CA-04.2).</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaInicio is { } inicio && FechaFinEstimada is { } fin && fin < inicio)
        {
            yield return new ValidationResult(
                "La fecha estimada de fin no puede ser anterior a la fecha de inicio.",
                [nameof(FechaFinEstimada)]);
        }
    }
}
