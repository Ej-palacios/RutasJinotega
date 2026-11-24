using Microsoft.Extensions.DependencyInjection;

namespace RutasJinotega.Shared.UI.Servicios;

public static class ExtensionesServicios
{
    /// <summary>Registra el cliente HTTP tipado hacia la API. Lo llaman las apps anfitrionas (Web y MAUI).</summary>
    public static IServiceCollection AgregarClienteApi(this IServiceCollection servicios, Uri urlBase)
    {
        ArgumentNullException.ThrowIfNull(urlBase);

        var baseConBarra = new Uri(urlBase.AbsoluteUri.TrimEnd('/') + "/");
        servicios.AddHttpClient<ILugaresClient, HttpLugaresClient>(cliente => cliente.BaseAddress = baseConBarra);

        return servicios;
    }
}
