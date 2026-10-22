namespace Fase08.EventBus.Distribuido.Core.Idempotencia;

using System.Collections.Concurrent;

public sealed class InMemoryStoreIdempotencia : IStoreIdempotencia
{
    private readonly ConcurrentDictionary<string, DateTime> _chaves = new();

    public Task<bool> JaProcessadoAsync(string chaveIdempotencia, CancellationToken cancellationToken = default)
    {
        if (_chaves.TryGetValue(chaveIdempotencia, out var expiracao))
        {
            if (DateTime.UtcNow <= expiracao)
            {
                return Task.FromResult(true);
            }

            _chaves.TryRemove(chaveIdempotencia, out _);
        }

        return Task.FromResult(false);
    }

    public Task RegistrarProcessamentoAsync(string chaveIdempotencia, TimeSpan? tempoVida = null, CancellationToken cancellationToken = default)
    {
        var limite = DateTime.UtcNow.Add(tempoVida ?? TimeSpan.FromHours(24));
        _chaves[chaveIdempotencia] = limite;
        return Task.CompletedTask;
    }
}
