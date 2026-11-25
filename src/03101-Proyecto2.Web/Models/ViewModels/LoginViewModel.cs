using System.ComponentModel.DataAnnotations;

namespace _03101_Proyecto2.Web.Models.ViewModels;

/// <summary>Formulario de la página de inicio de sesión.</summary>
public sealed class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [Display(Name = "Correo")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Contrasena { get; set; } = string.Empty;

    /// <summary>Página protegida que se intentó abrir antes de iniciar sesión.</summary>
    public string? ReturnUrl { get; set; }
}
