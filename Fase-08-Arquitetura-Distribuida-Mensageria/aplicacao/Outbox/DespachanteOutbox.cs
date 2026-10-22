namespace Fase08.EventBus.Distribuido.Core.Outbox;

using System.Text.Json;
using Fase08.EventBus.Distribuido.Core.Barramento;
using Fase08.EventBus.Distribuido.Core.Modelos;

public sealed class DespachanteOutbox
{
    private readonly IOutboxRepository _repositorio;
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, Type> _tiposRegistrados = new(StringComparer.OrdinalIgnoreCase);

    public DespachanteOutbox(IOutboxRepository repositorio, IEventBus eventBus)
    {
        _repositorio = repositorio;
        _eventBus = eventBus;
    }

    public void RegistrarTipoEvento<TEvento>() where TEvento : EventoDistribuido
    {
        _tiposRegistrados[typeof(TEvento).Name] = typeof(TEvento);
    }

    public async Task<int> DespacharPendentesAsync(int lote = 10, CancellationToken cancellationToken = default)
    {
        var pendentes = await _repositorio.ObterPendentesAsync(lote, cancellationToken);
        var despachados = 0;

        foreach (var mensagem in pendentes)
        {
            try
            {
                if (!_tiposRegistrados.TryGetValue(mensagem.TipoEvento, out var tipo))
                {
                    await _repositorio.RegistrarFalhaAsync(
                        mensagem.Id,
                        $"Tipo de evento '{mensagem.TipoEvento}' nao registrado no despachante.",
                        cancellationToken);
                    continue;
                }

                var eventoObj = JsonSerializer.Deserialize(mensagem.PayloadJson, tipo);
                if (eventoObj is not EventoDistribuido evento)
                {
                    await _repositorio.RegistrarFalhaAsync(
                        mensagem.Id,
                        "Payload nulo ou incompativel com EventoDistribuido.",
                        cancellationToken);
                    continue;
                }

                await PublicarDinamicoAsync(evento, tipo, cancellationToken);
                await _repositorio.MarcarComoProcessadoAsync(mensagem.Id, cancellationToken);
                despachados++;
            }
            catch (Exception ex)
            {
                await _repositorio.RegistrarFalhaAsync(mensagem.Id, ex.Message, cancellationToken);
            }
        }

        return despachados;
    }

    private Task PublicarDinamicoAsync(EventoDistribuido evento, Type tipo, CancellationToken cancellationToken)
    {
        var metodo = typeof(IEventBus).GetMethod(nameof(IEventBus.PublicarAsync))
            ?.MakeGenericMethod(tipo);

        if (metodo == null)
        {
            throw new InvalidOperationException($"Metodo PublicarAsync nao localizado para {tipo.Name}.");
        }

        var tarefa = metodo.Invoke(_eventBus, new object[] { evento, cancellationToken }) as Task;
        return tarefa ?? Task.CompletedTask;
    }
}
