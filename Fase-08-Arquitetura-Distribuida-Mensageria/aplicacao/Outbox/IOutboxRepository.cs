namespace Fase08.EventBus.Distribuido.Core.Outbox;

public interface IOutboxRepository
{
    Task AdicionarAsync(MensagemOutbox mensagem, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MensagemOutbox>> ObterPendentesAsync(int limite = 10, CancellationToken cancellationToken = default);
    Task MarcarComoProcessadoAsync(Guid id, CancellationToken cancellationToken = default);
    Task RegistrarFalhaAsync(Guid id, string erro, CancellationToken cancellationToken = default);
}
