namespace Fase08.EventBus.Distribuido.Core.Outbox;

using System.Collections.Concurrent;

public sealed class InMemoryOutboxRepository : IOutboxRepository
{
    private readonly ConcurrentDictionary<Guid, MensagemOutbox> _tabela = new();

    public Task AdicionarAsync(MensagemOutbox mensagem, CancellationToken cancellationToken = default)
    {
        _tabela[mensagem.Id] = mensagem;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<MensagemOutbox>> ObterPendentesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        var pendentes = _tabela.Values
            .Where(m => !m.EstaProcessada)
            .OrderBy(m => m.CriadoEmUtc)
            .Take(limite)
            .ToList();

        return Task.FromResult<IReadOnlyList<MensagemOutbox>>(pendentes);
    }

    public Task MarcarComoProcessadoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (_tabela.TryGetValue(id, out var msg))
        {
            msg.ProcessadoEmUtc = DateTime.UtcNow;
            msg.UltimoErro = null;
        }

        return Task.CompletedTask;
    }

    public Task RegistrarFalhaAsync(Guid id, string erro, CancellationToken cancellationToken = default)
    {
        if (_tabela.TryGetValue(id, out var msg))
        {
            msg.Tentativas++;
            msg.UltimoErro = erro;
        }

        return Task.CompletedTask;
    }
}
