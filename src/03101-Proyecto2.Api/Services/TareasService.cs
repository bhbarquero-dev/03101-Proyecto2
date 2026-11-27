using System.Linq.Expressions;
using _03101_Proyecto2.Api.Data;
using _03101_Proyecto2.Api.Entities;
using _03101_Proyecto2.Contracts.Tareas;
using Microsoft.EntityFrameworkCore;

namespace _03101_Proyecto2.Api.Services;

/// <summary>
/// Reglas de negocio de las tareas (HU-05). El Administrador gestiona las tareas de cualquier proyecto.
/// El Usuario crea tareas en los proyectos en los que participa y, de las tareas que tiene asignadas,
/// solo cambia el estado y la prioridad.
/// </summary>
public sealed class TareasService(GestionProyectosContext db)
{
    private const string MensajeProyectoCerrado = "El proyecto está cerrado y no admite cambios en sus tareas.";

    private static readonly Expression<Func<Tarea, TareaDto>> ADto = t => new TareaDto(
        t.Id,
        t.IdProyecto,
        t.Proyecto.Nombre,
        t.Titulo,
        t.Descripcion,
        t.IdAsignado,
        t.Asignado.Nombre,
        t.IdEstado,
        t.Estado.Nombre,
        t.IdPrioridad,
        t.Prioridad.Nombre,
        t.FechaCreacion,
        t.FechaVencimiento);

    /// <summary>
    /// Tareas del proyecto, o null si el proyecto no existe o el usuario no participa en él.
    /// Orden: por estado (pendientes primero), luego por prioridad y por fecha de vencimiento.
    /// </summary>
    public async Task<IReadOnlyList<TareaDto>?> ListarPorProyectoAsync(
        int idProyecto, int idUsuario, bool esAdministrador, CancellationToken ct)
    {
        var visible = await db.Proyectos.Visibles(idUsuario, esAdministrador).AnyAsync(p => p.Id == idProyecto, ct);
        if (!visible)
        {
            return null;
        }

        return await db.Tareas.AsNoTracking()
            .Where(t => t.IdProyecto == idProyecto)
            .OrderBy(t => t.IdEstado)
            .ThenBy(t => t.IdPrioridad)
            .ThenBy(t => t.FechaVencimiento == null)
            .ThenBy(t => t.FechaVencimiento)
            .ThenBy(t => t.Id)
            .Select(ADto)
            .ToListAsync(ct);
    }

    /// <summary>La tarea, o null si no existe o pertenece a un proyecto en el que el usuario no participa.</summary>
    public Task<TareaDto?> ObtenerAsync(int id, int idUsuario, bool esAdministrador, CancellationToken ct) =>
        Visibles(idUsuario, esAdministrador)
            .Where(t => t.Id == id)
            .Select(ADto)
            .SingleOrDefaultAsync(ct);

    /// <summary>Crea la tarea en un proyecto activo en el que el usuario participa (CA-05.1).</summary>
    public async Task<ResultadoServicio<TareaDto>> CrearAsync(
        TareaRequest datos, int idUsuario, bool esAdministrador, CancellationToken ct)
    {
        var proyecto = await db.Proyectos.AsNoTracking()
            .Visibles(idUsuario, esAdministrador)
            .Where(p => p.Id == datos.IdProyecto)
            .Select(p => new { p.IdEstado })
            .SingleOrDefaultAsync(ct);

        // Un proyecto en el que el Usuario no participa se trata como inexistente, sin revelar que existe.
        if (proyecto is null)
        {
            return ResultadoServicio<TareaDto>.Invalido(nameof(TareaRequest.IdProyecto), "El proyecto no existe.");
        }

        if (proyecto.IdEstado != Estado.Ids.Activo)
        {
            return ResultadoServicio<TareaDto>.Invalido(string.Empty, MensajeProyectoCerrado);
        }

        var errores = await ValidarCatalogosAsync(datos, validarAsignado: true, ct);
        if (errores.Count > 0)
        {
            return ResultadoServicio<TareaDto>.Invalido(errores);
        }

        var tarea = new Tarea { IdProyecto = datos.IdProyecto!.Value };
        Copiar(datos, tarea);
        db.Tareas.Add(tarea);
        await db.SaveChangesAsync(ct);

        return ResultadoServicio<TareaDto>.Exito(await LeerAsync(tarea.Id, ct));
    }

