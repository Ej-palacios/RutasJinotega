using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;

namespace RutasJinotega.Shared.UI.Servicios;

public sealed class HttpLugaresClient(HttpClient http) : ILugaresClient
{
    public Task<Resultado<CatalogoDto>> ObtenerCatalogoAsync(CancellationToken ct = default) =>
        LeerAsync<CatalogoDto>("api/catalogo", ct);

    public Task<Resultado<IReadOnlyList<LugarResumenDto>>> ListarAsync(
        FiltroLugares filtro,
        CancellationToken ct = default)
    {
        var query = ConstructorQuery.Crear(
            ("municipioId", filtro.MunicipioId),
            ("categoriaId", filtro.CategoriaId),
            ("soloPocoConocidos", filtro.SoloPocoConocidos ? (object)true : null),
            ("texto", string.IsNullOrWhiteSpace(filtro.Texto) ? null : filtro.Texto.Trim()));

        return LeerAsync<IReadOnlyList<LugarResumenDto>>("api/lugares" + query, ct);
    }

    public Task<Resultado<IReadOnlyList<LugarCercanoDto>>> ObtenerCercanosAsync(
        ObtenerLugaresCercanosConsulta consulta,
        CancellationToken ct = default)
    {
        var query = ConstructorQuery.Crear(
            ("latitud", consulta.Latitud),
            ("longitud", consulta.Longitud),
            ("radioKm", consulta.RadioKm),
            ("maximo", consulta.Maximo),
            ("categoriaId", consulta.CategoriaId));

        return LeerAsync<IReadOnlyList<LugarCercanoDto>>("api/lugares/cercanos" + query, ct);
    }

    public Task<Resultado<LugarDetalleDto>> ObtenerDetalleAsync(int id, CancellationToken ct = default) =>
        LeerAsync<LugarDetalleDto>($"api/lugares/{id}", ct);

    public Task<Resultado<RutaDetalleDto>> ObtenerRutaAsync(string clave, CancellationToken ct = default) =>
        LeerAsync<RutaDetalleDto>($"api/rutas/{Uri.EscapeDataString(clave)}", ct);

    private async Task<Resultado<T>> LeerAsync<T>(string ruta, CancellationToken ct)
    {
        try
        {
            var valor = await http.GetFromJsonAsync<T>(ruta, ct);

            return valor is null
                ? Resultado<T>.Falla(Error.Red("El servidor devolvió una respuesta vacía."))
                : Resultado<T>.Exito(valor);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return Resultado<T>.Falla(Error.NoEncontrado("No encontramos lo que buscabas."));
        }
        catch (HttpRequestException)
        {
            return Resultado<T>.Falla(Error.Red("No pudimos conectar con el servidor. Revisa tu conexión."));
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            return Resultado<T>.Falla(Error.Red("La respuesta del servidor no es válida."));
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return Resultado<T>.Falla(Error.Red("El servidor tardó demasiado en responder."));
        }
    }
}
