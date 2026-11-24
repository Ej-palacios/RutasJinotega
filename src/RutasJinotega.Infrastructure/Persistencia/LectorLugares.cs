using Microsoft.EntityFrameworkCore;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Infrastructure.Persistencia;

/// <summary>Implementación EF Core del puerto de lectura de lugares (solo lugares activos).</summary>
internal sealed class LectorLugares(RutasJinotegaDbContext db) : ILectorLugares
{
    public async Task<Lugar?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        await ConsultaBase()
            .Include(l => l.Fotos)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<IReadOnlyList<Lugar>> ListarAsync(FiltroLugares filtro, CancellationToken ct = default)
    {
        var consulta = ConsultaBase();

        if (filtro.MunicipioId is { } municipioId)
        {
            consulta = consulta.Where(l => l.MunicipioId == municipioId);
        }

        if (filtro.CategoriaId is { } categoriaId)
        {
            consulta = consulta.Where(l => l.Categorias.Any(c => c.Id == categoriaId));
        }

        if (filtro.SoloPocoConocidos)
        {
            consulta = consulta.Where(l => l.EsPocoConocido);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var texto = filtro.Texto.Trim();
            consulta = consulta.Where(l => l.Nombre.Contains(texto));
        }

        return await consulta.OrderBy(l => l.Nombre).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Lugar>> ListarEnCuadranteAsync(
        CuadranteGeografico cuadrante,
        int? categoriaId,
        CancellationToken ct = default)
    {
        var consulta = ConsultaBase().Where(l =>
            l.Ubicacion.Latitud >= cuadrante.Sur && l.Ubicacion.Latitud <= cuadrante.Norte &&
            l.Ubicacion.Longitud >= cuadrante.Oeste && l.Ubicacion.Longitud <= cuadrante.Este);

        if (categoriaId is { } id)
        {
            consulta = consulta.Where(l => l.Categorias.Any(c => c.Id == id));
        }

        return await consulta.ToListAsync(ct);
    }

    private IQueryable<Lugar> ConsultaBase() =>
        db.Lugares
            .AsNoTracking()
            .Include(l => l.Municipio)
            .Include(l => l.Categorias)
            .Where(l => l.Estado == EstadoLugar.Activo);
}
