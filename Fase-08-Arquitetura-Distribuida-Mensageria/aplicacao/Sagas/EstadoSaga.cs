namespace Fase08.EventBus.Distribuido.Core.Sagas;

public enum EstadoSaga
{
    Iniciada = 1,
    AguardandoPagamento = 2,
    Concluida = 3,
    Compensando = 4,
    FalhaCompensada = 5
}
