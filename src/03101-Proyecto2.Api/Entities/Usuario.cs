namespace _03101_Proyecto2.Api.Entities;

/// <summary>Usuario del sistema (tabla dbo.Usuarios).</summary>
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>Hash PBKDF2 en el formato de PasswordHasher de ASP.NET Core. Nunca la contraseña.</summary>
    public string ContrasenaHash { get; set; } = string.Empty;

    public int IdRol { get; set; }

    /// <summary>Hora UTC; la asigna SQL Server al insertar.</summary>
    public DateTime FechaRegistro { get; set; }

    /// <summary>Baja lógica: un usuario inactivo no inicia sesión pero conserva su historial.</summary>
    public bool Activo { get; set; } = true;

    public Rol Rol { get; set; } = null!;
    public ICollection<Proyecto> ProyectosResponsable { get; set; } = new List<Proyecto>();
    public ICollection<Tarea> TareasAsignadas { get; set; } = new List<Tarea>();
}
