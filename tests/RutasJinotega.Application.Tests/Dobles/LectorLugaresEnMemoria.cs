using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Application.Tests.Dobles;

/// <summary>Doble de prueba: cumple el puerto ILectorLugares sin base de datos.</summary>
internal sealed class LectorLugaresEnMemoria(IEnumerable<Lugar> lugares) : ILectorLugares
{
    private readonly List<Lugar> _lugares = lugares.ToList();

    public CuadranteGeografico? UltimoCuadrante { get; private set; }

    public Task<Lugar?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_lugares.FirstOrDefault(l => l.Id == id));

    public Task<IReadOnlyList<Lugar>> ListarAsync(FiltroLugares filtro, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Lugar>>(_lugares);

    public Task<IReadOnlyList<Lugar>> ListarEnCuadranteAsync(
        CuadranteGeografico cuadrante,
        int? categoriaId,
        CancellationToken ct = default)
    {
        UltimoCuadrante = cuadrante;

        IReadOnlyList<Lugar> dentro = _lugares.Where(l => cuadrante.Contiene(l.Ubicacion)).ToList();
        return Task.FromResult(dentro);
    }
}
