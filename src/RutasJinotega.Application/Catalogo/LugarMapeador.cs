using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Application.Catalogo;

/// <summary>Conversión explícita Entidad -> DTO. Nunca se exponen entidades fuera de Application.</summary>
internal static class LugarMapeador
{
    private const string IconoPredeterminado = "📍";

    public static LugarResumenDto AResumen(Lugar lugar) => new(
        lugar.Id,
        lugar.Nombre,
        lugar.Municipio?.Nombre ?? string.Empty,
        lugar.Ubicacion.Latitud,
        lugar.Ubicacion.Longitud,
        lugar.PromedioCalificacion,
        lugar.TotalResenas,
        lugar.EsPocoConocido,
        IconoPrincipal(lugar),
        lugar.Categorias.Select(c => c.Nombre).ToList());

    public static LugarDetalleDto ADetalle(Lugar lugar) => new(
        lugar.Id,
        lugar.Nombre,
        lugar.Municipio?.Nombre ?? string.Empty,
        lugar.Descripcion,
        lugar.Historia,
        lugar.Ubicacion.Latitud,
        lugar.Ubicacion.Longitud,
        lugar.Direccion,
        lugar.Horario,
        lugar.Dificultad.ToString(),
        lugar.EsPocoConocido,
        lugar.PromedioCalificacion,
        lugar.TotalResenas,
        IconoPrincipal(lugar),
        lugar.Categorias.Select(ACategoria).ToList(),
        lugar.Fotos.OrderBy(f => f.Orden).Select(f => f.Url).ToList());

    public static CategoriaDto ACategoria(Categoria categoria) =>
        new(categoria.Id, categoria.Nombre, categoria.Icono, categoria.Color);

    public static MunicipioDto AMunicipio(Municipio municipio) =>
        new(municipio.Id, municipio.Nombre);

    private static string IconoPrincipal(Lugar lugar) =>
        lugar.Categorias.FirstOrDefault()?.Icono ?? IconoPredeterminado;
}
