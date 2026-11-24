using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Api.Endpoints;

public static class EndpointsCatalogo
{
    public static IEndpointRouteBuilder MapCatalogo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalogo", ObtenerAsync);
        return app;
    }

    private static async Task<IResult> ObtenerAsync(
        IConsultaHandler<ObtenerCatalogoConsulta, CatalogoDto> handler,
        CancellationToken ct) =>
        (await handler.EjecutarAsync(new ObtenerCatalogoConsulta(), ct)).AResultadoHttp();
}
