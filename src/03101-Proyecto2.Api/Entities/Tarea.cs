namespace _03101_Proyecto2.Api.Entities;

/// <summary>Tarea de un proyecto (tabla dbo.Tareas).</summary>
public class Tarea
{
    public int Id { get; set; }
    public int IdProyecto { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdAsignado { get; set; }
    public int IdEstado { get; set; }
    public int IdPrioridad { get; set; }

    /// <summary>Hora UTC; la asigna SQL Server al insertar.</summary>
    public DateTime FechaCreacion { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public Proyecto Proyecto { get; set; } = null!;
    public Usuario Asignado { get; set; } = null!;
    public Estado Estado { get; set; } = null!;
    public Prioridad Prioridad { get; set; } = null!;
}
