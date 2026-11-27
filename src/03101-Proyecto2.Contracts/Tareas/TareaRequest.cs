using System.ComponentModel.DataAnnotations;

namespace _03101_Proyecto2.Contracts.Tareas;

/// <summary>
/// Datos para crear (POST /api/tareas) o editar (PUT /api/tareas/{id}) una tarea.
/// Que el estado sea de tarea, que la prioridad exista y que el asignado esté activo lo valida
/// la API contra la base de datos, porque esos catálogos no se conocen aquí (CA-05.3).
/// </summary>
public sealed class TareaRequest
{
    [Required(ErrorMessage = "El proyecto es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El proyecto es obligatorio.")]
    public int? IdProyecto { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título admite como máximo 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción admite como máximo 1000 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El asignado es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El asignado es obligatorio.")]
    public int? IdAsignado { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public int? IdEstado { get; set; }

    [Required(ErrorMessage = "La prioridad es obligatoria.")]
    public int? IdPrioridad { get; set; }

    public DateOnly? FechaVencimiento { get; set; }
}
