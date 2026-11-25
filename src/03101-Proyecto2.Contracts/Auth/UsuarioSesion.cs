namespace _03101_Proyecto2.Contracts.Auth;

/// <summary>Datos del usuario autenticado que la interfaz necesita mostrar (nombre y rol).</summary>
public sealed record UsuarioSesion(int Id, string Nombre, string Email, string Rol);
