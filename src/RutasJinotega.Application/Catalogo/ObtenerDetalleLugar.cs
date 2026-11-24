using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Application.Catalogo;

public sealed record ObtenerDetalleLugarConsulta(int Id);

public sealed class ObtenerDetalleLugarHandler(ILectorLugares lector)
    : IConsultaHandler<ObtenerDetalleLugarConsulta, LugarDetalleDto>
{
    public async Task<Resultado<LugarDetalleDto>> EjecutarAsync(
        ObtenerDetalleLugarConsulta consulta,
        CancellationToken ct = default)
    {
        if (consulta.Id <= 0)
        {
            return Resultado<LugarDetalleDto>.Falla(Error.Validacion("El identificador del lugar no es válido."));
        }

        var lugar = await lector.ObtenerPorIdAsync(consulta.Id, ct);

        return lugar is null
            ? Resultado<LugarDetalleDto>.Falla(Error.NoEncontrado("No encontramos ese lugar."))
            : Resultado<LugarDetalleDto>.Exito(LugarMapeador.ADetalle(lugar));
    }
}
