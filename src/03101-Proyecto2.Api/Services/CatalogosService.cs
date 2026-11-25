using _03101_Proyecto2.Api.Data;
using _03101_Proyecto2.Api.Entities;
using _03101_Proyecto2.Contracts.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace _03101_Proyecto2.Api.Services;

/// <summary>Valores de las listas desplegables: estados, prioridades y usuarios activos.</summary>
public sealed class CatalogosService(GestionProyectosContext db)
{
    public async Task<CatalogosResponse> ObtenerAsync(CancellationToken ct)
    {
        var estados = await db.Estados.AsNoTracking()
            .OrderBy(e => e.Id)
            .Select(e => new { e.Id, e.Nombre, e.EntidadAplicacion })
            .ToListAsync(ct);

        var prioridades = await db.Prioridades.AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new ItemCatalogo(p.Id, p.Nombre))
            .ToListAsync(ct);

        var usuarios = await db.Usuarios.AsNoTracking()
            .Where(u => u.Activo)
            .OrderBy(u => u.Nombre)
            .Select(u => new ItemCatalogo(u.Id, u.Nombre))
            .ToListAsync(ct);

        List<ItemCatalogo> EstadosDe(string entidad) => estados
            .Where(e => e.EntidadAplicacion == entidad)
            .Select(e => new ItemCatalogo(e.Id, e.Nombre))
            .ToList();

        return new CatalogosResponse(
            EstadosDe(Estado.Entidades.Proyecto),
            EstadosDe(Estado.Entidades.Tarea),
            prioridades,
            usuarios);
    }
}
