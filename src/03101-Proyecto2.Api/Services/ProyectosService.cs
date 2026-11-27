using System.Linq.Expressions;
using _03101_Proyecto2.Api.Data;
using _03101_Proyecto2.Api.Entities;
using _03101_Proyecto2.Contracts.Proyectos;
using Microsoft.EntityFrameworkCore;

namespace _03101_Proyecto2.Api.Services;

/// <summary>Reglas de negocio de los proyectos (HU-04) y su visibilidad por rol (CA-07.2).</summary>
public sealed class ProyectosService(GestionProyectosContext db)
{
    /// <summary>
    /// Proyección a DTO que EF Core traduce a una sola consulta SQL: trae los nombres del estado
    /// y del responsable y cuenta las tareas sin cargar las entidades completas.
    /// </summary>
    private static readonly Expression<Func<Proyecto, ProyectoDto>> ADto = p => new ProyectoDto(
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.FechaInicio,
        p.FechaFinEstimada,
        p.IdEstado,
        p.Estado.Nombre,
        p.IdResponsable,
        p.Responsable.Nombre,
        p.FechaCreacion,
        p.Tareas.Count(),
        p.Tareas.Count(t => t.IdEstado == Estado.Ids.Completada));

    /// <summary>Lista los proyectos visibles para el usuario: los activos primero, luego por fecha de inicio.</summary>
    public async Task<IReadOnlyList<ProyectoDto>> ListarAsync(int idUsuario, bool esAdministrador, CancellationToken ct) =>
        await Visibles(idUsuario, esAdministrador)
            .OrderBy(p => p.IdEstado)
            .ThenByDescending(p => p.FechaInicio)
            .ThenBy(p => p.Nombre)
            .Select(ADto)
            .ToListAsync(ct);

    /// <summary>Devuelve el proyecto, o null si no existe o el usuario no participa en él.</summary>
    public Task<ProyectoDto?> ObtenerAsync(int id, int idUsuario, bool esAdministrador, CancellationToken ct) =>
        Visibles(idUsuario, esAdministrador)
            .Where(p => p.Id == id)
            .Select(ADto)
            .SingleOrDefaultAsync(ct);

    /// <summary>Crea el proyecto en estado «Activo» (CA-04.1).</summary>
    public async Task<ResultadoServicio<ProyectoDto>> CrearAsync(ProyectoRequest datos, CancellationToken ct)
    {
        if (!await ResponsableValidoAsync(datos.IdResponsable!.Value, ct))
        {
            return ResponsableInvalido();
        }

        var proyecto = new Proyecto { IdEstado = Estado.Ids.Activo };
        Copiar(datos, proyecto);
        db.Proyectos.Add(proyecto);
        await db.SaveChangesAsync(ct);

        return ResultadoServicio<ProyectoDto>.Exito(await LeerAsync(proyecto.Id, ct));
    }

    /// <summary>Actualiza los datos del proyecto; su estado no cambia aquí.</summary>
    public async Task<ResultadoServicio<ProyectoDto>> ActualizarAsync(int id, ProyectoRequest datos, CancellationToken ct)
    {
        var proyecto = await db.Proyectos.SingleOrDefaultAsync(p => p.Id == id, ct);
        if (proyecto is null)
        {
            return ResultadoServicio<ProyectoDto>.NoEncontrado();
        }

        // Si el responsable no cambia se acepta aunque se haya desactivado después de asignarlo;
        // así se puede corregir el resto del proyecto sin obligar a reasignarlo.
        if (datos.IdResponsable != proyecto.IdResponsable
            && !await ResponsableValidoAsync(datos.IdResponsable!.Value, ct))
        {
            return ResponsableInvalido();
        }

        Copiar(datos, proyecto);
        await db.SaveChangesAsync(ct);

        return ResultadoServicio<ProyectoDto>.Exito(await LeerAsync(id, ct));
    }

    /// <summary>
    /// Cierra el proyecto (CA-04.3): cambia su estado a «Cerrado» y conserva sus tareas.
    /// Cerrar un proyecto ya cerrado no es un error, de modo que la operación es idempotente.
    /// </summary>
    public async Task<bool> CerrarAsync(int id, CancellationToken ct)
    {
        var proyecto = await db.Proyectos.SingleOrDefaultAsync(p => p.Id == id, ct);
        if (proyecto is null)
        {
            return false;
        }

        if (proyecto.IdEstado != Estado.Ids.Cerrado)
        {
            proyecto.IdEstado = Estado.Ids.Cerrado;
            await db.SaveChangesAsync(ct);
        }

        return true;
    }

    /// <summary>El Administrador ve todos los proyectos; el Usuario, solo aquellos en los que participa.</summary>
    private IQueryable<Proyecto> Visibles(int idUsuario, bool esAdministrador) =>
        db.Proyectos.AsNoTracking().Visibles(idUsuario, esAdministrador);

    private Task<ProyectoDto> LeerAsync(int id, CancellationToken ct) =>
        db.Proyectos.AsNoTracking().Where(p => p.Id == id).Select(ADto).SingleAsync(ct);

    private Task<bool> ResponsableValidoAsync(int idUsuario, CancellationToken ct) =>
        db.Usuarios.AnyAsync(u => u.Id == idUsuario && u.Activo, ct);

    private static ResultadoServicio<ProyectoDto> ResponsableInvalido() =>
        ResultadoServicio<ProyectoDto>.Invalido(
            nameof(ProyectoRequest.IdResponsable),
            "El responsable no existe o está inactivo.");

    private static void Copiar(ProyectoRequest datos, Proyecto proyecto)
    {
        proyecto.Nombre = datos.Nombre.Trim();
        proyecto.Descripcion = string.IsNullOrWhiteSpace(datos.Descripcion) ? null : datos.Descripcion.Trim();
        proyecto.FechaInicio = datos.FechaInicio!.Value;
        proyecto.FechaFinEstimada = datos.FechaFinEstimada!.Value;
        proyecto.IdResponsable = datos.IdResponsable!.Value;
    }
}
