namespace Fase08.EventBus.Distribuido.Tests;

using System.Text.Json;
using Fase08.EventBus.Distribuido.Core.Barramento;
using Fase08.EventBus.Distribuido.Core.Idempotencia;
using Fase08.EventBus.Distribuido.Core.Modelos;
using Fase08.EventBus.Distribuido.Core.Outbox;
using Fase08.EventBus.Distribuido.Core.Sagas;
using Xunit;

public class EventBusDistribuidoTests
{
    private sealed class ConsumidorEspiao<T> : IConsumidorEvento<T> where T : EventoDistribuido
    {
        public List<T> EventosRecebidos { get; } = new();

        public Task ConsumirAsync(T evento, CancellationToken cancellationToken = default)
        {
            EventosRecebidos.Add(evento);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task EventBus_DeveNotificarConsumidorInscrito()
    {
        var bus = new InMemoryEventBus();
        var consumidor = new ConsumidorEspiao<AlunoMatriculadoEvento>();
        bus.Inscrever(consumidor);

        var evento = AlunoMatriculadoEvento.Criar(1, "Linus Torvalds", "linus@linux.org", 5, 300.00m);
        await bus.PublicarAsync(evento);

        Assert.Single(consumidor.EventosRecebidos);
        Assert.Equal(evento.EventId, consumidor.EventosRecebidos[0].EventId);
        Assert.Equal("Linus Torvalds", consumidor.EventosRecebidos[0].NomeAluno);
    }

    [Fact]
    public async Task OutboxRepository_DevePersistirERecuperarMensagensPendentes()
    {
        var repo = new InMemoryOutboxRepository();
        var mensagem = new MensagemOutbox
        {
            TipoEvento = nameof(AlunoMatriculadoEvento),
            PayloadJson = "{}"
        };

        await repo.AdicionarAsync(mensagem);
        var pendentes = await repo.ObterPendentesAsync();

        Assert.Single(pendentes);
        Assert.False(pendentes[0].EstaProcessada);

        await repo.MarcarComoProcessadoAsync(mensagem.Id);
        var pendentesAposProcessar = await repo.ObterPendentesAsync();
        Assert.Empty(pendentesAposProcessar);
    }

    [Fact]
    public async Task DespachanteOutbox_DeveProcessarMensagensEPublicarNoBarramento()
    {
        var bus = new InMemoryEventBus();
        var repo = new InMemoryOutboxRepository();
        var despachante = new DespachanteOutbox(repo, bus);
        despachante.RegistrarTipoEvento<AlunoMatriculadoEvento>();

        var consumidor = new ConsumidorEspiao<AlunoMatriculadoEvento>();
        bus.Inscrever(consumidor);

        var evento = AlunoMatriculadoEvento.Criar(2, "Margaret Hamilton", "margaret@nasa.gov", 8, 500.00m);
        await repo.AdicionarAsync(new MensagemOutbox
        {
            Id = evento.EventId,
            TipoEvento = nameof(AlunoMatriculadoEvento),
            PayloadJson = JsonSerializer.Serialize(evento)
        });

        var processados = await despachante.DespacharPendentesAsync();

        Assert.Equal(1, processados);
        Assert.Single(consumidor.EventosRecebidos);
        Assert.Equal("Margaret Hamilton", consumidor.EventosRecebidos[0].NomeAluno);

        var pendentes = await repo.ObterPendentesAsync();
        Assert.Empty(pendentes);
    }

    [Fact]
    public async Task ConsumidorIdempotenteDecorator_DeveIgnorarChamadasDuplicadas()
    {
        var store = new InMemoryStoreIdempotencia();
        var espiao = new ConsumidorEspiao<AlunoMatriculadoEvento>();
        var decorator = new ConsumidorIdempotenteDecorator<AlunoMatriculadoEvento>(espiao, store);

        var evento = AlunoMatriculadoEvento.Criar(3, "Grace Hopper", "grace@navy.mil", 1, 150.00m);

        // Primeira execucao
        await decorator.ConsumirAsync(evento);
        // Segunda execucao duplicada
        await decorator.ConsumirAsync(evento);

        Assert.Single(espiao.EventosRecebidos);
    }

    [Fact]
    public async Task SagaCoordinator_FluxoComSucesso_DeveAtivarMatricula()
    {
        var bus = new InMemoryEventBus();
        var sagaCoordinator = new MatriculaSagaCoordinator(bus);
        bus.Inscrever<PagamentoAprovadoEvento>(sagaCoordinator);

        var saga = sagaCoordinator.IniciarSaga(alunoId: 10, cursoId: 3, valor: 250.00m);
        Assert.Equal(EstadoSaga.AguardandoPagamento, saga.Estado);
        Assert.True(saga.VagaReservada);
        Assert.False(saga.MatriculaAtiva);

        var eventoPagamento = new PagamentoAprovadoEvento(
            Guid.NewGuid(),
            DateTime.UtcNow,
            saga.SagaId,
            10,
            250.00m
        );
        await bus.PublicarAsync(eventoPagamento);

        Assert.Equal(EstadoSaga.Concluida, saga.Estado);
        Assert.True(saga.MatriculaAtiva);
        Assert.True(saga.VagaReservada);
    }

    [Fact]
    public async Task SagaCoordinator_FluxoComFalha_DeveExecutarCompensacaoELiberarVaga()
    {
        var bus = new InMemoryEventBus();
        var sagaCoordinator = new MatriculaSagaCoordinator(bus);
        bus.Inscrever<PagamentoRecusadoEvento>(sagaCoordinator);

        var saga = sagaCoordinator.IniciarSaga(alunoId: 11, cursoId: 4, valor: 350.00m);
        Assert.Equal(EstadoSaga.AguardandoPagamento, saga.Estado);
        Assert.True(saga.VagaReservada);

        var eventoRecusa = new PagamentoRecusadoEvento(
            Guid.NewGuid(),
            DateTime.UtcNow,
            saga.SagaId,
            11,
            "Saldo insuficiente"
        );
        await bus.PublicarAsync(eventoRecusa);

        Assert.Equal(EstadoSaga.FalhaCompensada, saga.Estado);
        Assert.False(saga.VagaReservada);
        Assert.False(saga.MatriculaAtiva);
        Assert.Equal("Saldo insuficiente", saga.MotivoFalha);
    }
}
