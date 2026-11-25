namespace _03101_Proyecto2.Api.Entities;

/// <summary>Rol del sistema (tabla dbo.Roles). Catálogo con identificadores fijos.</summary>
public class Rol
{
    /// <summary>Identificadores cargados por database/Datos/01_Roles.sql.</summary>
    public static class Ids
    {
        public const int Administrador = 1;
        public const int Usuario = 2;
    }

    /// <summary>Nombres de rol, tal como viajan en el claim "role" del JWT (definidos en Contracts).</summary>
    public static class Nombres
    {
        public const string Administrador = Contracts.Auth.Roles.Administrador;
        public const string Usuario = Contracts.Auth.Roles.Usuario;
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
