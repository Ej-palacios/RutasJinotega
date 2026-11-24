using Microsoft.AspNetCore.Components.WebView.Maui;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Maui.Servicios;
using RutasJinotega.Shared.UI.Servicios;

namespace RutasJinotega.Maui;

public static class MauiProgram
{
    /// <summary>Raíz de composición de la app móvil: conecta la UI compartida con los servicios nativos.</summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        builder.Services.AgregarClienteApi(new Uri(ConfiguracionApp.UrlApi));
        builder.Services.AddSingleton<IServicioUbicacion, ServicioUbicacionMaui>();

        return builder.Build();
    }
}
