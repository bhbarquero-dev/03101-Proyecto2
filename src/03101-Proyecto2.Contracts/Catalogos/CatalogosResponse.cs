namespace _03101_Proyecto2.Contracts.Catalogos;

/// <summary>
/// Valores para las listas desplegables de los formularios (GET /api/catalogos).
/// Usuarios trae solo los activos: son los que se pueden asignar como responsables o a tareas.
/// </summary>
public sealed record CatalogosResponse(
    IReadOnlyList<ItemCatalogo> EstadosProyecto,
    IReadOnlyList<ItemCatalogo> EstadosTarea,
    IReadOnlyList<ItemCatalogo> Prioridades,
    IReadOnlyList<ItemCatalogo> Usuarios);
