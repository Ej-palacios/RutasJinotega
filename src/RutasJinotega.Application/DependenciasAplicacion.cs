using Microsoft.Extensions.DependencyInjection;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Application.Comun;
using RutasJinotega.Application.Rutas;

namespace RutasJinotega.Application;

public static class DependenciasAplicacion
{
    /// <summary>Registra los casos de uso. Se llama desde la raíz de composición (Api).</summary>
    public static IServiceCollection AgregarAplicacion(this IServiceCollection servicios)
    {
        servicios.AddScoped<
            IConsultaHandler<ObtenerLugaresCercanosConsulta, IReadOnlyList<LugarCercanoDto>>,
            ObtenerLugaresCercanosHandler>();

        servicios.AddScoped<
            IConsultaHandler<ListarLugaresConsulta, IReadOnlyList<LugarResumenDto>>,
            ListarLugaresHandler>();

        servicios.AddScoped<
            IConsultaHandler<ObtenerDetalleLugarConsulta, LugarDetalleDto>,
            ObtenerDetalleLugarHandler>();

        servicios.AddScoped<
            IConsultaHandler<ObtenerCatalogoConsulta, CatalogoDto>,
            ObtenerCatalogoHandler>();

        servicios.AddScoped<
            IConsultaHandler<ObtenerRutaConsulta, RutaDetalleDto>,
            ObtenerRutaHandler>();

        return servicios;
    }
}
