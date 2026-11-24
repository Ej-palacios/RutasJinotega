namespace RutasJinotega.Domain.Comun;

/// <summary>Rectángulo geográfico delimitado por latitudes y longitudes.</summary>
public sealed record CuadranteGeografico(double Sur, double Norte, double Oeste, double Este)
{
    public bool Contiene(Coordenada punto)
    {
        ArgumentNullException.ThrowIfNull(punto);

        return punto.Latitud >= Sur && punto.Latitud <= Norte
            && punto.Longitud >= Oeste && punto.Longitud <= Este;
    }
}
