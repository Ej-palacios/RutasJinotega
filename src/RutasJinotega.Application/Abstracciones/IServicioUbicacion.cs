using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Application.Abstracciones;

/// <summary>
/// Puerto de ubicación del dispositivo. MAUI lo implementa con el GPS nativo
/// y Web con la API de geolocalización del navegador.
/// Devuelve null si no hay permiso o no se pudo obtener la posición.
/// </summary>
public interface IServicioUbicacion
{
    Task<Coordenada?> ObtenerUbicacionActualAsync(CancellationToken ct = default);
}
