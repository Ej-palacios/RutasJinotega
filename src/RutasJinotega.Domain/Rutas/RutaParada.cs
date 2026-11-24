using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Domain.Rutas;

/// <summary>Parada ordenada de una ruta. Referencia un lugar del catálogo.</summary>
public sealed class RutaParada
{
    private RutaParada() { }

    public Ruta Ruta { get; private set; } = null!;
    public Lugar Lugar { get; private set; } = null!;
    public int Orden { get; private set; }
    public int? TiempoEstimadoMin { get; private set; }
    public string? Nota { get; private set; }

    internal static RutaParada Crear(
        Ruta ruta,
        Lugar lugar,
        int orden,
        int? tiempoEstimadoMin,
        string? nota)
    {
        ArgumentNullException.ThrowIfNull(ruta);
        ArgumentNullException.ThrowIfNull(lugar);

        if (tiempoEstimadoMin is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(tiempoEstimadoMin),
                "El tiempo estimado no puede ser negativo.");
        }

        return new RutaParada
        {
            Ruta = ruta,
            Lugar = lugar,
            Orden = orden,
            TiempoEstimadoMin = tiempoEstimadoMin,
            Nota = nota?.Trim()
        };
    }
}
