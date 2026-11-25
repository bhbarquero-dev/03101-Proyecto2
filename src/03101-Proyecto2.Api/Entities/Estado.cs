namespace _03101_Proyecto2.Api.Entities;

/// <summary>
/// Estado de un proyecto o de una tarea (tabla dbo.Estados). Catálogo con identificadores fijos.
/// <see cref="EntidadAplicacion"/> indica a qué entidad aplica; la API valida que una tarea
/// solo use estados de tarea y un proyecto solo estados de proyecto.
/// </summary>
public class Estado
{
    /// <summary>Identificadores cargados por database/Datos/02_Estados.sql (definidos en Contracts).</summary>
    public static class Ids
    {
        public const int Activo = Contracts.Catalogos.EstadosIds.Activo;
        public const int Cerrado = Contracts.Catalogos.EstadosIds.Cerrado;
        public const int Pendiente = Contracts.Catalogos.EstadosIds.Pendiente;
        public const int EnProgreso = Contracts.Catalogos.EstadosIds.EnProgreso;
        public const int Completada = Contracts.Catalogos.EstadosIds.Completada;
        public const int Cancelada = Contracts.Catalogos.EstadosIds.Cancelada;
    }

    /// <summary>Valores posibles de <see cref="EntidadAplicacion"/> (restricción CK_Estados_EntidadAplicacion).</summary>
    public static class Entidades
    {
        public const string Proyecto = "Proyecto";
        public const string Tarea = "Tarea";
    }

    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string EntidadAplicacion { get; set; } = string.Empty;

    public ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
