namespace Fase08.EventBus.Distribuido.Core.Idempotencia;

using Fase08.EventBus.Distribuido.Core.Barramento;
using Fase08.EventBus.Distribuido.Core.Modelos;

public sealed class ConsumidorIdempotenteDecorator<TEvento> : IConsumidorEvento<TEvento>
    where TEvento : EventoDistribuido
{
    private readonly IConsumidorEvento<TEvento> _interno;
    private readonly IStoreIdempotencia _store;
    private readonly Func<TEvento, string> _seletorChave;

    public ConsumidorIdempotenteDecorator(
        IConsumidorEvento<TEvento> interno,
        IStoreIdempotencia store,
        Func<TEvento, string>? seletorChave = null)
    {
        _interno = interno;
        _store = store;
        _seletorChave = seletorChave ?? (e => $"{typeof(TEvento).Name}:{e.EventId}");
    }

    public async Task ConsumirAsync(TEvento evento, CancellationToken cancellationToken = default)
    {
        var chave = _seletorChave(evento);
        var jaProcessado = await _store.JaProcessadoAsync(chave, cancellationToken);
        if (jaProcessado)
        {
            return;
        }

        await _interno.ConsumirAsync(evento, cancellationToken);
        await _store.RegistrarProcessamentoAsync(chave, cancellationToken: cancellationToken);
    }
}
