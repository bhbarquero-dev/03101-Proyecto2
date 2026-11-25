using _03101_Proyecto2.Contracts.Proyectos;

namespace _03101_Proyecto2.Web.Services;

/// <summary>Cliente HTTP de los endpoints de proyectos de la API (/api/proyectos).</summary>
public sealed class ProyectosApiClient(HttpClient http) : ApiClientBase(http)
{
    private const string Ruta = "api/proyectos";

    public Task<IReadOnlyList<ProyectoDto>> ListarAsync(CancellationToken ct) =>
        ObtenerAsync<IReadOnlyList<ProyectoDto>>(Ruta, ct);

    public Task<ResultadoApi<ProyectoDto>> ObtenerAsync(int id, CancellationToken ct) =>
        EnviarAsync<ProyectoDto>(HttpMethod.Get, $"{Ruta}/{id}", null, ct);

    public Task<ResultadoApi<ProyectoDto>> CrearAsync(ProyectoRequest datos, CancellationToken ct) =>
        EnviarAsync<ProyectoDto>(HttpMethod.Post, Ruta, datos, ct);

    public Task<ResultadoApi<ProyectoDto>> ActualizarAsync(int id, ProyectoRequest datos, CancellationToken ct) =>
        EnviarAsync<ProyectoDto>(HttpMethod.Put, $"{Ruta}/{id}", datos, ct);

    public Task<ResultadoApi<object>> CerrarAsync(int id, CancellationToken ct) =>
        EnviarAsync<object>(HttpMethod.Delete, $"{Ruta}/{id}", null, ct);
}
