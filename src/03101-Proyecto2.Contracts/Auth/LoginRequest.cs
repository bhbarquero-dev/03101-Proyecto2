using System.ComponentModel.DataAnnotations;

namespace _03101_Proyecto2.Contracts.Auth;

/// <summary>Credenciales enviadas a POST /api/auth/login.</summary>
public sealed class LoginRequest
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(128)]
    public string Contrasena { get; set; } = string.Empty;
}
