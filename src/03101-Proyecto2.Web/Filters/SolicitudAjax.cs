namespace _03101_Proyecto2.Web.Filters;

/// <summary>
/// Identifica las solicitudes hechas con fetch desde la página (wwwroot/js/tareas.js), que envían
/// el encabezado X-Requested-With. A esas no se les responde con una redirección, que fetch seguiría
/// en silencio, sino con el código HTTP para que el script decida qué hacer.
/// </summary>
public static class SolicitudAjax
{
    public const string Encabezado = "X-Requested-With";

    public static bool Es(HttpRequest solicitud) => solicitud.Headers.ContainsKey(Encabezado);
}
