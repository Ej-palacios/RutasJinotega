using RutasJinotega.Application.Catalogo;

namespace RutasJinotega.Application.Rutas;

public sealed record RutaParadaDto(
    int Orden,
    LugarResumenDto Lugar,
    double DistanciaAcumuladaKm,
    int? TiempoEstimadoMin,
    string? Nota);

public sealed record RutaDetalleDto(
    string Clave,
    string Nombre,
    string Descripcion,
    bool EsCurada,
    string Municipio,
    double DistanciaTotalKm,
    int DuracionTotalMin,
    IReadOnlyList<RutaParadaDto> Paradas);
