using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Rutas;

namespace RutasJinotega.Application.Rutas;

/// <summary>Conversión explícita Ruta -> DTO. Nunca se exponen entidades fuera de Application.</summary>
internal static class RutaMapeador
{
    public static RutaDetalleDto ADetalle(Ruta ruta)
    {
        var paradas = ruta.Paradas.OrderBy(p => p.Orden).ToList();
        var distanciaAcumulada = 0d;
        var resultado = new List<RutaParadaDto>(paradas.Count);

        for (var i = 0; i < paradas.Count; i++)
        {
            if (i > 0)
            {
                distanciaAcumulada += paradas[i - 1].Lugar.Ubicacion.DistanciaKm(paradas[i].Lugar.Ubicacion);
            }

            var parada = paradas[i];
            resultado.Add(new RutaParadaDto(
                parada.Orden,
                LugarMapeador.AResumen(parada.Lugar),
                Math.Round(distanciaAcumulada, 1),
                parada.TiempoEstimadoMin,
                parada.Nota));
        }

        var duracion = paradas.Sum(p => p.TiempoEstimadoMin ?? 0);

        return new RutaDetalleDto(
            ruta.Clave,
            ruta.Nombre,
            ruta.Descripcion,
            ruta.EsCurada,
            ruta.Municipio.Nombre,
            Math.Round(distanciaAcumulada, 1),
            duracion,
            resultado);
    }
}
