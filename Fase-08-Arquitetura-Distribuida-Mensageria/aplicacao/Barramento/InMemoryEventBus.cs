namespace Fase08.EventBus.Distribuido.Core.Barramento;

using System.Collections.Concurrent;
using Fase08.EventBus.Distribuido.Core.Modelos;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<object>> _inscricoes = new();

    public void Inscrever<TEvento>(IConsumidorEvento<TEvento> consumidor) where TEvento : EventoDistribuido
    {
        var tipo = typeof(TEvento);
        var lista = _inscricoes.GetOrAdd(tipo, _ => new List<object>());
        lock (lista)
        {
            lista.Add(consumidor);
        }
    }

    public async Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancellationToken = default) where TEvento : EventoDistribuido
    {
        var tipo = typeof(TEvento);
        if (!_inscricoes.TryGetValue(tipo, out var lista))
        {
            return;
        }

        List<object> copiaConsumidores;
        lock (lista)
        {
            copiaConsumidores = new List<object>(lista);
        }

        foreach (var handler in copiaConsumidores)
        {
            if (handler is IConsumidorEvento<TEvento> consumidor)
            {
                await consumidor.ConsumirAsync(evento, cancellationToken);
            }
        }
    }
}
