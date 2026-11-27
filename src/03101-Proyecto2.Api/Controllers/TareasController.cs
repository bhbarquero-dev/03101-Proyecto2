using _03101_Proyecto2.Api.Security;
using _03101_Proyecto2.Api.Services;
using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Contracts.Tareas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Api.Controllers;

/// <summary>
/// Gestión de tareas (HU-05). Cualquier usuario autenticado consulta y crea tareas en los proyectos en
/// los que participa; el Usuario actualiza solo el estado y la prioridad de las tareas que tiene asignadas.
/// </summary>
[ApiController]
[Route("api/tareas")]
public sealed class TareasController(TareasService tareas) : ControllerBase
{
    private int IdUsuario => User.ObtenerIdUsuario();
    private bool EsAdministrador => User.IsInRole(Roles.Administrador);

    /// <summary>Tareas de un proyecto.</summary>
    /// <response code="200">Tareas del proyecto.</response>
    /// <response code="404">El proyecto no existe, o el Usuario no participa en él.</response>
    [HttpGet("/api/proyectos/{idProyecto:int}/tareas")]
    [ProducesResponseType<IReadOnlyList<TareaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TareaDto>>> ListarPorProyecto(int idProyecto, CancellationToken ct)
    {
        var lista = await tareas.ListarPorProyectoAsync(idProyecto, IdUsuario, EsAdministrador, ct);
        return lista is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: "El proyecto no existe.")
            : Ok(lista);
    }

    /// <summary>Detalle de una tarea.</summary>
    /// <response code="200">La tarea.</response>
    /// <response code="404">No existe, o pertenece a un proyecto en el que el Usuario no participa.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<TareaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TareaDto>> Obtener(int id, CancellationToken ct)
    {
        var tarea = await tareas.ObtenerAsync(id, IdUsuario, EsAdministrador, ct);
        return tarea is null ? NoEncontrada() : tarea;
    }

    /// <summary>Crea una tarea (CA-05.1).</summary>
    /// <response code="201">Tarea creada; el encabezado Location apunta a su detalle.</response>
    /// <response code="400">Datos incompletos, estado que no es de tarea, prioridad o asignado inválidos, o proyecto cerrado.</response>
    [HttpPost]
    [ProducesResponseType<TareaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear(TareaRequest datos, CancellationToken ct)
    {
        var resultado = await tareas.CrearAsync(datos, IdUsuario, EsAdministrador, ct);
        return resultado.Estado == EstadoResultado.Exito
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : Responder(resultado);
    }

    /// <summary>Actualiza una tarea (CA-05.2). Si el estado no es válido, la tarea no cambia (CA-05.3).</summary>
    /// <response code="200">Tarea actualizada.</response>
    /// <response code="400">Datos incompletos, estado que no es de tarea, prioridad o asignado inválidos, o proyecto cerrado.</response>
    /// <response code="403">El Usuario no tiene asignada la tarea o intenta cambiar algo más que el estado y la prioridad.</response>
    /// <response code="404">La tarea no existe, o el Usuario no participa en su proyecto.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType<TareaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, TareaRequest datos, CancellationToken ct) =>
        Responder(await tareas.ActualizarAsync(id, datos, IdUsuario, EsAdministrador, ct));

    /// <summary>Cancela una tarea: cambia su estado a «Cancelada» y la conserva.</summary>
    /// <response code="204">Tarea cancelada (también si ya lo estaba).</response>
    /// <response code="400">El proyecto de la tarea está cerrado.</response>
    /// <response code="403">El usuario no es Administrador.</response>
    /// <response code="404">La tarea no existe.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int id, CancellationToken ct)
    {
        var resultado = await tareas.CancelarAsync(id, ct);
        return resultado.Estado == EstadoResultado.Exito ? NoContent() : Responder(resultado);
    }

    private IActionResult Responder(ResultadoServicio<TareaDto> resultado)
    {
        switch (resultado.Estado)
        {
            case EstadoResultado.Exito:
                return Ok(resultado.Valor);
            case EstadoResultado.NoEncontrado:
                return NoEncontrada();
            case EstadoResultado.Prohibido:
                return Problem(statusCode: StatusCodes.Status403Forbidden, title: resultado.Mensaje);
            default:
                foreach (var (campo, mensajes) in resultado.Errores!)
                {
                    foreach (var mensaje in mensajes)
                    {
                        ModelState.AddModelError(campo, mensaje);
                    }
                }

                return ValidationProblem(ModelState);
        }
    }

    private ObjectResult NoEncontrada() =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "La tarea no existe.");
}
