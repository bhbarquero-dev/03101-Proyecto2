using _03101_Proyecto2.Contracts.Tareas;

namespace _03101_Proyecto2.Web.Services;

/// <summary>Cliente HTTP de los endpoints de tareas de la API (/api/tareas y /api/proyectos/{id}/tareas).</summary>
public sealed class TareasApiClient(HttpClient http) : ApiClientBase(http)
{
    private const string Ruta = "api/tareas";

    public Task<ResultadoApi<IReadOnlyList<TareaDto>>> ListarPorProyectoAsync(int idProyecto, CancellationToken ct) =>
        EnviarAsync<IReadOnlyList<TareaDto>>(HttpMethod.Get, $"api/proyectos/{idProyecto}/tareas", null, ct);

    public Task<ResultadoApi<TareaDto>> ObtenerAsync(int id, CancellationToken ct) =>
        EnviarAsync<TareaDto>(HttpMethod.Get, $"{Ruta}/{id}", null, ct);

    public Task<ResultadoApi<TareaDto>> CrearAsync(TareaRequest datos, CancellationToken ct) =>
        EnviarAsync<TareaDto>(HttpMethod.Post, Ruta, datos, ct);

    public Task<ResultadoApi<TareaDto>> ActualizarAsync(int id, TareaRequest datos, CancellationToken ct) =>
        EnviarAsync<TareaDto>(HttpMethod.Put, $"{Ruta}/{id}", datos, ct);

    public Task<ResultadoApi<object>> CancelarAsync(int id, CancellationToken ct) =>
        EnviarAsync<object>(HttpMethod.Delete, $"{Ruta}/{id}", null, ct);
}
