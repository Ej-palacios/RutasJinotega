namespace RutasJinotega.Maui;

/// <summary>Configuración de la app móvil. En producción debe venir de configuración, no de una constante.</summary>
public static class ConfiguracionApp
{
#if ANDROID
    // El emulador de Android ve el equipo anfitrión en 10.0.2.2.
    // En un teléfono físico usa la IP de tu PC en la red local (ej. http://192.168.1.20:5100/).
    public const string UrlApi = "http://10.0.2.2:5100/";
#else
    public const string UrlApi = "http://localhost:5100/";
#endif
}
