namespace _03101_Proyecto2.Contracts.Catalogos;

/// <summary>
/// Identificadores fijos de la tabla Estados (database/Datos/02_Estados.sql).
/// Se comparten para que la API y la aplicación web se refieran a cada estado por el mismo valor.
/// </summary>
public static class EstadosIds
{
    // Estados de proyecto
    public const int Activo = 1;
    public const int Cerrado = 2;

    // Estados de tarea
    public const int Pendiente = 3;
    public const int EnProgreso = 4;
    public const int Completada = 5;
    public const int Cancelada = 6;
}
