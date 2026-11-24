using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Catalogo;

public sealed class Categoria : Entidad
{
    private Categoria() { }

    public string Nombre { get; private set; } = string.Empty;
    public string Icono { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;

    public static Categoria Crear(string nombre, string icono, string color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(icono);
        ArgumentException.ThrowIfNullOrWhiteSpace(color);

        return new Categoria { Nombre = nombre.Trim(), Icono = icono, Color = color };
    }
}
