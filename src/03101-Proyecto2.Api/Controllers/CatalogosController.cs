using _03101_Proyecto2.Api.Services;
using _03101_Proyecto2.Contracts.Catalogos;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Api.Controllers;

/// <summary>
/// Catálogos para los formularios. Lo usa cualquier usuario autenticado, porque el Usuario
/// también asigna tareas y no tiene acceso a /api/usuarios, que es solo del Administrador.
/// </summary>
[ApiController]
[Route("api/catalogos")]
public sealed class CatalogosController(CatalogosService catalogos) : ControllerBase
{
    /// <summary>Estados de proyecto y de tarea, prioridades y usuarios activos.</summary>
    /// <response code="200">Catálogos.</response>
    [HttpGet]
    [ProducesResponseType<CatalogosResponse>(StatusCodes.Status200OK)]
    public Task<CatalogosResponse> Obtener(CancellationToken ct) => catalogos.ObtenerAsync(ct);
}
