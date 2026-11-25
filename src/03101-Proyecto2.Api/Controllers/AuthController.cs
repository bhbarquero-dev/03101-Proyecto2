using System.Security.Claims;
using _03101_Proyecto2.Api.Security;
using _03101_Proyecto2.Api.Services;
using _03101_Proyecto2.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Api.Controllers;

/// <summary>Autenticación: inicio de sesión y validación del token.</summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService auth) : ControllerBase
{
    /// <summary>Valida las credenciales y emite el JWT.</summary>
    /// <response code="200">Credenciales válidas: devuelve el token y los datos del usuario.</response>
    /// <response code="400">Faltan datos o el correo no tiene un formato válido.</response>
    /// <response code="401">Correo no registrado, contraseña incorrecta o usuario inactivo.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest solicitud, CancellationToken ct)
    {
        var respuesta = await auth.IniciarSesionAsync(solicitud, ct);
        if (respuesta is null)
        {
            return Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Correo o contraseña incorrectos.");
        }

        return Ok(respuesta);
    }

    /// <summary>Valida el token recibido y devuelve el usuario que identifica.</summary>
    /// <response code="200">Token válido.</response>
    /// <response code="401">Sin token, o token vencido o inválido.</response>
    [HttpGet("yo")]
    [ProducesResponseType<UsuarioSesion>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UsuarioSesion> Yo() => new UsuarioSesion(
        User.ObtenerIdUsuario(),
        User.FindFirstValue(ClaimNames.Nombre) ?? string.Empty,
        User.FindFirstValue(ClaimNames.Email) ?? string.Empty,
        User.FindFirstValue(ClaimNames.Rol) ?? string.Empty);
}
