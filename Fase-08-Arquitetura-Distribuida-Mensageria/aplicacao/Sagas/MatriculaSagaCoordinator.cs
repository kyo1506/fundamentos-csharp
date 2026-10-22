namespace Fase08.EventBus.Distribuido.Core.Sagas;

using System.Collections.Concurrent;
using Fase08.EventBus.Distribuido.Core.Barramento;
using Fase08.EventBus.Distribuido.Core.Modelos;

public sealed class MatriculaSagaCoordinator :
    IConsumidorEvento<PagamentoAprovadoEvento>,
    IConsumidorEvento<PagamentoRecusadoEvento>
{
    private readonly ConcurrentDictionary<Guid, MatriculaSagaContexto> _sagas = new();
    private readonly IEventBus _eventBus;

    public MatriculaSagaCoordinator(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public MatriculaSagaContexto IniciarSaga(int alunoId, int cursoId, decimal valor)
    {
        var saga = new MatriculaSagaContexto
        {
            AlunoId = alunoId,
            CursoId = cursoId,
            Valor = valor,
            Estado = EstadoSaga.Iniciada
        };

        saga.VagaReservada = true;
        saga.RegistrarEtapa($"Vaga reservada com sucesso para o curso {cursoId}.");

        saga.Estado = EstadoSaga.AguardandoPagamento;
        saga.RegistrarEtapa("Aguardando aprovacao do servico de pagamentos.");

        _sagas[saga.SagaId] = saga;
        return saga;
    }

    public Task ConsumirAsync(PagamentoAprovadoEvento evento, CancellationToken cancellationToken = default)
    {
        if (!_sagas.TryGetValue(evento.SagaId, out var saga))
        {
            return Task.CompletedTask;
        }

        saga.MatriculaAtiva = true;
        saga.Estado = EstadoSaga.Concluida;
        saga.RegistrarEtapa($"Pagamento de {evento.Valor:C} confirmado. Matricula ativada.");

        return Task.CompletedTask;
    }

    public Task ConsumirAsync(PagamentoRecusadoEvento evento, CancellationToken cancellationToken = default)
    {
        if (!_sagas.TryGetValue(evento.SagaId, out var saga))
        {
            return Task.CompletedTask;
        }

        saga.Estado = EstadoSaga.Compensando;
        saga.MotivoFalha = evento.Motivo;
        saga.RegistrarEtapa($"Falha no pagamento: {evento.Motivo}. Iniciando compensacao.");

        // Acao compensatoria: liberar a vaga reservada
        saga.VagaReservada = false;
        saga.RegistrarEtapa("Compensacao concluida: reserva de vaga cancelada.");

        saga.Estado = EstadoSaga.FalhaCompensada;
        return Task.CompletedTask;
    }

    public MatriculaSagaContexto? ObterSaga(Guid sagaId)
    {
        _sagas.TryGetValue(sagaId, out var saga);
        return saga;
    }
}
