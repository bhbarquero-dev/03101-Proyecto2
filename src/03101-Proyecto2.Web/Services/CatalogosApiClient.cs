using _03101_Proyecto2.Contracts.Catalogos;

namespace _03101_Proyecto2.Web.Services;

/// <summary>Cliente HTTP del endpoint de catálogos de la API (/api/catalogos).</summary>
public sealed class CatalogosApiClient(HttpClient http) : ApiClientBase(http)
{
    public Task<CatalogosResponse> ObtenerAsync(CancellationToken ct) =>
        ObtenerAsync<CatalogosResponse>("api/catalogos", ct);
}
