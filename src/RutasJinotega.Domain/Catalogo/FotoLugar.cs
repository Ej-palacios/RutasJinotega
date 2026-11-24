using RutasJinotega.Domain.Comun;

namespace RutasJinotega.Domain.Catalogo;

public sealed class FotoLugar : Entidad
{
    private FotoLugar() { }

    public int LugarId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int Orden { get; private set; }

    public static FotoLugar Crear(string url, int orden)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentOutOfRangeException.ThrowIfNegative(orden);

        return new FotoLugar { Url = url.Trim(), Orden = orden };
    }
}
