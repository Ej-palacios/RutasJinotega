using System.Globalization;
using Microsoft.AspNetCore.Components;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Shared.UI.Servicios;

namespace RutasJinotega.Shared.UI.Paginas;

public partial class DetalleLugar
{
    private LugarDetalleDto? _lugar;
    private bool _cargando = true;
    private string? _error;

    [Parameter] public int Id { get; set; }

    [Inject] private ILugaresClient Cliente { get; set; } = default!;

    [Inject] private NavigationManager Navegacion { get; set; } = default!;

    private string UrlComoLlegar
    {
        get
        {
            if (_lugar is null)
            {
                return string.Empty;
            }

            var lat = _lugar.Latitud.ToString("R", CultureInfo.InvariantCulture);
            var lon = _lugar.Longitud.ToString("R", CultureInfo.InvariantCulture);
            return $"https://www.google.com/maps/dir/?api=1&destination={lat},{lon}";
        }
    }

    protected override Task OnParametersSetAsync() => CargarAsync();

    private async Task CargarAsync()
    {
        _cargando = true;
        _error = null;

        var resultado = await Cliente.ObtenerDetalleAsync(Id);

        _cargando = false;

        if (resultado.EsExito)
        {
            _lugar = resultado.Valor;
        }
        else
        {
            _lugar = null;
            _error = resultado.Error.Mensaje;
        }
    }

    private void Volver() => Navegacion.NavigateTo(string.Empty);
}
