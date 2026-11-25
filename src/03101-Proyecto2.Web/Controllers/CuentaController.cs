using System.Security.Claims;
using _03101_Proyecto2.Contracts.Auth;
using _03101_Proyecto2.Web.Models.ViewModels;
using _03101_Proyecto2.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _03101_Proyecto2.Web.Controllers;

/// <summary>Inicio y cierre de sesión (HU-01 y HU-02).</summary>
public sealed class CuentaController(AuthApiClient authApi) : Controller
{
    /// <summary>Nombre con que se guarda el JWT de la API dentro de la cookie.</summary>
    public const string NombreToken = "access_token";

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var (resultado, respuesta) = await authApi.LoginAsync(
            new LoginRequest { Email = modelo.Email, Contrasena = modelo.Contrasena }, ct);

        if (resultado != ResultadoLogin.Exitoso || respuesta is null)
        {
            ModelState.AddModelError(string.Empty, resultado == ResultadoLogin.CredencialesInvalidas
                ? "Correo o contraseña incorrectos."
                : "No se pudo contactar el servicio. Intente de nuevo en unos minutos.");
            modelo.Contrasena = string.Empty;
            return View(modelo);
        }

        var identidad = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, respuesta.Usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, respuesta.Usuario.Nombre),
                new Claim(ClaimTypes.Email, respuesta.Usuario.Email),
                new Claim(ClaimTypes.Role, respuesta.Usuario.Rol)
            ],
            CookieAuthenticationDefaults.AuthenticationScheme);

        // La cookie vence junto con el JWT y lo guarda cifrado; nunca queda al alcance del JavaScript.
        var propiedades = new AuthenticationProperties
        {
            IsPersistent = false,
            ExpiresUtc = respuesta.ExpiraEn
        };
        propiedades.StoreTokens([new AuthenticationToken { Name = NombreToken, Value = respuesta.Token }]);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad),
            propiedades);

        return Url.IsLocalUrl(modelo.ReturnUrl)
            ? LocalRedirect(modelo.ReturnUrl)
            : RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccesoDenegado() => View();
}
