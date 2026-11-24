using RutasJinotega.Application.Comun;

namespace RutasJinotega.Api.Endpoints;

/// <summary>Traduce un Resultado de Application a una respuesta HTTP. Es el único lugar que conoce esa regla.</summary>
public static class ResultadoHttp
{
    public static IResult AResultadoHttp<T>(this Resultado<T> resultado)
    {
        if (resultado.EsExito)
        {
            return Results.Ok(resultado.Valor);
        }

        var error = resultado.Error;
        var estado = error.Codigo switch
        {
            Error.CodigoValidacion => StatusCodes.Status400BadRequest,
            Error.CodigoNoEncontrado => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(detail: error.Mensaje, title: error.Codigo, statusCode: estado);
    }
}
