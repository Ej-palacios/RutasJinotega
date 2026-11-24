using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Shared.UI.Modelos;

/// <summary>Un lugar listo para mostrarse en pantalla, con su distancia al usuario si se conoce.</summary>
public sealed record ItemLugar(LugarResumenDto Lugar, double? DistanciaKm)
{
    public static ItemLugar Desde(LugarResumenDto lugar, Coordenada? origen)
    {
        double? distancia = origen is null
            ? null
            : Math.Round(origen.DistanciaKm(new Coordenada(lugar.Latitud, lugar.Longitud)), 1);

        return new ItemLugar(lugar, distancia);
    }
}
