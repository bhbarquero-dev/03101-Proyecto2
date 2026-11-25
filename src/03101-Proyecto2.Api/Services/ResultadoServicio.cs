namespace _03101_Proyecto2.Api.Services;

/// <summary>Cómo terminó una operación de un servicio; el controlador lo traduce a un código HTTP.</summary>
public enum EstadoResultado
{
    Exito,
    NoEncontrado,
    Invalido
}

/// <summary>
/// Resultado de una operación de escritura: el valor si tuvo éxito, o los errores de validación
/// por campo si una regla de negocio la rechazó. Evita usar excepciones para casos esperados.
/// </summary>
public sealed record ResultadoServicio<T>(
    EstadoResultado Estado,
    T? Valor = default,
    IReadOnlyDictionary<string, string[]>? Errores = null)
{
    public static ResultadoServicio<T> Exito(T valor) => new(EstadoResultado.Exito, valor);

    public static ResultadoServicio<T> NoEncontrado() => new(EstadoResultado.NoEncontrado);

    public static ResultadoServicio<T> Invalido(string campo, string mensaje) =>
        new(EstadoResultado.Invalido, Errores: new Dictionary<string, string[]> { [campo] = [mensaje] });
}
