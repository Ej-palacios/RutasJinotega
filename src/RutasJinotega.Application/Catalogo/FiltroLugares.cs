namespace RutasJinotega.Application.Catalogo;

/// <summary>Criterios opcionales para listar lugares.</summary>
public sealed record FiltroLugares(
    int? MunicipioId = null,
    int? CategoriaId = null,
    bool SoloPocoConocidos = false,
    string? Texto = null)
{
    public const int LongitudMaximaTexto = 100;
}
