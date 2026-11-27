namespace _03101_Proyecto2.Api.Entities;

/// <summary>Prioridad de una tarea (tabla dbo.Prioridades). Catálogo con identificadores fijos.</summary>
public class Prioridad
{
    /// <summary>Identificadores cargados por database/Datos/03_Prioridades.sql (definidos en Contracts).</summary>
    public static class Ids
    {
        public const int Alta = Contracts.Catalogos.PrioridadesIds.Alta;
        public const int Media = Contracts.Catalogos.PrioridadesIds.Media;
        public const int Baja = Contracts.Catalogos.PrioridadesIds.Baja;
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
