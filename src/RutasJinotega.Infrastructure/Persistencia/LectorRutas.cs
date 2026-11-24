using Microsoft.EntityFrameworkCore;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Domain.Catalogo;
using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Infrastructure.Persistencia;

/// <summary>
/// Rutas curadas de la fase actual. La definición es estática y las paradas se resuelven
/// contra los lugares existentes en la base de datos.
/// </summary>
internal sealed class LectorRutas(RutasJinotegaDbContext db) : ILectorRutas
{
    private const string MunicipioSanRafael = "San Rafael del Norte";

    private static readonly IReadOnlyList<RutaCurada> Rutas =
    [
        new RutaCurada(
            "san-rafael",
            "Ruta turística San Rafael",
            "Caminata urbana desde las mini pulperías del centro de San Rafael hasta el Tepeyac, centro de retiro y referente religioso del municipio.",
            MunicipioSanRafael,
            [
                new ParadaCurada("(Ejemplo) Mini pulpería La Esquina", 1, 10, "Punto de partida. Compra agua y algo de comer para la caminata."),
                new ParadaCurada("(Ejemplo) Mini pulpería El Camino", 2, 10, "Pulpería de barrio sobre la ruta al Tepeyac."),
                new ParadaCurada("(Ejemplo) Mini pulpería Doña Rosa", 3, 10, "Última parada de comercio local antes de la subida."),
                new ParadaCurada("Iglesia de San Rafael del Norte", 4, 15, "Parada cultural: aquí se casaron Sandino y Blanca Aráuz en 1927."),
                new ParadaCurada("El Tepeyac", 5, 20, "Destino: centro de retiro, Vía Crucis y santuario propuesto de Nicaragua.")
            ])
    ];

    public async Task<Ruta?> ObtenerPorClaveAsync(string clave, CancellationToken ct = default)
    {
        var definicion = Rutas.FirstOrDefault(r =>
            string.Equals(r.Clave, clave, StringComparison.OrdinalIgnoreCase));

        if (definicion is null)
        {
            return null;
        }

        var municipio = await db.Municipios
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Nombre == definicion.Municipio, ct);

        if (municipio is null)
        {
            return null;
        }

        var nombres = definicion.Paradas.Select(p => p.Nombre).ToArray();
        var lugares = await db.Lugares
            .AsNoTracking()
            .Include(l => l.Municipio)
            .Include(l => l.Categorias)
            .Where(l => l.MunicipioId == municipio.Id && nombres.Contains(l.Nombre))
            .ToListAsync(ct);

        var porNombre = lugares
            .GroupBy(l => l.Nombre, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        if (definicion.Paradas.Any(p => !porNombre.ContainsKey(p.Nombre)))
        {
            return null;
        }

        var ruta = Ruta.Crear(
            municipio,
            definicion.Clave,
            definicion.Nombre,
            definicion.Descripcion);

        foreach (var parada in definicion.Paradas.OrderBy(p => p.Orden))
        {
            ruta.AgregarParada(
                porNombre[parada.Nombre],
                parada.Orden,
                parada.TiempoEstimadoMin,
                parada.Nota);
        }

        return ruta;
    }

    private sealed record RutaCurada(
        string Clave,
        string Nombre,
        string Descripcion,
        string Municipio,
        IReadOnlyList<ParadaCurada> Paradas);

    private sealed record ParadaCurada(
        string Nombre,
        int Orden,
        int? TiempoEstimadoMin,
        string? Nota);
}
