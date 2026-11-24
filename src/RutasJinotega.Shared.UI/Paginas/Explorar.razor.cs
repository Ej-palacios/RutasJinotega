using Microsoft.AspNetCore.Components;
using RutasJinotega.Application.Abstracciones;
using RutasJinotega.Application.Catalogo;
using RutasJinotega.Domain.Comun;
using RutasJinotega.Shared.UI.Modelos;
using RutasJinotega.Shared.UI.Servicios;

namespace RutasJinotega.Shared.UI.Paginas;

public partial class Explorar
{
    private IReadOnlyList<CategoriaDto> _categorias = [];
    private IReadOnlyList<ItemLugar> _items = [];
    private ItemLugar? _seleccionado;
    private Coordenada? _origen;
    private bool _cargando = true;
    private string? _error;

    [Inject] private ILugaresClient Cliente { get; set; } = default!;

    [Inject] private IServicioUbicacion Ubicacion { get; set; } = default!;

    [Inject] private NavigationManager Navegacion { get; set; } = default!;

    private string? Texto { get; set; }

    private int? CategoriaId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CargarCatalogoAsync();
        await CargarAsync();

        // La ubicación se pide después de mostrar los lugares para no bloquear la pantalla.
        _origen = await Ubicacion.ObtenerUbicacionActualAsync();
        if (_origen is not null)
        {
            await CargarAsync();
        }
    }

    private async Task CargarCatalogoAsync()
    {
        var catalogo = await Cliente.ObtenerCatalogoAsync();
        if (catalogo.EsExito)
        {
            _categorias = catalogo.Valor.Categorias;
        }
    }

    private async Task CargarAsync()
    {
        _cargando = true;
        _error = null;
        _seleccionado = null;

        var filtro = new FiltroLugares(null, CategoriaId, false, Texto);
        var resultado = await Cliente.ListarAsync(filtro);

        _cargando = false;

        if (!resultado.EsExito)
        {
            _items = [];
            _error = resultado.Error.Mensaje;
            return;
        }

        _items = Ordenar(resultado.Valor.Select(l => ItemLugar.Desde(l, _origen)));
    }

    private async Task FiltrarCategoriaAsync(int? categoriaId)
    {
        CategoriaId = categoriaId;
        await CargarAsync();
    }

    private void Seleccionar(ItemLugar item) => _seleccionado = item;

    private void VerDetalle(ItemLugar item) => Navegacion.NavigateTo($"lugar/{item.Lugar.Id}");

    private static IReadOnlyList<ItemLugar> Ordenar(IEnumerable<ItemLugar> items) =>
        items.OrderBy(i => i.DistanciaKm ?? double.MaxValue).ThenBy(i => i.Lugar.Nombre).ToList();
}
