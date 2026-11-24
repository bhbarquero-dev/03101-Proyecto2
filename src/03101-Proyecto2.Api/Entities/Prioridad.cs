namespace _03101_Proyecto2.Api.Entities;

/// <summary>Prioridad de una tarea (tabla dbo.Prioridades). Catálogo con identificadores fijos.</summary>
public class Prioridad
{
    /// <summary>Identificadores cargados por database/Datos/03_Prioridades.sql.</summary>
    public static class Ids
    {
        public const int Alta = 1;
        public const int Media = 2;
        public const int Baja = 3;
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
