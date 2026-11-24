namespace RutasJinotega.Shared.UI.Modelos;

/// <summary>Punto que se dibuja en el mapa de calles o en el esquema de respaldo.</summary>
public sealed record PuntoMapa(
    int Id,
    double Latitud,
    double Longitud,
    string Icono,
    string Titulo);

/// <summary>Vértice de la línea que une las paradas de una ruta.</summary>
public sealed record CoordenadaMapa(double Latitud, double Longitud);
