using System.Threading.Channels;
using Fase07.AltaPerformance.Core.Modelos;

namespace Fase07.AltaPerformance.Core.Pipeline;

public sealed record EstatisticasProcessamento(
    int TotalEventos,
    int TotalSucessos,
    int TotalErrosHttp,
    double DuracaoMediaMs);

public sealed class ProcessadorEventosChannel
{
    private readonly Channel<RegistroEvento> _canal;
    private readonly int _capacidade;

    public ProcessadorEventosChannel(int capacidade = 100)
    {
        _capacidade = capacidade;
        var opcoes = new BoundedChannelOptions(capacidade)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        _canal = Channel.CreateBounded<RegistroEvento>(opcoes);
    }

    public int Capacidade => _capacidade;
    public ChannelReader<RegistroEvento> Reader => _canal.Reader;
    public ChannelWriter<RegistroEvento> Writer => _canal.Writer;

    public async ValueTask<bool> PublicarAsync(RegistroEvento evento, CancellationToken ct = default)
    {
        while (await _canal.Writer.WaitToWriteAsync(ct).ConfigureAwait(false))
        {
            if (_canal.Writer.TryWrite(evento))
            {
                return true;
            }
        }
        return false;
    }

    public void ConcluirProducao()
    {
        _canal.Writer.Complete();
    }

    public async IAsyncEnumerable<RegistroEvento> LerTodosAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        while (await _canal.Reader.WaitToReadAsync(ct).ConfigureAwait(false))
        {
            while (_canal.Reader.TryRead(out var evento))
            {
                yield return evento;
            }
        }
    }

    public async Task<EstatisticasProcessamento> ConsumirEstatisticasAsync(CancellationToken ct = default)
    {
        int total = 0;
        int erros = 0;
        double somaDuracao = 0;

        await foreach (var evento in LerTodosAsync(ct).ConfigureAwait(false))
        {
            total++;
            if (evento.CodigoHttp >= 400)
            {
                erros++;
            }
            somaDuracao += evento.DuracaoMs;
        }

        double media = total > 0 ? somaDuracao / total : 0;
        int sucessos = total - erros;

        return new EstatisticasProcessamento(total, sucessos, erros, media);
    }
}
