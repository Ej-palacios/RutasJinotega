using Microsoft.JSInterop;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Shared.UI.Servicios;

/// <summary>Implementación web de IServicioUbicacion usando navigator.geolocation vía JS interop.</summary>
public sealed class ServicioUbicacionWeb(IJSRuntime js) : IServicioUbicacion, IAsyncDisposable
{
    private const string RutaModulo = "./_content/RutasJinotega.Shared.UI/js/ubicacion.js";

    private IJSObjectReference? _modulo;

    public async Task<Coordenada?> ObtenerUbicacionActualAsync(CancellationToken ct = default)
    {
        try
        {
            _modulo ??= await js.InvokeAsync<IJSObjectReference>("import", RutaModulo);
            var posicion = await _modulo.InvokeAsync<PosicionJs?>("obtener");

            return posicion is not null && Coordenada.EsValida(posicion.Latitud, posicion.Longitud)
                ? new Coordenada(posicion.Latitud, posicion.Longitud)
                : null;
        }
        catch (JSException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_modulo is not null)
        {
            try
            {
                await _modulo.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // El circuito o la página ya se cerraron; no hay nada que liberar.
            }
        }
    }
}

internal sealed record PosicionJs(double Latitud, double Longitud);
