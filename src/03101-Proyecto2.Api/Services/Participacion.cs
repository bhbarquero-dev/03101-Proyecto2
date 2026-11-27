using System.Linq.Expressions;
using _03101_Proyecto2.Api.Entities;

namespace _03101_Proyecto2.Api.Services;

/// <summary>
/// Regla de participación (sección 4.3 del documento): un usuario participa en un proyecto si es
/// su responsable o si tiene al menos una tarea asignada en él. Se expresa como árbol de expresión
/// para que EF Core la traduzca a SQL, y la comparten los servicios de proyectos y de tareas.
/// </summary>
public static class Participacion
{
    public static Expression<Func<Proyecto, bool>> Participa(int idUsuario) =>
        p => p.IdResponsable == idUsuario || p.Tareas.Any(t => t.IdAsignado == idUsuario);

    /// <summary>Proyectos que el usuario puede ver: todos para el Administrador, los suyos para el Usuario.</summary>
    public static IQueryable<Proyecto> Visibles(this IQueryable<Proyecto> proyectos, int idUsuario, bool esAdministrador) =>
        esAdministrador ? proyectos : proyectos.Where(Participa(idUsuario));
}
