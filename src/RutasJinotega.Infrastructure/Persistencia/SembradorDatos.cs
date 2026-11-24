using Microsoft.EntityFrameworkCore;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Comun;
using RutasJinotega.Infrastructure.Persistencia.Datos;

namespace RutasJinotega.Infrastructure.Persistencia;

/// <summary>
/// Siembra idempotente del catálogo. Inserta lo que falta y refresca el contenido editorial
/// de los lugares ya existentes, sin borrar datos ni depender de un esquema concreto.
/// </summary>
public static class SembradorDatos
{
    public static async Task SembrarAsync(RutasJinotegaDbContext db, CancellationToken ct = default)
    {
        var municipios = await AsegurarMunicipiosAsync(db, ct);
        var categorias = await AsegurarCategoriasAsync(db, ct);
        var lugares = await db.Lugares
            .Include(l => l.Categorias)
            .ToListAsync(ct);

        foreach (var semilla in DatosSemilla.Lugares)
        {
            var municipio = municipios[semilla.Municipio];
            var lugar = lugares.FirstOrDefault(l =>
                l.MunicipioId == municipio.Id
                && string.Equals(l.Nombre, semilla.Nombre, StringComparison.OrdinalIgnoreCase));

            if (lugar is null)
            {
                lugar = Crear(municipio, semilla);
                db.Lugares.Add(lugar);
                lugares.Add(lugar);
            }
            else
            {
                lugar.ActualizarContenido(
                    semilla.Descripcion,
                    semilla.Historia,
                    semilla.Direccion,
                    semilla.Horario,
                    new Coordenada(semilla.Latitud, semilla.Longitud));
            }

            foreach (var nombreCategoria in semilla.Categorias)
            {
                lugar.AgregarCategoria(categorias[nombreCategoria]);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static async Task<Dictionary<string, Municipio>> AsegurarMunicipiosAsync(
        RutasJinotegaDbContext db,
        CancellationToken ct)
    {
        var existentes = await db.Municipios.ToListAsync(ct);
        var porNombre = existentes.ToDictionary(m => m.Nombre, StringComparer.OrdinalIgnoreCase);

        foreach (var semilla in DatosSemilla.Municipios)
        {
            if (!porNombre.ContainsKey(semilla.Nombre))
            {
                var municipio = Municipio.Crear(semilla.Nombre, semilla.Descripcion);
                db.Municipios.Add(municipio);
                porNombre[semilla.Nombre] = municipio;
            }
        }

        await db.SaveChangesAsync(ct);
        return porNombre;
    }

    private static async Task<Dictionary<string, Categoria>> AsegurarCategoriasAsync(
        RutasJinotegaDbContext db,
        CancellationToken ct)
    {
        var existentes = await db.Categorias.ToListAsync(ct);
        var porNombre = existentes.ToDictionary(c => c.Nombre, StringComparer.OrdinalIgnoreCase);

        foreach (var semilla in DatosSemilla.Categorias)
        {
            if (!porNombre.ContainsKey(semilla.Nombre))
            {
                var categoria = Categoria.Crear(semilla.Nombre, semilla.Icono, semilla.Color);
                db.Categorias.Add(categoria);
                porNombre[semilla.Nombre] = categoria;
            }
        }

        await db.SaveChangesAsync(ct);
        return porNombre;
    }

    private static Lugar Crear(Municipio municipio, LugarSemilla semilla)
    {
        var lugar = Lugar.Crear(
            municipio,
            semilla.Nombre,
            semilla.Descripcion,
            new Coordenada(semilla.Latitud, semilla.Longitud),
            semilla.Dificultad,
            semilla.EsPocoConocido,
            semilla.Direccion,
            semilla.Horario,
            semilla.Historia);

        lugar.ActualizarCalificacion(semilla.Calificacion, semilla.TotalResenas);
        return lugar;
    }
}
