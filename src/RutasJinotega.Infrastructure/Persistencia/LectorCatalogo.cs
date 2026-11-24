using Microsoft.EntityFrameworkCore;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Domain.Catalogo;

namespace RutasJinotega.Infrastructure.Persistencia;

internal sealed class LectorCatalogo(RutasJinotegaDbContext db) : ILectorCatalogo
{
    public async Task<IReadOnlyList<Municipio>> ListarMunicipiosAsync(CancellationToken ct = default) =>
        await db.Municipios.AsNoTracking().OrderBy(m => m.Nombre).ToListAsync(ct);

    public async Task<IReadOnlyList<Categoria>> ListarCategoriasAsync(CancellationToken ct = default) =>
        await db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync(ct);
}