    /// <summary>
    /// Actualiza la tarea (CA-05.2). Si el estado o la prioridad no son válidos no guarda nada, de modo
    /// que la tarea conserva sus valores anteriores (CA-05.3).
    /// </summary>
    public async Task<ResultadoServicio<TareaDto>> ActualizarAsync(
        int id, TareaRequest datos, int idUsuario, bool esAdministrador, CancellationToken ct)
    {
        var proyectosVisibles = db.Proyectos.Visibles(idUsuario, esAdministrador);
        var tarea = await db.Tareas
            .Include(t => t.Proyecto)
            .Where(t => proyectosVisibles.Any(p => p.Id == t.IdProyecto))
            .SingleOrDefaultAsync(t => t.Id == id, ct);

        if (tarea is null)
        {
            return ResultadoServicio<TareaDto>.NoEncontrado();
        }

        if (datos.IdProyecto != tarea.IdProyecto)
        {
            return ResultadoServicio<TareaDto>.Invalido(
                nameof(TareaRequest.IdProyecto), "Una tarea no se puede mover a otro proyecto.");
        }

        if (tarea.Proyecto.IdEstado != Estado.Ids.Activo)
        {
            return ResultadoServicio<TareaDto>.Invalido(string.Empty, MensajeProyectoCerrado);
        }

        if (!esAdministrador)
        {
            if (tarea.IdAsignado != idUsuario)
            {
                return ResultadoServicio<TareaDto>.Prohibido("Solo puede actualizar las tareas que tiene asignadas.");
            }

            if (CambiaMasQueEstadoYPrioridad(tarea, datos))
            {
                return ResultadoServicio<TareaDto>.Prohibido("Solo puede cambiar el estado y la prioridad de sus tareas.");
            }
        }

        // Un asignado que ya no está activo se acepta si no cambia, como el responsable de un proyecto.
        var errores = await ValidarCatalogosAsync(datos, validarAsignado: datos.IdAsignado != tarea.IdAsignado, ct);
        if (errores.Count > 0)
        {
            return ResultadoServicio<TareaDto>.Invalido(errores);
        }

        Copiar(datos, tarea);
        await db.SaveChangesAsync(ct);

        return ResultadoServicio<TareaDto>.Exito(await LeerAsync(id, ct));
    }

    /// <summary>
    /// Cancela la tarea: cambia su estado a «Cancelada» en lugar de borrarla, para conservar el
    /// historial, igual que el cierre de proyectos. Cancelar una tarea ya cancelada no es un error.
    /// </summary>
    public async Task<ResultadoServicio<TareaDto>> CancelarAsync(int id, CancellationToken ct)
    {
        var tarea = await db.Tareas.Include(t => t.Proyecto).SingleOrDefaultAsync(t => t.Id == id, ct);
        if (tarea is null)
        {
            return ResultadoServicio<TareaDto>.NoEncontrado();
        }

        if (tarea.Proyecto.IdEstado != Estado.Ids.Activo)
        {
            return ResultadoServicio<TareaDto>.Invalido(string.Empty, MensajeProyectoCerrado);
        }

        if (tarea.IdEstado != Estado.Ids.Cancelada)
        {
            tarea.IdEstado = Estado.Ids.Cancelada;
            await db.SaveChangesAsync(ct);
        }

        return ResultadoServicio<TareaDto>.Exito(await LeerAsync(id, ct));
    }

    /// <summary>Tareas de los proyectos visibles para el usuario.</summary>
    private IQueryable<Tarea> Visibles(int idUsuario, bool esAdministrador)
    {
        // La consulta de proyectos se captura en una variable para que EF Core la incorpore como subconsulta.
        var proyectosVisibles = db.Proyectos.Visibles(idUsuario, esAdministrador);
        return db.Tareas.AsNoTracking().Where(t => proyectosVisibles.Any(p => p.Id == t.IdProyecto));
    }

    /// <summary>
    /// Valida contra los catálogos lo que una clave foránea no alcanza a impedir: que el estado sea de
    /// tarea y no de proyecto (sección 4.3), además de que la prioridad exista y el asignado esté activo.
    /// </summary>
    private async Task<Dictionary<string, string[]>> ValidarCatalogosAsync(
        TareaRequest datos, bool validarAsignado, CancellationToken ct)
    {
        var errores = new Dictionary<string, string[]>();

        var estadoValido = await db.Estados.AnyAsync(
            e => e.Id == datos.IdEstado && e.EntidadAplicacion == Estado.Entidades.Tarea, ct);
        if (!estadoValido)
        {
            errores[nameof(TareaRequest.IdEstado)] = ["El estado no existe o no corresponde a una tarea."];
        }

        if (!await db.Prioridades.AnyAsync(p => p.Id == datos.IdPrioridad, ct))
        {
            errores[nameof(TareaRequest.IdPrioridad)] = ["La prioridad no existe."];
        }

        if (validarAsignado && !await db.Usuarios.AnyAsync(u => u.Id == datos.IdAsignado && u.Activo, ct))
        {
            errores[nameof(TareaRequest.IdAsignado)] = ["El usuario asignado no existe o está inactivo."];
        }

        return errores;
    }

    private static bool CambiaMasQueEstadoYPrioridad(Tarea tarea, TareaRequest datos) =>
        datos.Titulo.Trim() != tarea.Titulo
        || Normalizar(datos.Descripcion) != tarea.Descripcion
        || datos.IdAsignado != tarea.IdAsignado
        || datos.FechaVencimiento != tarea.FechaVencimiento;

    private Task<TareaDto> LeerAsync(int id, CancellationToken ct) =>
        db.Tareas.AsNoTracking().Where(t => t.Id == id).Select(ADto).SingleAsync(ct);

    private static string? Normalizar(string? texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();

    private static void Copiar(TareaRequest datos, Tarea tarea)
    {
        tarea.Titulo = datos.Titulo.Trim();
        tarea.Descripcion = Normalizar(datos.Descripcion);
        tarea.IdAsignado = datos.IdAsignado!.Value;
        tarea.IdEstado = datos.IdEstado!.Value;
        tarea.IdPrioridad = datos.IdPrioridad!.Value;
        tarea.FechaVencimiento = datos.FechaVencimiento;
    }
}
