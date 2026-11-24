using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Comun;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Application.Catalogo;

public sealed record ObtenerLugaresCercanosConsulta(
    double Latitud,
    double Longitud,
    double RadioKm = 25,
    int Maximo = 20,
    int? CategoriaId = null);

/// <summary>
/// Devuelve los lugares dentro de un radio, ordenados del más cercano al más lejano.
/// Estrategia: prefiltro por cuadrante en la BD y distancia exacta (Haversine) en memoria.
/// </summary>
public sealed class ObtenerLugaresCercanosHandler(ILectorLugares lector)
    : IConsultaHandler<ObtenerLugaresCercanosConsulta, IReadOnlyList<LugarCercanoDto>>
{
    public const double RadioMaximoKm = 200;
    public const int MaximoPermitido = 100;

    public async Task<Resultado<IReadOnlyList<LugarCercanoDto>>> EjecutarAsync(
        ObtenerLugaresCercanosConsulta consulta,
        CancellationToken ct = default)
    {
        if (Validar(consulta) is { } error)
        {
            return Resultado<IReadOnlyList<LugarCercanoDto>>.Falla(error);
        }

        var origen = new Coordenada(consulta.Latitud, consulta.Longitud);
        var candidatos = await lector.ListarEnCuadranteAsync(
            origen.Alrededor(consulta.RadioKm),
            consulta.CategoriaId,
            ct);

        IReadOnlyList<LugarCercanoDto> cercanos = candidatos
            .Select(lugar => (Lugar: lugar, Distancia: origen.DistanciaKm(lugar.Ubicacion)))
            .Where(x => x.Distancia <= consulta.RadioKm)
            .OrderBy(x => x.Distancia)
            .Take(consulta.Maximo)
            .Select(x => new LugarCercanoDto(LugarMapeador.AResumen(x.Lugar), Math.Round(x.Distancia, 2)))
            .ToList();

        return Resultado<IReadOnlyList<LugarCercanoDto>>.Exito(cercanos);
    }

    private static Error? Validar(ObtenerLugaresCercanosConsulta consulta)
    {
        if (!Coordenada.EsValida(consulta.Latitud, consulta.Longitud))
        {
            return Error.Validacion("La latitud o la longitud están fuera de rango.");
        }

        if (consulta.RadioKm <= 0 || consulta.RadioKm > RadioMaximoKm)
        {
            return Error.Validacion($"El radio debe estar entre 0 y {RadioMaximoKm} km.");
        }

        if (consulta.Maximo < 1 || consulta.Maximo > MaximoPermitido)
        {
            return Error.Validacion($"El máximo de resultados debe estar entre 1 y {MaximoPermitido}.");
        }

        return null;
    }
}
