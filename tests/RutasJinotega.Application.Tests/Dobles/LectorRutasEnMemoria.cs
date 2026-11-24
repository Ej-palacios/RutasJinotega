using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Application.Tests.Dobles;

/// <summary>Doble de prueba: cumple el puerto ILectorRutas sin base de datos.</summary>
internal sealed class LectorRutasEnMemoria(IEnumerable<Ruta> rutas) : ILectorRutas
{
    private readonly List<Ruta> _rutas = rutas.ToList();

    public Task<Ruta?> ObtenerPorClaveAsync(string clave, CancellationToken ct = default) =>
        Task.FromResult(_rutas.FirstOrDefault(r =>
            string.Equals(r.Clave, clave, StringComparison.OrdinalIgnoreCase)));
}
