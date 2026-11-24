namespace RutasJinotega.Application.Catalogo;

public sealed record CategoriaDto(int Id, string Nombre, string Icono, string Color);

public sealed record MunicipioDto(int Id, string Nombre);

public sealed record CatalogoDto(
    IReadOnlyList<MunicipioDto> Municipios,
    IReadOnlyList<CategoriaDto> Categorias);

public sealed record LugarResumenDto(
    int Id,
    string Nombre,
    string Municipio,
    double Latitud,
    double Longitud,
    double PromedioCalificacion,
    int TotalResenas,
    bool EsPocoConocido,
    string IconoPrincipal,
    IReadOnlyList<string> Categorias);

public sealed record LugarCercanoDto(LugarResumenDto Lugar, double DistanciaKm);

public sealed record LugarDetalleDto(
    int Id,
    string Nombre,
    string Municipio,
    string Descripcion,
    string? Historia,
    double Latitud,
    double Longitud,
    string? Direccion,
    string? Horario,
    string Dificultad,
    bool EsPocoConocido,
    double PromedioCalificacion,
    int TotalResenas,
    string IconoPrincipal,
    IReadOnlyList<CategoriaDto> Categorias,
    IReadOnlyList<string> Fotos);
