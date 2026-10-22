# Fase 08: Arquitetura Distribuída, Mensageria e Event-Driven

Nesta fase avançada, você aprenderá a projetar e implementar sistemas distribuídos resilientes em C# e .NET. Abordaremos os desafios fundamentais de comunicação desacoplada entre serviços, o padrão de barramento de eventos, o Padrão Outbox Transacional para evitar o problema de dual-write, técnicas rigorosas de idempotência para consumo de mensagens duplicadas e orquestração de Sagas compensatórias em cenários de falha.

---

## Conteúdo Programático

| Aula | Tema | Arquivo |
|---|---|---|
| **08.1** | Fundamentos de Sistemas Distribuídos e Comunicação Assíncrona | [08.1-sistemas-distribuidos-comunicacao-assincrona.md](teoria/08.1-sistemas-distribuidos-comunicacao-assincrona.md) |
| **08.2** | Mensageria, Message Brokers e Abstrações de Barramento | [08.2-mensageria-message-brokers-event-bus.md](teoria/08.2-mensageria-message-brokers-event-bus.md) |
| **08.3** | Padrão Outbox Transacional (Transactional Outbox Pattern) | [08.3-padrao-outbox-transacional.md](teoria/08.3-padrao-outbox-transacional.md) |
| **08.4** | Idempotência e Desduplicação de Mensagens | [08.4-idempotencia-e-desduplicacao.md](teoria/08.4-idempotencia-e-desduplicacao.md) |
| **08.5** | Padrão Saga e Transações Compensatórias | [08.5-padrao-saga-transacoes-compensatorias.md](teoria/08.5-padrao-saga-transacoes-compensatorias.md) |

---

## Projeto Integrador: `EventBus.Distribuido.Core`

O projeto prático desta fase implementa um motor de mensageria e resiliência distribuída com:
1. **Barramento de Eventos (`IEventBus`):** roteamento e publicação assíncrona desacoplada de mensagens para múltiplos consumidores.
2. **Outbox Transacional (`OutboxDispatcher`):** persistência atômica de eventos em tabela de saída e despacho em segundo plano com controle de retentativas.
3. **Filtro de Idempotência (`IdempotentConsumerDecorator`):** garantia de que mensagens duplicadas na rede não reexecutem efeitos colaterais no negócio.
4. **Orquestrador de Saga (`MatriculaSagaCoordinator`):** máquina de estados com passos de reserva, faturamento e compensação automática em caso de inconsistência.
