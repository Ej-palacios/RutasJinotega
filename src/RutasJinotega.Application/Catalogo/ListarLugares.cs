using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Comun;

namespace RutasJinotega.Application.Catalogo;

public sealed record ListarLugaresConsulta(FiltroLugares Filtro);

public sealed class ListarLugaresHandler(ILectorLugares lector)
    : IConsultaHandler<ListarLugaresConsulta, IReadOnlyList<LugarResumenDto>>
{
    public async Task<Resultado<IReadOnlyList<LugarResumenDto>>> EjecutarAsync(
        ListarLugaresConsulta consulta,
        CancellationToken ct = default)
    {
        var texto = consulta.Filtro.Texto;

        if (texto is { Length: > FiltroLugares.LongitudMaximaTexto })
        {
            return Resultado<IReadOnlyList<LugarResumenDto>>.Falla(
                Error.Validacion($"El texto de búsqueda no puede superar {FiltroLugares.LongitudMaximaTexto} caracteres."));
        }

        var lugares = await lector.ListarAsync(consulta.Filtro, ct);

        IReadOnlyList<LugarResumenDto> resumenes = lugares.Select(LugarMapeador.AResumen).ToList();
        return Resultado<IReadOnlyList<LugarResumenDto>>.Exito(resumenes);
    }
}
