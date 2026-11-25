using _03101_Proyecto2.Api.Security;
using _03101_Proyecto2.Api.Services;
using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Contracts.Proyectos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Api.Controllers;

/// <summary>Mantenimiento de proyectos (HU-04). Leer: cualquier usuario autenticado; escribir: Administrador.</summary>
[ApiController]
[Route("api/proyectos")]
public sealed class ProyectosController(ProyectosService proyectos) : ControllerBase
{
    private bool EsAdministrador => User.IsInRole(Roles.Administrador);

    /// <summary>Lista los proyectos. El Usuario recibe solo aquellos en los que participa (CA-07.2).</summary>
    /// <response code="200">Lista de proyectos visibles para el usuario.</response>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProyectoDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<ProyectoDto>> Listar(CancellationToken ct) =>
        proyectos.ListarAsync(User.ObtenerIdUsuario(), EsAdministrador, ct);

    /// <summary>Detalle de un proyecto.</summary>
    /// <response code="200">El proyecto.</response>
    /// <response code="404">No existe, o el Usuario no participa en él (no se revela si existe).</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ProyectoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProyectoDto>> Obtener(int id, CancellationToken ct)
    {
        var proyecto = await proyectos.ObtenerAsync(id, User.ObtenerIdUsuario(), EsAdministrador, ct);
        return proyecto is null ? NoEncontrado() : proyecto;
    }

    /// <summary>Crea un proyecto en estado «Activo» (CA-04.1).</summary>
    /// <response code="201">Proyecto creado; el encabezado Location apunta a su detalle.</response>
    /// <response code="400">Datos incompletos, fechas incoherentes (CA-04.2) o responsable inválido.</response>
    /// <response code="403">El usuario no es Administrador (CA-07.3).</response>
    [HttpPost]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType<ProyectoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Crear(ProyectoRequest datos, CancellationToken ct)
    {
        var resultado = await proyectos.CrearAsync(datos, ct);
        return resultado.Estado == EstadoResultado.Exito
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Valor!.Id }, resultado.Valor)
            : Responder(resultado);
    }

    /// <summary>Actualiza los datos de un proyecto.</summary>
    /// <response code="200">Proyecto actualizado.</response>
    /// <response code="400">Datos incompletos, fechas incoherentes o responsable inválido.</response>
    /// <response code="403">El usuario no es Administrador.</response>
    /// <response code="404">El proyecto no existe.</response>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType<ProyectoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, ProyectoRequest datos, CancellationToken ct) =>
        Responder(await proyectos.ActualizarAsync(id, datos, ct));

    /// <summary>Cierra un proyecto (CA-04.3). No lo borra: cambia su estado a «Cerrado» y conserva sus tareas.</summary>
    /// <response code="204">Proyecto cerrado (también si ya lo estaba).</response>
    /// <response code="403">El usuario no es Administrador.</response>
    /// <response code="404">El proyecto no existe.</response>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Administrador)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cerrar(int id, CancellationToken ct) =>
        await proyectos.CerrarAsync(id, ct) ? NoContent() : NoEncontrado();

    private IActionResult Responder(ResultadoServicio<ProyectoDto> resultado)
    {
        switch (resultado.Estado)
        {
            case EstadoResultado.Exito:
                return Ok(resultado.Valor);
            case EstadoResultado.NoEncontrado:
                return NoEncontrado();
            default:
                foreach (var (campo, mensajes) in resultado.Errores!)
                {
                    foreach (var mensaje in mensajes)
                    {
                        ModelState.AddModelError(campo, mensaje);
                    }
                }

                // Mismo formato que la validación automática de [ApiController].
                return ValidationProblem(ModelState);
        }
    }

    private ObjectResult NoEncontrado() =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "El proyecto no existe.");
}
