using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Application.Abstracciones;

/// <summary>Puerto de lectura de lugares. Lo implementa Infrastructure.</summary>
public interface ILectorLugares
{
    Task<Lugar?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<Lugar>> ListarAsync(FiltroLugares filtro, CancellationToken ct = default);

    Task<IReadOnlyList<Lugar>> ListarEnCuadranteAsync(
        CuadranteGeografico cuadrante,
        int? categoriaId,
        CancellationToken ct = default);
}
