namespace Fase08.EventBus.Distribuido.Core.Modelos;

public abstract record EventoDistribuido(
    Guid EventId,
    DateTime OccurredOnUtc,
    string EventType
);

public sealed record AlunoMatriculadoEvento(
    Guid EventId,
    DateTime OccurredOnUtc,
    int AlunoId,
    string NomeAluno,
    string EmailAluno,
    int CursoId,
    decimal Valor
) : EventoDistribuido(EventId, OccurredOnUtc, nameof(AlunoMatriculadoEvento))
{
    public static AlunoMatriculadoEvento Criar(int alunoId, string nome, string email, int cursoId, decimal valor)
        => new(Guid.NewGuid(), DateTime.UtcNow, alunoId, nome, email, cursoId, valor);
}

public sealed record PagamentoAprovadoEvento(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid SagaId,
    int AlunoId,
    decimal Valor
) : EventoDistribuido(EventId, OccurredOnUtc, nameof(PagamentoAprovadoEvento));

public sealed record PagamentoRecusadoEvento(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid SagaId,
    int AlunoId,
    string Motivo
) : EventoDistribuido(EventId, OccurredOnUtc, nameof(PagamentoRecusadoEvento));
