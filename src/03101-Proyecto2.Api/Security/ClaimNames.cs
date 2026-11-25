using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace _03101_Proyecto2.Api.Security;

/// <summary>
/// Nombres de los claims del JWT. Program.cs desactiva el mapeo de claims entrantes
/// (MapInboundClaims = false), así que los nombres llegan tal cual se emiten.
/// </summary>
public static class ClaimNames
{
    public const string Id = JwtRegisteredClaimNames.Sub;
    public const string Nombre = "name";
    public const string Email = JwtRegisteredClaimNames.Email;
    public const string Rol = "role";

    /// <summary>Identificador del usuario autenticado, leído del claim "sub".</summary>
    public static int ObtenerIdUsuario(this ClaimsPrincipal usuario) =>
        int.TryParse(usuario.FindFirstValue(Id), out var id)
            ? id
            : throw new InvalidOperationException("El token no trae un identificador de usuario válido.");
}
