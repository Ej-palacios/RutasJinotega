using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Application.Rutas;

public sealed record ObtenerRutaConsulta(string Clave);

public sealed class ObtenerRutaHandler(ILectorRutas lector)
    : IConsultaHandler<ObtenerRutaConsulta, RutaDetalleDto>
{
    public async Task<Resultado<RutaDetalleDto>> EjecutarAsync(
        ObtenerRutaConsulta consulta,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(consulta.Clave))
        {
            return Resultado<RutaDetalleDto>.Falla(Error.Validacion("La clave de la ruta no es válida."));
        }

        var ruta = await lector.ObtenerPorClaveAsync(consulta.Clave.Trim(), ct);

        return ruta is null
            ? Resultado<RutaDetalleDto>.Falla(Error.NoEncontrado("No encontramos esa ruta."))
            : Resultado<RutaDetalleDto>.Exito(RutaMapeador.ADetalle(ruta));
    }
}
