namespace Fase08.EventBus.Distribuido.Core.Outbox;

public sealed class MensagemOutbox
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string TipoEvento { get; init; } = string.Empty;
    public string PayloadJson { get; init; } = string.Empty;
    public DateTime CriadoEmUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessadoEmUtc { get; set; }
    public int Tentativas { get; set; }
    public string? UltimoErro { get; set; }

    public bool EstaProcessada => ProcessadoEmUtc.HasValue;
}
