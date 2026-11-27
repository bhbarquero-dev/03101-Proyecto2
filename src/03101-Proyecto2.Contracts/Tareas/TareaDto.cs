namespace _03101_Proyecto2.Contracts.Tareas;

/// <summary>Tarea tal como la devuelve la API, con los nombres de su proyecto, asignado, estado y prioridad.</summary>
public sealed record TareaDto(
    int Id,
    int IdProyecto,
    string Proyecto,
    string Titulo,
    string? Descripcion,
    int IdAsignado,
    string Asignado,
    int IdEstado,
    string Estado,
    int IdPrioridad,
    string Prioridad,
    DateTime FechaCreacion,
    DateOnly? FechaVencimiento);
