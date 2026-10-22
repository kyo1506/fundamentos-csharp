namespace Fase08.EventBus.Distribuido.Core.Barramento;

using Fase08.EventBus.Distribuido.Core.Modelos;

public interface IEventBus
{
    Task PublicarAsync<TEvento>(TEvento evento, CancellationToken cancellationToken = default) where TEvento : EventoDistribuido;
    void Inscrever<TEvento>(IConsumidorEvento<TEvento> consumidor) where TEvento : EventoDistribuido;
}
