using System.ComponentModel.DataAnnotations;
using _03101_Proyecto2.Contracts.Tareas;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _03101_Proyecto2.Web.Models.ViewModels;

/// <summary>
/// Formulario de nueva tarea y de edición, que se abre en un diálogo del detalle del proyecto.
/// Las reglas de negocio (estado de tarea, asignado activo, proyecto activo) las decide la API.
/// </summary>
public sealed class TareaFormViewModel
{
    /// <summary>Null en una tarea nueva.</summary>
    public int? Id { get; set; }

    public int IdProyecto { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título admite como máximo 150 caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción admite como máximo 1000 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Seleccione el usuario asignado.")]
    [Display(Name = "Asignado a")]
    public int? IdAsignado { get; set; }

    [Required(ErrorMessage = "Seleccione el estado.")]
    [Display(Name = "Estado")]
    public int? IdEstado { get; set; }

    [Required(ErrorMessage = "Seleccione la prioridad.")]
    [Display(Name = "Prioridad")]
    public int? IdPrioridad { get; set; }

    [Display(Name = "Fecha de vencimiento")]
    public DateOnly? FechaVencimiento { get; set; }

    /// <summary>
    /// El Usuario que edita una tarea suya solo cambia el estado y la prioridad; los demás campos
    /// se muestran como texto. Lo decide el controlador según el rol, nunca llega en el formulario.
    /// </summary>
    [ValidateNever]
    public bool SoloEstadoYPrioridad { get; set; }

    /// <summary>Nombre del asignado, para mostrarlo cuando el campo no se puede cambiar.</summary>
    [ValidateNever]
    public string? NombreAsignado { get; set; }

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Asignados { get; set; } = [];

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Estados { get; set; } = [];

    [ValidateNever]
    public IReadOnlyList<SelectListItem> Prioridades { get; set; } = [];

    public TareaRequest ARequest() => new()
    {
        IdProyecto = IdProyecto,
        Titulo = Titulo,
        Descripcion = Descripcion,
        IdAsignado = IdAsignado,
        IdEstado = IdEstado,
        IdPrioridad = IdPrioridad,
        FechaVencimiento = FechaVencimiento
    };

    /// <summary>Copia los datos de la tarea guardada.</summary>
    public void CopiarDe(TareaDto tarea)
    {
        Id = tarea.Id;
        IdProyecto = tarea.IdProyecto;
        Titulo = tarea.Titulo;
        Descripcion = tarea.Descripcion;
        IdAsignado = tarea.IdAsignado;
        NombreAsignado = tarea.Asignado;
        FechaVencimiento = tarea.FechaVencimiento;
    }
}
