using _03101_Proyecto2.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _03101_Proyecto2.Web.Filters;

/// <summary>
/// Atiende en un solo lugar las respuestas de la API que ninguna pantalla resuelve por sí misma:
/// 401 cierra la sesión y lleva al login, 403 muestra Acceso denegado y una falla de red
/// o un error del servidor muestra una página de servicio no disponible. A una solicitud hecha
/// con fetch se le responde solo con el código HTTP (401, 403 o 503).
/// </summary>
public sealed class ErroresApiFilter(ILogger<ErroresApiFilter> logger) : IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        var solicitud = context.HttpContext.Request;
        var esAjax = SolicitudAjax.Es(solicitud);

        switch (context.Exception)
        {
            case SesionVencidaException:
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // Solo una página GET se puede volver a abrir después del login.
                var volverA = HttpMethods.IsGet(solicitud.Method) ? $"{solicitud.PathBase}{solicitud.Path}{solicitud.QueryString}" : null;
                context.Result = esAjax
                    ? new StatusCodeResult(StatusCodes.Status401Unauthorized)
                    : new RedirectToActionResult("Login", "Cuenta", new { returnUrl = volverA });
                break;

            case AccesoDenegadoException:
                context.Result = esAjax
                    ? new StatusCodeResult(StatusCodes.Status403Forbidden)
                    : new RedirectToActionResult("AccesoDenegado", "Cuenta", null);
                break;

            case HttpRequestException ex:
                logger.LogError(ex, "Falló una llamada a la API.");
                context.Result = esAjax
                    ? new StatusCodeResult(StatusCodes.Status503ServiceUnavailable)
                    : new ViewResult
                    {
                        ViewName = "ServicioNoDisponible",
                        StatusCode = StatusCodes.Status503ServiceUnavailable
                    };
                break;

            default:
                return;
        }

        context.ExceptionHandled = true;
    }
}
