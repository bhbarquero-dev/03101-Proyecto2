namespace _03101_Proyecto2.Contracts.Auth;

/// <summary>Respuesta de un inicio de sesión exitoso: el JWT, su vencimiento y el usuario.</summary>
public sealed record LoginResponse(string Token, DateTimeOffset ExpiraEn, UsuarioSesion Usuario);
