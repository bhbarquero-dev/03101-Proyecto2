using System.Security.Claims;
using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Contracts.Catalogos;
using _03101_Proyecto2.Contracts.Tareas;
using _03101_Proyecto2.Web.Models.ViewModels;
using _03101_Proyecto2.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _03101_Proyecto2.Web.Controllers;

/// <summary>
/// Gestión de tareas desde el detalle de un proyecto (HU-05). Lo llama wwwroot/js/tareas.js con fetch
/// y devuelve fragmentos HTML: el formulario del diálogo, o la sección de tareas ya actualizada para
/// reemplazarla en la página sin recargarla (CA-05.1 y CA-05.2).
/// </summary>
public sealed class TareasController(
    TareasApiClient tareasApi,
    ProyectosApiClient proyectosApi,
    CatalogosApiClient catalogosApi) : Controller
{
    private const string VistaFormulario = "_FormularioTarea";
    private const string VistaLista = "_TareasProyecto";

    private bool EsAdministrador => User.IsInRole(Roles.Administrador);

    private int IdUsuario =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    /// <summary>Formulario de nueva tarea (sin <paramref name="id"/>) o de edición.</summary>
    [HttpGet]
    public async Task<IActionResult> Formulario(int idProyecto, int? id, CancellationToken ct)
    {
        var modelo = new TareaFormViewModel { IdProyecto = idProyecto };

        if (id is int idTarea)
        {
            var actual = await tareasApi.ObtenerAsync(idTarea, ct);
            if (!actual.Exitoso)
            {
                return NotFound();
            }

            if (!PuedeEditar(actual.Datos!))
            {
                return Forbid();
            }

            modelo.CopiarDe(actual.Datos!);
            modelo.IdEstado = actual.Datos!.IdEstado;
            modelo.IdPrioridad = actual.Datos.IdPrioridad;
            modelo.SoloEstadoYPrioridad = !EsAdministrador;
        }
        else
        {
            // Valores iniciales de una tarea nueva: asignada a quien la crea, pendiente y de prioridad media.
            modelo.IdAsignado = IdUsuario;
            modelo.IdEstado = EstadosIds.Pendiente;
            modelo.IdPrioridad = PrioridadesIds.Media;
        }

        await CargarOpcionesAsync(modelo, ct);
        return PartialView(VistaFormulario, modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(TareaFormViewModel modelo, CancellationToken ct)
    {
        if (modelo.Id is int idTarea)
        {
            var actual = await tareasApi.ObtenerAsync(idTarea, ct);
            if (!actual.Exitoso)
            {
                return NotFound();
            }

            if (!PuedeEditar(actual.Datos!))
            {
                return Forbid();
            }

            if (!EsAdministrador)
            {
                // El Usuario solo envía estado y prioridad; lo demás se toma de la tarea guardada.
                foreach (var campo in ModelState.Keys.Where(k => k is not (nameof(modelo.IdEstado) or nameof(modelo.IdPrioridad))).ToList())
                {
                    ModelState.Remove(campo);
                }

                modelo.CopiarDe(actual.Datos!);
                modelo.SoloEstadoYPrioridad = true;
            }
        }

        if (ModelState.IsValid)
        {
            var resultado = modelo.Id is int id
                ? await tareasApi.ActualizarAsync(id, modelo.ARequest(), ct)
                : await tareasApi.CrearAsync(modelo.ARequest(), ct);

            if (resultado.Exitoso)
            {
                var mensaje = modelo.Id is null
                    ? $"Se creó la tarea «{resultado.Datos!.Titulo}»."
                    : $"Se guardaron los cambios de «{resultado.Datos!.Titulo}».";
                return await ListaAsync(modelo.IdProyecto, mensaje, ct);
            }

            if (resultado.NoEncontrado)
            {
                return NotFound();
            }

            // 400: el diálogo muestra los errores de la API y la tarea conserva sus valores (CA-05.3).
            ModelState.AgregarErroresApi<TareaFormViewModel>(resultado.Errores);
        }

        await CargarOpcionesAsync(modelo, ct);
        Response.StatusCode = StatusCodes.Status400BadRequest;
        return PartialView(VistaFormulario, modelo);
    }

    /// <summary>Cancela la tarea: la API cambia su estado a «Cancelada» y la conserva.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id, int idProyecto, CancellationToken ct)
    {
        var resultado = await tareasApi.CancelarAsync(id, ct);
        if (resultado.NoEncontrado)
        {
            return NotFound();
        }

        var mensaje = resultado.Exitoso
            ? "La tarea quedó cancelada."
            : resultado.Errores?.Values.SelectMany(m => m).FirstOrDefault() ?? "No se pudo cancelar la tarea.";
        return await ListaAsync(idProyecto, mensaje, ct);
    }

    /// <summary>La sección de tareas del proyecto, con los datos que tiene la API en este momento.</summary>
    private async Task<IActionResult> ListaAsync(int idProyecto, string? mensaje, CancellationToken ct)
    {
        var proyecto = await proyectosApi.ObtenerAsync(idProyecto, ct);
        var tareas = await tareasApi.ListarPorProyectoAsync(idProyecto, ct);
        if (!proyecto.Exitoso || !tareas.Exitoso)
        {
            return NotFound();
        }

        return PartialView(VistaLista, TareasProyectoViewModel.Crear(proyecto.Datos!, tareas.Datos!, User, mensaje));
    }

    private bool PuedeEditar(TareaDto tarea) => EsAdministrador || tarea.IdAsignado == IdUsuario;

    /// <summary>Opciones de las listas: usuarios activos, estados de tarea y prioridades.</summary>
    private async Task CargarOpcionesAsync(TareaFormViewModel modelo, CancellationToken ct)
    {
        var catalogos = await catalogosApi.ObtenerAsync(ct);

        var asignados = catalogos.Usuarios.Select(u => new SelectListItem(u.Nombre, u.Id.ToString())).ToList();
        // Al editar, un asignado que ya no está activo se agrega igual, para no cambiarlo sin que se note.
        if (modelo.IdAsignado is int idAsignado && catalogos.Usuarios.All(u => u.Id != idAsignado) && modelo.NombreAsignado is not null)
        {
            asignados.Insert(0, new SelectListItem($"{modelo.NombreAsignado} (inactivo)", idAsignado.ToString()));
        }

        modelo.Asignados = asignados;
        modelo.Estados = catalogos.EstadosTarea.Select(e => new SelectListItem(e.Nombre, e.Id.ToString())).ToList();
        modelo.Prioridades = catalogos.Prioridades.Select(p => new SelectListItem(p.Nombre, p.Id.ToString())).ToList();
        modelo.NombreAsignado ??= catalogos.Usuarios.FirstOrDefault(u => u.Id == modelo.IdAsignado)?.Nombre;
    }
}
