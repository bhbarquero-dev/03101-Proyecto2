namespace _03101_Proyecto2.Api.Entities;

/// <summary>Proyecto (tabla dbo.Proyectos). Se cierra cambiando su estado; no se borra.</summary>
public class Proyecto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFinEstimada { get; set; }
    public int IdEstado { get; set; }
    public int IdResponsable { get; set; }

    /// <summary>Hora UTC; la asigna SQL Server al insertar.</summary>
    public DateTime FechaCreacion { get; set; }

    public Estado Estado { get; set; } = null!;
    public Usuario Responsable { get; set; } = null!;
    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
}
