namespace Fase08.EventBus.Distribuido.Core.Barramento;

using Fase08.EventBus.Distribuido.Core.Modelos;

public interface IConsumidorEvento<in TEvento> where TEvento : EventoDistribuido
{
    Task ConsumirAsync(TEvento evento, CancellationToken cancellationToken = default);
}
