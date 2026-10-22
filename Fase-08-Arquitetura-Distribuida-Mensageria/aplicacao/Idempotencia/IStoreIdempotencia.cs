namespace Fase08.EventBus.Distribuido.Core.Idempotencia;

public interface IStoreIdempotencia
{
    Task<bool> JaProcessadoAsync(string chaveIdempotencia, CancellationToken cancellationToken = default);
    Task RegistrarProcessamentoAsync(string chaveIdempotencia, TimeSpan? tempoVida = null, CancellationToken cancellationToken = default);
}
