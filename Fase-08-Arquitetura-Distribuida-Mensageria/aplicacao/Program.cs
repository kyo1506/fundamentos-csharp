namespace Fase08.EventBus.Distribuido.Core;

using System.Text.Json;
using Fase08.EventBus.Distribuido.Core.Barramento;
using Fase08.EventBus.Distribuido.Core.Idempotencia;
using Fase08.EventBus.Distribuido.Core.Modelos;
using Fase08.EventBus.Distribuido.Core.Outbox;
using Fase08.EventBus.Distribuido.Core.Sagas;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("==================================================================");
        Console.WriteLine("  Fase 08: Arquitetura Distribuida e Mensageria (Event-Driven)   ");
        Console.WriteLine("==================================================================");

        var eventBus = new InMemoryEventBus();
        var outboxRepo = new InMemoryOutboxRepository();
        var storeIdempotencia = new InMemoryStoreIdempotencia();

        var despachante = new DespachanteOutbox(outboxRepo, eventBus);
        despachante.RegistrarTipoEvento<AlunoMatriculadoEvento>();

        // 1. Configuracao de consumidor idempotente de AlunoMatriculado
        var consumidorBase = new ConsumidorBoasVindas();
        var consumidorIdempotente = new ConsumidorIdempotenteDecorator<AlunoMatriculadoEvento>(
            consumidorBase,
            storeIdempotencia
        );
        eventBus.Inscrever(consumidorIdempotente);

        // 2. Simulacao do Padrao Outbox
        Console.WriteLine("\n[1] Gravando evento no Outbox da transacao de negocio...");
        var eventoMatricula = AlunoMatriculadoEvento.Criar(101, "Ada Lovelace", "ada@exemplo.com", 8, 450.00m);
        var mensagemOutbox = new MensagemOutbox
        {
            Id = eventoMatricula.EventId,
            CorrelationId = Guid.NewGuid(),
            TipoEvento = nameof(AlunoMatriculadoEvento),
            PayloadJson = JsonSerializer.Serialize(eventoMatricula)
        };
        await outboxRepo.AdicionarAsync(mensagemOutbox);

        Console.WriteLine($"Evento persistido no Outbox com status pendente (Id: {mensagemOutbox.Id}).");

        Console.WriteLine("\n[2] Despachante Outbox processando mensagens pendentes...");
        var despachados = await despachante.DespacharPendentesAsync();
        Console.WriteLine($"Total de mensagens despachadas para o barramento: {despachados}.");

        // 3. Teste de Idempotencia: reenviando o mesmo evento
        Console.WriteLine("\n[3] Testando idempotencia: republicando o mesmo evento duplicado...");
        await eventBus.PublicarAsync(eventoMatricula);
        Console.WriteLine("O consumidor ignorou o duplicado com sucesso.");

        // 4. Simulacao de Saga Coreografada / Orquestrada
        Console.WriteLine("\n[4] Simulando Saga de Matricula com Transacao Compensatoria...");
        var sagaCoordinator = new MatriculaSagaCoordinator(eventBus);
        eventBus.Inscrever<PagamentoAprovadoEvento>(sagaCoordinator);
        eventBus.Inscrever<PagamentoRecusadoEvento>(sagaCoordinator);

        // Caso de Sucesso
        var sagaSucesso = sagaCoordinator.IniciarSaga(101, 8, 450.00m);
        Console.WriteLine($"Saga iniciada. Estado: {sagaSucesso.Estado}, Vaga reservada: {sagaSucesso.VagaReservada}");
        await eventBus.PublicarAsync(new PagamentoAprovadoEvento(Guid.NewGuid(), DateTime.UtcNow, sagaSucesso.SagaId, 101, 450.00m));
        Console.WriteLine($"Apos pagamento aprovado. Estado: {sagaSucesso.Estado}, Matricula ativa: {sagaSucesso.MatriculaAtiva}");

        // Caso de Falha e Compensacao
        Console.WriteLine("\n[5] Simulando Saga com Pagamento Recusado (Compensacao)...");
        var sagaFalha = sagaCoordinator.IniciarSaga(102, 8, 450.00m);
        Console.WriteLine($"Saga iniciada. Estado: {sagaFalha.Estado}, Vaga reservada: {sagaFalha.VagaReservada}");
        await eventBus.PublicarAsync(new PagamentoRecusadoEvento(Guid.NewGuid(), DateTime.UtcNow, sagaFalha.SagaId, 102, "Limite insuficiente no cartao."));
        Console.WriteLine($"Apos recusa. Estado: {sagaFalha.Estado}, Vaga reservada: {sagaFalha.VagaReservada}, Motivo: {sagaFalha.MotivoFalha}");

        Console.WriteLine("\nDemonstracao da Fase 08 finalizada com sucesso!");
    }
}

public sealed class ConsumidorBoasVindas : IConsumidorEvento<AlunoMatriculadoEvento>
{
    public int TotalProcessados { get; private set; }

    public Task ConsumirAsync(AlunoMatriculadoEvento evento, CancellationToken cancellationToken = default)
    {
        TotalProcessados++;
        Console.WriteLine($"   [ConsumidorBoasVindas] Enviando e-mail de boas-vindas para {evento.NomeAluno} ({evento.EmailAluno}). Total: {TotalProcessados}");
        return Task.CompletedTask;
    }
}
