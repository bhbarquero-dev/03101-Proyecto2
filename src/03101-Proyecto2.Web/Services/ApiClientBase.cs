using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Web.Services;

/// <summary>La API respondió 401: el token venció o no es válido y hay que iniciar sesión de nuevo.</summary>
public sealed class SesionVencidaException() : Exception("La sesión con la API venció.");

/// <summary>La API respondió 403: el rol del usuario no permite la operación.</summary>
public sealed class AccesoDenegadoException() : Exception("La API rechazó la operación por el rol del usuario.");

/// <summary>
/// Respuesta esperada de la API: éxito con datos, 400 con errores por campo o 404.
/// Los casos que la pantalla no puede resolver (401, 403, errores del servidor o de red)
/// se lanzan como excepción y los atiende <see cref="Filters.ErroresApiFilter"/>.
/// </summary>
public sealed record ResultadoApi<T>(
    HttpStatusCode Estado,
    T? Datos = default,
    IReadOnlyDictionary<string, string[]>? Errores = null)
{
    public bool Exitoso => (int)Estado is >= 200 and < 300;
    public bool NoEncontrado => Estado == HttpStatusCode.NotFound;
}

/// <summary>Envío de solicitudes a la API e interpretación uniforme de sus respuestas.</summary>
public abstract class ApiClientBase(HttpClient http)
{
    protected async Task<ResultadoApi<T>> EnviarAsync<T>(HttpMethod metodo, string ruta, object? cuerpo, CancellationToken ct)
    {
        using var solicitud = new HttpRequestMessage(metodo, ruta);
        if (cuerpo is not null)
        {
            solicitud.Content = JsonContent.Create(cuerpo);
        }

        using var respuesta = await http.SendAsync(solicitud, ct);

        switch (respuesta.StatusCode)
        {
            case HttpStatusCode.Unauthorized:
                throw new SesionVencidaException();
            case HttpStatusCode.Forbidden:
                throw new AccesoDenegadoException();
            case HttpStatusCode.NotFound:
                return new ResultadoApi<T>(respuesta.StatusCode);
            case HttpStatusCode.BadRequest:
                var problema = await respuesta.Content.ReadFromJsonAsync<ValidationProblemDetails>(ct);
                return new ResultadoApi<T>(
                    respuesta.StatusCode,
                    Errores: problema?.Errors.AsReadOnly() ?? new Dictionary<string, string[]>().AsReadOnly());
        }

        // Cualquier otro error (5xx) lanza HttpRequestException, igual que una falla de red.
        respuesta.EnsureSuccessStatusCode();

        var datos = respuesta.StatusCode == HttpStatusCode.NoContent
            ? default
            : await respuesta.Content.ReadFromJsonAsync<T>(ct);
        return new ResultadoApi<T>(respuesta.StatusCode, datos);
    }

    protected async Task<T> ObtenerAsync<T>(string ruta, CancellationToken ct)
    {
        var resultado = await EnviarAsync<T>(HttpMethod.Get, ruta, null, ct);
        return resultado.Datos ?? throw new HttpRequestException($"La API no devolvió datos para {ruta}.");
    }
}
