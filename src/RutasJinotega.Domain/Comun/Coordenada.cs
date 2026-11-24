namespace RutasJinotega.Domain.Comun;

/// <summary>
/// Punto geográfico (WGS84). Value object inmutable con las reglas de distancia.
/// </summary>
public sealed record Coordenada
{
    private const double RadioTierraKm = 6371.0088;

    // Valor conservador (menor al real) para que el cuadrante nunca quede corto.
    private const double KmPorGradoConservador = 111.0;

    public double Latitud { get; }
    public double Longitud { get; }

    public Coordenada(double latitud, double longitud)
    {
        if (!EsValida(latitud, longitud))
        {
            throw new ArgumentOutOfRangeException(nameof(latitud), "La coordenada está fuera de rango.");
        }

        Latitud = latitud;
        Longitud = longitud;
    }

    public static bool EsValida(double latitud, double longitud) =>
        double.IsFinite(latitud)
        && double.IsFinite(longitud)
        && latitud is >= -90 and <= 90
        && longitud is >= -180 and <= 180;

    /// <summary>Distancia en kilómetros por la fórmula de Haversine.</summary>
    public double DistanciaKm(Coordenada otra)
    {
        ArgumentNullException.ThrowIfNull(otra);

        var deltaLat = ARadianes(otra.Latitud - Latitud);
        var deltaLon = ARadianes(otra.Longitud - Longitud);

        var a = Math.Pow(Math.Sin(deltaLat / 2), 2)
              + Math.Cos(ARadianes(Latitud)) * Math.Cos(ARadianes(otra.Latitud))
              * Math.Pow(Math.Sin(deltaLon / 2), 2);

        return 2 * RadioTierraKm * Math.Asin(Math.Min(1, Math.Sqrt(a)));
    }

    /// <summary>Rectángulo que contiene todos los puntos a un radio dado (prefiltro barato para la BD).</summary>
    public CuadranteGeografico Alrededor(double radioKm)
    {
        var deltaLat = radioKm / KmPorGradoConservador;
        var coseno = Math.Max(Math.Cos(ARadianes(Latitud)), 0.01);
        var deltaLon = radioKm / (KmPorGradoConservador * coseno);

        return new CuadranteGeografico(
            Sur: Math.Max(-90, Latitud - deltaLat),
            Norte: Math.Min(90, Latitud + deltaLat),
            Oeste: Math.Max(-180, Longitud - deltaLon),
            Este: Math.Min(180, Longitud + deltaLon));
    }

    private static double ARadianes(double grados) => grados * Math.PI / 180.0;
}
