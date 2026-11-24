using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Catalogo;

/// <summary>
/// Agregado raíz del catálogo: un sitio turístico (conocido o poco conocido).
/// Protege sus invariantes: nombre obligatorio, calificación entre 0 y 5, ubicación válida.
/// </summary>
public sealed class Lugar : Entidad
{
    private readonly List<Categoria> _categorias = [];
    private readonly List<FotoLugar> _fotos = [];

    private Lugar() { }

    public int MunicipioId { get; private set; }
    public Municipio Municipio { get; private set; } = null!;
    public string Nombre { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public string? Historia { get; private set; }
    public Coordenada Ubicacion { get; private set; } = null!;
    public string? Direccion { get; private set; }
    public string? Horario { get; private set; }
    public DificultadAcceso Dificultad { get; private set; }
    public bool EsPocoConocido { get; private set; }
    public EstadoLugar Estado { get; private set; } = EstadoLugar.Activo;
    public double PromedioCalificacion { get; private set; }
    public int TotalResenas { get; private set; }
    public DateTime CreadoEn { get; private set; }

    public IReadOnlyCollection<Categoria> Categorias => _categorias;
    public IReadOnlyCollection<FotoLugar> Fotos => _fotos;

    public static Lugar Crear(
        Municipio municipio,
        string nombre,
        string descripcion,
        Coordenada ubicacion,
        DificultadAcceso dificultad,
        bool esPocoConocido,
        string? direccion = null,
        string? horario = null,
        string? historia = null)
    {
        ArgumentNullException.ThrowIfNull(municipio);
        ArgumentNullException.ThrowIfNull(ubicacion);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        return new Lugar
        {
            Municipio = municipio,
            MunicipioId = municipio.Id,
            Nombre = nombre.Trim(),
            Descripcion = descripcion?.Trim() ?? string.Empty,
            Ubicacion = ubicacion,
            Dificultad = dificultad,
            EsPocoConocido = esPocoConocido,
            Direccion = direccion?.Trim(),
            Horario = horario?.Trim(),
            Historia = string.IsNullOrWhiteSpace(historia) ? null : historia.Trim(),
            CreadoEn = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Actualiza el contenido editorial y la ubicación sin tocar identidad, estado ni calificación.
    /// La usa el sembrador para refrescar datos verificados en bases ya existentes.
    /// </summary>
    public void ActualizarContenido(
        string descripcion,
        string? historia,
        string? direccion,
        string? horario,
        Coordenada ubicacion)
    {
        ArgumentNullException.ThrowIfNull(ubicacion);

        Descripcion = descripcion?.Trim() ?? string.Empty;
        Historia = string.IsNullOrWhiteSpace(historia) ? null : historia.Trim();
        Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion.Trim();
        Horario = string.IsNullOrWhiteSpace(horario) ? null : horario.Trim();
        Ubicacion = ubicacion;
    }

    public void AgregarCategoria(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        if (!_categorias.Contains(categoria))
        {
            _categorias.Add(categoria);
        }
    }

    public void AgregarFoto(string url) => _fotos.Add(FotoLugar.Crear(url, _fotos.Count));

    public void ActualizarCalificacion(double promedio, int totalResenas)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(promedio, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(promedio, 5);
        ArgumentOutOfRangeException.ThrowIfNegative(totalResenas);

        PromedioCalificacion = Math.Round(promedio, 2);
        TotalResenas = totalResenas;
    }

    public void Ocultar() => Estado = EstadoLugar.Oculto;

    public void Activar() => Estado = EstadoLugar.Activo;
}
