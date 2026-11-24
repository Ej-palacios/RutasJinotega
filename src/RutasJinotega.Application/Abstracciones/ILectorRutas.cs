using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Application.Abstracciones;

/// <summary>Puerto de lectura de rutas curadas. Lo implementa Infrastructure.</summary>
public interface ILectorRutas
{
    Task<Ruta?> ObtenerPorClaveAsync(string clave, CancellationToken ct = default);
}
