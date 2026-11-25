using System.Net.Http.Headers;
using _03101_Proyecto2.Web.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace _03101_Proyecto2.Web.Services;

/// <summary>
/// Agrega a cada llamada a la API el JWT guardado en la cookie de sesión, como
/// encabezado Authorization: Bearer. Así los clientes HTTP no manejan el token.
/// </summary>
public sealed class TokenBearerHandler(IHttpContextAccessor accesor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (accesor.HttpContext is { } contexto)
        {
            var token = await contexto.GetTokenAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, CuentaController.NombreToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
