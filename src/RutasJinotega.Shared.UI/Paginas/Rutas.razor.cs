using System.Globalization;
using Microsoft.AspNetCore.Components;
using RutasJinotega.Application.Rutas;
using RutasJinotega.Shared.UI.Modelos;
using RutasJinotega.Shared.UI.Servicios;

namespace RutasJinotega.Shared.UI.Paginas;

public partial class Rutas
{
    private const string ClaveRutaSanRafael = "san-rafael";

    private RutaDetalleDto? _ruta;
    private RutaParadaDto? _seleccionada;
    private bool _cargando = true;
    private string? _error;

    [Inject] private ILugaresClient Cliente { get; set; } = default!;

    [Inject] private NavigationManager Navegacion { get; set; } = default!;

    private IReadOnlyList<RutaParadaDto> Paradas => _ruta?.Paradas ?? [];

    private ItemLugar ItemSeleccionado => ItemLugar.Desde(_seleccionada!.Lugar, null);

    private string UrlIniciarRuta
    {
        get
        {
            if (_ruta is null || _ruta.Paradas.Count == 0)
            {
                return string.Empty;
            }

            var origen = _ruta.Paradas[0].Lugar;
            var destino = _ruta.Paradas[^1].Lugar;
            var intermedias = _ruta.Paradas
                .Skip(1)
                .Take(_ruta.Paradas.Count - 2)
                .Select(p => Coordenada(p.Lugar.Latitud, p.Lugar.Longitud))
                .ToList();

            var url = "https://www.google.com/maps/dir/?api=1" +
                      $"&origin={Coordenada(origen.Latitud, origen.Longitud)}" +
                      $"&destination={Coordenada(destino.Latitud, destino.Longitud)}";

            return intermedias.Count == 0
                ? url
                : url + $"&waypoints={Uri.EscapeDataString(string.Join("|", intermedias))}";
        }
    }

    protected override Task OnInitializedAsync() => CargarAsync();

    private async Task CargarAsync()
    {
        _cargando = true;
        _error = null;
        _seleccionada = null;

        var resultado = await Cliente.ObtenerRutaAsync(ClaveRutaSanRafael);

        _cargando = false;

        if (resultado.EsExito)
        {
            _ruta = resultado.Valor;
        }
        else
        {
            _ruta = null;
            _error = resultado.Error.Mensaje;
        }
    }

    private void Seleccionar(RutaParadaDto parada) => _seleccionada = parada;

    private bool EsSeleccionada(RutaParadaDto parada) =>
        _seleccionada?.Lugar.Id == parada.Lugar.Id;

    private void VerDetalle() =>
        Navegacion.NavigateTo($"lugar/{_seleccionada!.Lugar.Id}");

    private static string Coordenada(double latitud, double longitud) =>
        $"{latitud.ToString("R", CultureInfo.InvariantCulture)},{longitud.ToString("R", CultureInfo.InvariantCulture)}";
}
