using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Api.Endpoints;

public static class EndpointsLugares
{
    private const double RadioPredeterminadoKm = 25;
    private const int MaximoPredeterminado = 20;

    public static IEndpointRouteBuilder MapLugares(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/lugares");

        grupo.MapGet("/", ListarAsync);
        grupo.MapGet("/cercanos", CercanosAsync);
        grupo.MapGet("/{id:int}", DetalleAsync);

        return app;
    }

    private static async Task<IResult> ListarAsync(
        int? municipioId,
        int? categoriaId,
        bool? soloPocoConocidos,
        string? texto,
        IConsultaHandler<ListarLugaresConsulta, IReadOnlyList<LugarResumenDto>> handler,
        CancellationToken ct)
    {
        var filtro = new FiltroLugares(municipioId, categoriaId, soloPocoConocidos ?? false, texto);
        var resultado = await handler.EjecutarAsync(new ListarLugaresConsulta(filtro), ct);
        return resultado.AResultadoHttp();
    }

    private static async Task<IResult> CercanosAsync(
        double latitud,
        double longitud,
        double? radioKm,
        int? maximo,
        int? categoriaId,
        IConsultaHandler<ObtenerLugaresCercanosConsulta, IReadOnlyList<LugarCercanoDto>> handler,
        CancellationToken ct)
    {
        var consulta = new ObtenerLugaresCercanosConsulta(
            latitud,
            longitud,
            radioKm ?? RadioPredeterminadoKm,
            maximo ?? MaximoPredeterminado,
            categoriaId);

        return (await handler.EjecutarAsync(consulta, ct)).AResultadoHttp();
    }

    private static async Task<IResult> DetalleAsync(
        int id,
        IConsultaHandler<ObtenerDetalleLugarConsulta, LugarDetalleDto> handler,
        CancellationToken ct) =>
        (await handler.EjecutarAsync(new ObtenerDetalleLugarConsulta(id), ct)).AResultadoHttp();
}
