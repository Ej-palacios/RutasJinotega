using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Application.Abstracciones;

/// <summary>Puerto de lectura de catálogos auxiliares (municipios y categorías).</summary>
public interface ILectorCatalogo
{
    Task<IReadOnlyList<Municipio>> ListarMunicipiosAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Categoria>> ListarCategoriasAsync(CancellationToken ct = default);
}
