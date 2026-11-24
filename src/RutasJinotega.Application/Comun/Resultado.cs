using System.Diagnostics.CodeAnalysis;

namespace RutasJinotega.Application.Comun;

/// <summary>
/// Resultado de un caso de uso: éxito con valor, o error esperado.
/// Evita usar excepciones para el flujo normal.
/// </summary>
public sealed class Resultado<T>
{
    private Resultado(T? valor, Error? error)
    {
        Valor = valor;
        Error = error;
    }

    [MemberNotNullWhen(true, nameof(Valor))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool EsExito => Error is null;

    public T? Valor { get; }

    public Error? Error { get; }

    public static Resultado<T> Exito(T valor) => new(valor, null);

    public static Resultado<T> Falla(Error error) => new(default, error);
}
