using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Maui.Servicios;

/// <summary>Implementación móvil de IServicioUbicacion con el GPS nativo.</summary>
public sealed class ServicioUbicacionMaui : IServicioUbicacion
{
    private static readonly TimeSpan TiempoMaximo = TimeSpan.FromSeconds(8);

    public async Task<Coordenada?> ObtenerUbicacionActualAsync(CancellationToken ct = default)
    {
        try
        {
            var permiso = await MainThread.InvokeOnMainThreadAsync(PedirPermisoAsync);
            if (permiso != PermissionStatus.Granted)
            {
                return null;
            }

            var peticion = new GeolocationRequest(GeolocationAccuracy.Medium, TiempoMaximo);
            var ubicacion = await Geolocation.Default.GetLocationAsync(peticion, ct);

            return ubicacion is not null && Coordenada.EsValida(ubicacion.Latitude, ubicacion.Longitude)
                ? new Coordenada(ubicacion.Latitude, ubicacion.Longitude)
                : null;
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException
                                      or FeatureNotEnabledException
                                      or PermissionException
                                      or TaskCanceledException)
        {
            return null;
        }
    }

    private static async Task<PermissionStatus> PedirPermisoAsync()
    {
        var estado = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        return estado == PermissionStatus.Granted
            ? estado
            : await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
}
