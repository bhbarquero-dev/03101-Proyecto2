namespace _03101_Proyecto2.Contracts.Proyectos;

/// <summary>Proyecto tal como lo devuelve la API, con el nombre de su estado y de su responsable.</summary>
public sealed record ProyectoDto(
    int Id,
    string Nombre,
    string? Descripcion,
    DateOnly FechaInicio,
    DateOnly FechaFinEstimada,
    int IdEstado,
    string Estado,
    int IdResponsable,
    string Responsable,
    DateTime FechaCreacion,
    int TotalTareas,
    int TareasCompletadas);
