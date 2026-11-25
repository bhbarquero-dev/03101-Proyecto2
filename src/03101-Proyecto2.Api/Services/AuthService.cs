using _03101_Proyecto2.Api.Data;
using _03101_Proyecto2.Api.Entities;
using _03101_Proyecto2.Api.Security;
using _03101_Proyecto2.Contracts.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _03101_Proyecto2.Api.Services;

/// <summary>Valida credenciales y emite el JWT (HU-01).</summary>
public sealed class AuthService(
    GestionProyectosContext db,
    IPasswordHasher<Usuario> hasher,
    TokenService tokens)
{
    // Hash de una contraseña cualquiera. Si el correo no existe, se verifica contra este hash
    // para que la respuesta tarde lo mismo y no revele qué correos están registrados.
    private static readonly Lazy<string> HashFicticio =
        new(() => new PasswordHasher<Usuario>().HashPassword(new Usuario(), Guid.NewGuid().ToString()));

    /// <summary>
    /// Devuelve el token si el correo existe, el usuario está activo y la contraseña coincide;
    /// en cualquier otro caso devuelve null, sin distinguir el motivo (CA-01.2 y CA-01.3).
    /// </summary>
    public async Task<LoginResponse?> IniciarSesionAsync(LoginRequest solicitud, CancellationToken ct)
    {
        var email = solicitud.Email.Trim();
        var usuario = await db.Usuarios
            .Include(u => u.Rol)
            .SingleOrDefaultAsync(u => u.Email == email, ct);

        if (usuario is null)
        {
            hasher.VerifyHashedPassword(new Usuario(), HashFicticio.Value, solicitud.Contrasena);
            return null;
        }

        var resultado = hasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, solicitud.Contrasena);
        if (resultado == PasswordVerificationResult.Failed || !usuario.Activo)
        {
            return null;
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            // El hash usa parámetros antiguos: se actualiza aprovechando que se conoce la contraseña.
            usuario.ContrasenaHash = hasher.HashPassword(usuario, solicitud.Contrasena);
            await db.SaveChangesAsync(ct);
        }

        var (token, expiraEn) = tokens.Emitir(usuario, usuario.Rol.Nombre);
        return new LoginResponse(
            token,
            expiraEn,
            new UsuarioSesion(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol.Nombre));
    }
}
