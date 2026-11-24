namespace RutasJinotega.Application.Comun;

/// <summary>Contrato de un caso de uso de lectura (CQRS ligero).</summary>
public interface IConsultaHandler<in TConsulta, TRespuesta>
{
    Task<Resultado<TRespuesta>> EjecutarAsync(TConsulta consulta, CancellationToken ct = default);
}
