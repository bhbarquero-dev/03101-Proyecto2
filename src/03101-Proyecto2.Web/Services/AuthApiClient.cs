using System.Net;
using _03101_Proyecto2.Contracts.Auth;

namespace _03101_Proyecto2.Web.Services;

/// <summary>Resultado de un intento de inicio de sesión contra la API.</summary>
public enum ResultadoLogin
{
    Exitoso,
    CredencialesInvalidas,
    ApiNoDisponible
}

/// <summary>Cliente HTTP de los endpoints de autenticación de la API (/api/auth).</summary>
public sealed class AuthApiClient(HttpClient http, ILogger<AuthApiClient> logger)
{
    public async Task<(ResultadoLogin Resultado, LoginResponse? Respuesta)> LoginAsync(
        LoginRequest solicitud, CancellationToken ct)
    {
        try
        {
            using var respuesta = await http.PostAsJsonAsync("api/auth/login", solicitud, ct);

            if (respuesta.IsSuccessStatusCode)
            {
                var datos = await respuesta.Content.ReadFromJsonAsync<LoginResponse>(ct);
                return datos is null
                    ? (ResultadoLogin.ApiNoDisponible, null)
                    : (ResultadoLogin.Exitoso, datos);
            }

            // 401: credenciales incorrectas o usuario inactivo. 400: datos que la API rechazó.
            // En ambos casos se muestra el mismo mensaje, sin indicar qué falló (CA-01.2).
            if (respuesta.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.BadRequest)
            {
                return (ResultadoLogin.CredencialesInvalidas, null);
            }

            logger.LogWarning("La API respondió {Estado} al iniciar sesión.", (int)respuesta.StatusCode);
            return (ResultadoLogin.ApiNoDisponible, null);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "No se pudo contactar la API para iniciar sesión.");
            return (ResultadoLogin.ApiNoDisponible, null);
        }
    }
}
