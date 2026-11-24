using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;

namespace RutasJinotega.Shared.UI.Servicios;

/// <summary>
/// Cliente tipado de la API. Los componentes dependen de esta interfaz, nunca de HttpClient.
/// </summary>
public interface ILugaresClient
{
    Task<Resultado<CatalogoDto>> ObtenerCatalogoAsync(CancellationToken ct = default);

    Task<Resultado<IReadOnlyList<LugarResumenDto>>> ListarAsync(FiltroLugares filtro, CancellationToken ct = default);

    Task<Resultado<IReadOnlyList<LugarCercanoDto>>> ObtenerCercanosAsync(
        ObtenerLugaresCercanosConsulta consulta,
        CancellationToken ct = default);

    Task<Resultado<LugarDetalleDto>> ObtenerDetalleAsync(int id, CancellationToken ct = default);

    Task<Resultado<RutaDetalleDto>> ObtenerRutaAsync(string clave, CancellationToken ct = default);
}
