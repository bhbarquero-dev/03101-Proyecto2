using System.Security.Claims;
using System.Text;
using _03101_Proyecto2.Api.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace _03101_Proyecto2.Api.Security;

/// <summary>Emite los JWT firmados con HMAC-SHA256 según la sección "Jwt" de la configuración.</summary>
public sealed class TokenService(IOptions<JwtOptions> options, TimeProvider reloj)
{
    private readonly JwtOptions _opciones = options.Value;
    private readonly JsonWebTokenHandler _handler = new();

    /// <summary>
    /// Crea el token del usuario. Lleva el identificador (sub), el nombre, el correo y el rol;
    /// los nombres de claim coinciden con NameClaimType y RoleClaimType configurados en Program.cs.
    /// </summary>
    public (string Token, DateTimeOffset ExpiraEn) Emitir(Usuario usuario, string nombreRol)
    {
        var ahora = reloj.GetUtcNow();
        var expiraEn = ahora.AddMinutes(_opciones.ExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _opciones.Issuer,
            Audience = _opciones.Audience,
            IssuedAt = ahora.UtcDateTime,
            NotBefore = ahora.UtcDateTime,
            Expires = expiraEn.UtcDateTime,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimNames.Nombre, usuario.Nombre),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimNames.Rol, nombreRol)
            ]),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return (_handler.CreateToken(descriptor), expiraEn);
    }
}
