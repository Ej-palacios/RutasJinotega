using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Domain.Rutas;

/// <summary>
/// Ruta turística curada: una secuencia ordenada de lugares dentro de un municipio.
/// Las rutas de esta fase son de lectura; su definición vive en Infrastructure.
/// </summary>
public sealed class Ruta
{
    private readonly List<RutaParada> _paradas = [];

    private Ruta() { }

    public string Clave { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public bool EsCurada { get; private set; }
    public Municipio Municipio { get; private set; } = null!;

    public IReadOnlyList<RutaParada> Paradas => _paradas;

    public static Ruta Crear(
        Municipio municipio,
        string clave,
        string nombre,
        string descripcion,
        bool esCurada = true)
    {
        ArgumentNullException.ThrowIfNull(municipio);
        ArgumentException.ThrowIfNullOrWhiteSpace(clave);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        return new Ruta
        {
            Municipio = municipio,
            Clave = clave.Trim(),
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim() ?? string.Empty,
            EsCurada = esCurada
        };
    }

    public RutaParada AgregarParada(
        Lugar lugar,
        int orden,
        int? tiempoEstimadoMin = null,
        string? nota = null)
    {
        ArgumentNullException.ThrowIfNull(lugar);

        if (orden <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(orden), "El orden de la parada debe ser mayor que cero.");
        }

        if (_paradas.Any(p => p.Orden == orden))
        {
            throw new ArgumentException("Ya existe una parada con ese orden.", nameof(orden));
        }

        var parada = RutaParada.Crear(this, lugar, orden, tiempoEstimadoMin, nota);
        _paradas.Add(parada);
        return parada;
    }
}
