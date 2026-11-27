namespace _03101_Proyecto2.Api.Services;

/// <summary>Cómo terminó una operación de un servicio; el controlador lo traduce a un código HTTP.</summary>
public enum EstadoResultado
{
    Exito,
    NoEncontrado,
    Invalido,
    Prohibido
}

/// <summary>
/// Resultado de una operación de escritura: el valor si tuvo éxito, o los errores de validación
/// por campo si una regla de negocio la rechazó. Evita usar excepciones para casos esperados.
/// </summary>
public sealed record ResultadoServicio<T>(
    EstadoResultado Estado,
    T? Valor = default,
    IReadOnlyDictionary<string, string[]>? Errores = null,
    string? Mensaje = null)
{
    public static ResultadoServicio<T> Exito(T valor) => new(EstadoResultado.Exito, valor);

    public static ResultadoServicio<T> NoEncontrado() => new(EstadoResultado.NoEncontrado);

    /// <summary>El rol del usuario no permite la operación; <paramref name="mensaje"/> explica por qué.</summary>
    public static ResultadoServicio<T> Prohibido(string mensaje) => new(EstadoResultado.Prohibido, Mensaje: mensaje);

    public static ResultadoServicio<T> Invalido(string campo, string mensaje) =>
        Invalido(new Dictionary<string, string[]> { [campo] = [mensaje] });

    public static ResultadoServicio<T> Invalido(IReadOnlyDictionary<string, string[]> errores) =>
        new(EstadoResultado.Invalido, Errores: errores);
}
