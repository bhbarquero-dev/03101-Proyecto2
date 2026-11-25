using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Contracts.Proyectos;
using _03101_Proyecto2.Web.Models.ViewModels;
using _03101_Proyecto2.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _03101_Proyecto2.Web.Controllers;

/// <summary>
/// Mantenimiento de proyectos (HU-04). Cualquier usuario consulta los proyectos que la API le
/// permite ver (CA-07.2); crear, editar y cerrar es solo del Administrador.
/// </summary>
public sealed class ProyectosController(ProyectosApiClient proyectosApi, CatalogosApiClient catalogosApi) : Controller
{
    /// <summary>Clave de TempData para el mensaje de confirmación que muestra el layout.</summary>
    public const string ClaveMensaje = "Mensaje";

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await proyectosApi.ListarAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken ct)
    {
        var resultado = await proyectosApi.ObtenerAsync(id, ct);
        return resultado.Exitoso ? View(resultado.Datos) : ProyectoNoEncontrado();
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Crear(CancellationToken ct)
    {
        var modelo = new ProyectoFormViewModel { FechaInicio = DateOnly.FromDateTime(DateTime.Today) };
        await CargarResponsablesAsync(modelo, null, ct);
        return View(modelo);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ProyectoFormViewModel modelo, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            var resultado = await proyectosApi.CrearAsync(modelo.ARequest(), ct);
            if (resultado.Exitoso)
            {
                TempData[ClaveMensaje] = $"Se creó el proyecto «{resultado.Datos!.Nombre}».";
                return RedirectToAction(nameof(Index));
            }

            // 400: el formulario muestra los errores de la API sin perder lo ingresado (CA-04.2).
            ModelState.AgregarErroresApi<ProyectoFormViewModel>(resultado.Errores);
        }

        await CargarResponsablesAsync(modelo, null, ct);
        return View(modelo);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrador)]
    public async Task<IActionResult> Editar(int id, CancellationToken ct)
    {
        var resultado = await proyectosApi.ObtenerAsync(id, ct);
        if (!resultado.Exitoso)
        {
            return ProyectoNoEncontrado();
        }

        var modelo = ProyectoFormViewModel.Desde(resultado.Datos!);
        await CargarResponsablesAsync(modelo, resultado.Datos, ct);
        return View(modelo);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, ProyectoFormViewModel modelo, CancellationToken ct)
    {
        modelo.Id = id;
        if (ModelState.IsValid)
        {
            var resultado = await proyectosApi.ActualizarAsync(id, modelo.ARequest(), ct);
            if (resultado.Exitoso)
            {
                TempData[ClaveMensaje] = "Se guardaron los cambios del proyecto.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            if (resultado.NoEncontrado)
            {
                return ProyectoNoEncontrado();
            }

            ModelState.AgregarErroresApi<ProyectoFormViewModel>(resultado.Errores);
        }

        // El responsable actual puede estar inactivo y faltar en el catálogo: se vuelve a leer el proyecto.
        var actual = await proyectosApi.ObtenerAsync(id, ct);
        await CargarResponsablesAsync(modelo, actual.Datos, ct);
        return View(modelo);
    }

    /// <summary>Cierra el proyecto (CA-04.3): la API cambia su estado a «Cerrado» y conserva sus tareas.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar(int id, CancellationToken ct)
    {
        var resultado = await proyectosApi.CerrarAsync(id, ct);
        if (resultado.NoEncontrado)
        {
            return ProyectoNoEncontrado();
        }

        TempData[ClaveMensaje] = "El proyecto quedó cerrado.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    /// <summary>
    /// Opciones del campo Responsable: los usuarios activos. Al editar, si el responsable actual
    /// ya no está activo se agrega igual, para que el formulario no lo cambie sin que se note.
    /// </summary>
    private async Task CargarResponsablesAsync(ProyectoFormViewModel modelo, ProyectoDto? actual, CancellationToken ct)
    {
        var catalogos = await catalogosApi.ObtenerAsync(ct);
        var opciones = catalogos.Usuarios
            .Select(u => new SelectListItem(u.Nombre, u.Id.ToString()))
            .ToList();

        if (actual is not null && catalogos.Usuarios.All(u => u.Id != actual.IdResponsable))
        {
            opciones.Insert(0, new SelectListItem($"{actual.Responsable} (inactivo)", actual.IdResponsable.ToString()));
        }

        modelo.Responsables = opciones;
    }

    private ViewResult ProyectoNoEncontrado()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NoEncontrado");
    }
}
