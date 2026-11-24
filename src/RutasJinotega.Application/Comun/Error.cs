namespace RutasJinotega.Application.Comun;

/// <summary>Error esperado de negocio (no es una excepción).</summary>
public sealed record Error(string Codigo, string Mensaje)
{
    public const string CodigoValidacion = "validacion";
    public const string CodigoNoEncontrado = "no_encontrado";
    public const string CodigoRed = "red";

    public static Error Validacion(string mensaje) => new(CodigoValidacion, mensaje);

    public static Error NoEncontrado(string mensaje) => new(CodigoNoEncontrado, mensaje);

    public static Error Red(string mensaje) => new(CodigoRed, mensaje);
}
