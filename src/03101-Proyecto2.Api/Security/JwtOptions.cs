using System.ComponentModel.DataAnnotations;

namespace _03101_Proyecto2.Api.Security;

/// <summary>
/// Parámetros de emisión y validación de tokens JWT (sección "Jwt" de appsettings).
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    /// <summary>Clave simétrica HMAC-SHA256; mínimo 32 caracteres (256 bits).</summary>
    [Required, MinLength(32)]
    public string Key { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int ExpirationMinutes { get; init; } = 60;
}
