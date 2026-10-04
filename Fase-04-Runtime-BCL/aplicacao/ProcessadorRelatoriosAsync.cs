using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Fase04.RelatoriosAsyncCli;

/// <summary>
/// Motor de processamento assíncrono de relatórios acadêmicos.
/// Demonstra Streams, System.Text.Json com Source Generator, Task/async/await,
/// CancellationToken, SemaphoreSlim, TimeProvider e IAsyncDisposable.
/// </summary>
public sealed class ProcessadorRelatoriosAsync : IDisposable, IAsyncDisposable
{
    private readonly TimeProvider _timeProvider;
    private readonly SemaphoreSlim _semaforo;
    private readonly MemoryStream _bufferAuditoria;
    private bool _descartado;

    public ProcessadorRelatoriosAsync(TimeProvider? timeProvider = null, int maxConcorrencia = 3)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
        _semaforo = new SemaphoreSlim(maxConcorrencia, maxConcorrencia);
        _bufferAuditoria = new MemoryStream();
    }

    /// <summary>
    /// Processa um stream JSON contendo uma lista de ItemRelatorio, calcula métricas,
    /// serializa o ResumoRelatorio gerado no stream de saída e registra auditoria.
    /// </summary>
    public async Task<ResumoRelatorio> ProcessarLoteJsonAsync(
        Stream streamEntrada,
        Stream streamSaidaJson,
        CancellationToken cancellationToken = default)
    {
        VerificarDescarte();
        cancellationToken.ThrowIfCancellationRequested();

        long timestampInicio = _timeProvider.GetTimestamp();

        // Desserializa assincronamente a partir do stream usando o Source Generator
        List<ItemRelatorio>? itens = await JsonSerializer.DeserializeAsync(
            streamEntrada,
            RelatorioJsonContext.Default.ListItemRelatorio,
            cancellationToken);

        itens ??= [];

        cancellationToken.ThrowIfCancellationRequested();

        int total = itens.Count;
        decimal media = total > 0 ? itens.Average(i => i.Nota) : 0m;
        int aprovados = itens.Count(i => i.Nota >= 6.0m);

        long timestampFim = _timeProvider.GetTimestamp();
        TimeSpan decorrido = _timeProvider.GetElapsedTime(timestampInicio, timestampFim);

        var resumo = new ResumoRelatorio(
            TotalProcessado: total,
            MediaGeral: Math.Round(media, 2),
            TotalAprovados: aprovados,
            GeradoEmUtc: _timeProvider.GetUtcNow(),
            DuracaoMs: (long)decorrido.TotalMilliseconds
        );

        // Serializa o resumo para o stream de saída usando Source Generator
        await JsonSerializer.SerializeAsync(
            streamSaidaJson,
            resumo,
            RelatorioJsonContext.Default.ResumoRelatorio,
            cancellationToken);

        await streamSaidaJson.FlushAsync(cancellationToken);

        // Registra evento no buffer em memória de auditoria
        byte[] bytesLog = Encoding.UTF8.GetBytes(
            $"[AUDIT {_timeProvider.GetUtcNow():O}] Lote de {total} itens processado em {decorrido.TotalMilliseconds:F1}ms.\n");
        await _bufferAuditoria.WriteAsync(bytesLog, cancellationToken);

        return resumo;
    }

    /// <summary>
    /// Processa múltiplos streams concorrentemente limitados por SemaphoreSlim e agrega os resumos.
    /// </summary>
    public async Task<IReadOnlyList<ResumoRelatorio>> ProcessarMultiplosStreamsAsync(
        IEnumerable<Stream> fluxosEntrada,
        Stream fluxoSaidaConsolidado,
        CancellationToken cancellationToken = default)
    {
        VerificarDescarte();

        var tarefas = fluxosEntrada.Select(async fluxo =>
        {
            await _semaforo.WaitAsync(cancellationToken);
            try
            {
                using var saidaTemporaria = new MemoryStream();
                return await ProcessarLoteJsonAsync(fluxo, saidaTemporaria, cancellationToken);
            }
            finally
            {
                _semaforo.Release();
            }
        });

        ResumoRelatorio[] resumos = await Task.WhenAll(tarefas);

        // Grava consolidação final no stream de saída
        await JsonSerializer.SerializeAsync(
            fluxoSaidaConsolidado,
            resumos.ToList(),
            RelatorioJsonContext.Default.ListResumoRelatorio,
            cancellationToken);

        await fluxoSaidaConsolidado.FlushAsync(cancellationToken);

        return resumos;
    }

    /// <summary>
    /// Exporta os itens para formato CSV em stream utilizando Reflection para ler os cabeçalhos.
    /// </summary>
    public async Task ExportarCsvAssincronoAsync(
        IEnumerable<ItemRelatorio> itens,
        Stream streamSaida,
        CancellationToken cancellationToken = default)
    {
        VerificarDescarte();

        await using var escritor = new StreamWriter(streamSaida, Encoding.UTF8, leaveOpen: true);

        // Cabeçalho gerado via Reflection
        string cabecalho = ObterCabecalhoCsvReflectivo();
        await escritor.WriteLineAsync(cabecalho.AsMemory(), cancellationToken);

        foreach (var item in itens)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string linha = string.Join(";",
                item.Id.ToString(CultureInfo.InvariantCulture),
                item.TituloCurso,
                item.NomeAluno,
                item.Nota.ToString("F2", CultureInfo.InvariantCulture),
                item.DataConclusao.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                item.Status);

            await escritor.WriteLineAsync(linha.AsMemory(), cancellationToken);
        }

        await escritor.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Utiliza Reflection para descobrir as propriedades decoradas com ColunaExportacaoAttribute
    /// e gerar o cabeçalho CSV na ordem configurada.
    /// </summary>
    public static string ObterCabecalhoCsvReflectivo()
    {
        var colunas = typeof(ItemRelatorio)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new
            {
                Prop = p,
                Attr = p.GetCustomAttribute<ColunaExportacaoAttribute>()
            })
            .Where(x => x.Attr != null)
            .OrderBy(x => x.Attr!.Ordem)
            .Select(x => x.Attr!.NomeCabecalho);

        return string.Join(";", colunas);
    }

    /// <summary>
    /// Obtém os bytes do buffer de auditoria acumulados em memória.
    /// </summary>
    public byte[] ObterBytesAuditoria()
    {
        VerificarDescarte();
        return _bufferAuditoria.ToArray();
    }

    private void VerificarDescarte()
    {
        ObjectDisposedException.ThrowIf(_descartado, this);
    }

    public void Dispose()
    {
        if (_descartado) return;
        _semaforo.Dispose();
        _bufferAuditoria.Dispose();
        _descartado = true;
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_descartado) return;
        _semaforo.Dispose();
        await _bufferAuditoria.DisposeAsync();
        _descartado = true;
        GC.SuppressFinalize(this);
    }
}
