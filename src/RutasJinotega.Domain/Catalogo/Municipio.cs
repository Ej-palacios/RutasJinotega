using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Catalogo;

public sealed class Municipio : Entidad
{
    private Municipio() { }

    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }

    public static Municipio Crear(string nombre, string? descripcion = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);

        return new Municipio { Nombre = nombre.Trim(), Descripcion = descripcion?.Trim() };
    }
}
