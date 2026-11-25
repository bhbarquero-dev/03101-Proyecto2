using System.ComponentModel.DataAnnotations;
using _03101_Proyecto2.Contracts.Proyectos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _03101_Proyecto2.Web.Models.ViewModels;

/// <summary>
/// Formulario de nuevo proyecto y de edición. Valida en el navegador lo básico (campos obligatorios
/// y longitudes); las reglas de negocio, como la coherencia de fechas, las decide la API (CA-04.2).
/// </summary>
public sealed class ProyectoFormViewModel
{
    /// <summary>Null en un proyecto nuevo.</summary>
    public int? Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre admite como máximo 150 caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción admite como máximo 1000 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateOnly? FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha estimada de fin es obligatoria.")]
    [Display(Name = "Fecha estimada de fin")]
    public DateOnly? FechaFinEstimada { get; set; }

    [Required(ErrorMessage = "Seleccione el responsable.")]
    [Display(Name = "Responsable")]
    public int? IdResponsable { get; set; }

    /// <summary>Opciones del campo Responsable; se cargan de la API, no llegan en el formulario.</summary>
    [ValidateNever]
    public IReadOnlyList<SelectListItem> Responsables { get; set; } = [];

    public ProyectoRequest ARequest() => new()
    {
        Nombre = Nombre,
        Descripcion = Descripcion,
        FechaInicio = FechaInicio,
        FechaFinEstimada = FechaFinEstimada,
        IdResponsable = IdResponsable
    };

    public static ProyectoFormViewModel Desde(ProyectoDto proyecto) => new()
    {
        Id = proyecto.Id,
        Nombre = proyecto.Nombre,
        Descripcion = proyecto.Descripcion,
        FechaInicio = proyecto.FechaInicio,
        FechaFinEstimada = proyecto.FechaFinEstimada,
        IdResponsable = proyecto.IdResponsable
    };
}
