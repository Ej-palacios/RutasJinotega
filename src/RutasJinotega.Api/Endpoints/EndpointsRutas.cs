using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;

namespace RutasJinotega.Api.Endpoints;

public static class EndpointsRutas
{
    public static IEndpointRouteBuilder MapRutas(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/rutas");

        grupo.MapGet("/{clave}", DetalleAsync);

        return app;
    }

    private static async Task<IResult> DetalleAsync(
        string clave,
        IConsultaHandler<ObtenerRutaConsulta, RutaDetalleDto> handler,
        CancellationToken ct) =>
        (await handler.EjecutarAsync(new ObtenerRutaConsulta(clave), ct)).AResultadoHttp();
}
