using System.Security.Claims;
using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Contracts.Catalogos;
using _03101_Proyecto2.Contracts.Proyectos;
using _03101_Proyecto2.Contracts.Tareas;

namespace _03101_Proyecto2.Web.Models.ViewModels;

/// <summary>
/// Sección de tareas del detalle de un proyecto. Se dibuja al abrir la página y se vuelve a dibujar,
/// sin recargarla, cada vez que se crea, edita o cancela una tarea (CA-05.1 y CA-05.2).
/// </summary>
public sealed class TareasProyectoViewModel
{
    public required ProyectoDto Proyecto { get; init; }
    public required IReadOnlyList<TareaDto> Tareas { get; init; }
    public required int IdUsuario { get; init; }
    public required bool EsAdministrador { get; init; }

    /// <summary>Mensaje de confirmación de la última operación, si la hubo.</summary>
    public string? Mensaje { get; init; }

    public bool ProyectoActivo => Proyecto.IdEstado == EstadosIds.Activo;

    public int Completadas => Tareas.Count(t => t.IdEstado == EstadosIds.Completada);

    /// <summary>Se crean tareas solo en un proyecto activo; la API aplica la misma regla.</summary>
    public bool PuedeCrear => ProyectoActivo;

    /// <summary>El Administrador edita cualquier tarea; el Usuario, solo las que tiene asignadas.</summary>
    public bool PuedeEditar(TareaDto tarea) => ProyectoActivo && (EsAdministrador || tarea.IdAsignado == IdUsuario);

    public bool PuedeCancelar(TareaDto tarea) =>
        ProyectoActivo && EsAdministrador && tarea.IdEstado != EstadosIds.Cancelada;

    public static TareasProyectoViewModel Crear(
        ProyectoDto proyecto, IReadOnlyList<TareaDto> tareas, ClaimsPrincipal usuario, string? mensaje = null) => new()
    {
        Proyecto = proyecto,
        Tareas = tareas,
        IdUsuario = int.TryParse(usuario.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0,
        EsAdministrador = usuario.IsInRole(Roles.Administrador),
        Mensaje = mensaje
    };
}
