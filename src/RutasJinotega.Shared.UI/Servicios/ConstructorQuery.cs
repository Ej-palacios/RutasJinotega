using System.Globalization;

namespace RutasJinotega.Shared.UI.Servicios;

/// <summary>Arma query strings con cultura invariante (los decimales siempre con punto).</summary>
internal static class ConstructorQuery
{
    public static string Crear(params (string Nombre, object? Valor)[] parametros)
    {
        var partes = parametros
            .Where(p => p.Valor is not null)
            .Select(p => $"{Uri.EscapeDataString(p.Nombre)}={Uri.EscapeDataString(Formatear(p.Valor!))}")
            .ToList();

        return partes.Count == 0 ? string.Empty : "?" + string.Join('&', partes);
    }

    private static string Formatear(object valor) => valor switch
    {
        double d => d.ToString("R", CultureInfo.InvariantCulture),
        bool b => b ? "true" : "false",
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => valor.ToString() ?? string.Empty
    };
}
