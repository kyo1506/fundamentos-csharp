using System.Text;
using System.Text.Json;
using Xunit;

namespace Fase04.RelatoriosAsyncCli.Tests;

/// <summary>
/// Provedor de tempo controlado para testes unitários determinísticos.
/// </summary>
public sealed class FakeTimeProvider : TimeProvider
{
    private DateTimeOffset _horaAtual;

    public FakeTimeProvider(DateTimeOffset horaInicial)
    {
        _horaAtual = horaInicial;
    }

    public override DateTimeOffset GetUtcNow() => _horaAtual;

    public void Avancar(TimeSpan duracao)
    {
        _horaAtual = _horaAtual.Add(duracao);
    }
}

public class RelatoriosAsyncCliTests
{
    [Fact]
    public async Task ProcessarLoteJsonAsync_DeveCalcularMetricas_ESerializarResumoCorretamente()
    {
        // Arrange
        var horaFixa = new DateTimeOffset(2026, 10, 4, 12, 0, 0, TimeSpan.Zero);
        var fakeTime = new FakeTimeProvider(horaFixa);

        var itens = new List<ItemRelatorio>
        {
            new(1, "C#", "Alice", 10.0m, new DateOnly(2026, 10, 1), "Aprovado"),
            new(2, "C#", "Bob", 6.0m, new DateOnly(2026, 10, 2), "Aprovado"),
            new(3, "C#", "Carol", 2.0m, new DateOnly(2026, 10, 3), "Reprovado"),
        };

        using var streamEntrada = new MemoryStream();
        await JsonSerializer.SerializeAsync(streamEntrada, itens, RelatorioJsonContext.Default.ListItemRelatorio);
        streamEntrada.Position = 0;

        using var streamSaida = new MemoryStream();
        await using var processador = new ProcessadorRelatoriosAsync(fakeTime, maxConcorrencia: 2);

        // Act
        var resumo = await processador.ProcessarLoteJsonAsync(streamEntrada, streamSaida);

        // Assert
        Assert.Equal(3, resumo.TotalProcessado);
        Assert.Equal(6.0m, resumo.MediaGeral); // (10 + 6 + 2) / 3 = 6.0
        Assert.Equal(2, resumo.TotalAprovados);
        Assert.Equal(horaFixa, resumo.GeradoEmUtc);

        // Verifica o stream de saída
        streamSaida.Position = 0;
        var resumoDeserializado = await JsonSerializer.DeserializeAsync(
            streamSaida,
            RelatorioJsonContext.Default.ResumoRelatorio);

        Assert.NotNull(resumoDeserializado);
        Assert.Equal(3, resumoDeserializado.TotalProcessado);
        Assert.Equal(6.0m, resumoDeserializado.MediaGeral);
    }

    [Fact]
    public async Task ProcessarMultiplosStreamsAsync_DeveExecutarLotesConcorrentes()
    {
        // Arrange
        var lote1 = new List<ItemRelatorio> { new(1, "NET", "Dev 1", 8.0m, new DateOnly(2026, 10, 1), "Aprovado") };
        var lote2 = new List<ItemRelatorio> { new(2, "NET", "Dev 2", 4.0m, new DateOnly(2026, 10, 2), "Reprovado") };

        using var s1 = new MemoryStream();
        using var s2 = new MemoryStream();
        await JsonSerializer.SerializeAsync(s1, lote1, RelatorioJsonContext.Default.ListItemRelatorio);
        await JsonSerializer.SerializeAsync(s2, lote2, RelatorioJsonContext.Default.ListItemRelatorio);
        s1.Position = 0;
        s2.Position = 0;

        using var streamConsolidado = new MemoryStream();
        await using var processador = new ProcessadorRelatoriosAsync(TimeProvider.System, maxConcorrencia: 2);

        // Act
        var resumos = await processador.ProcessarMultiplosStreamsAsync(new[] { s1, s2 }, streamConsolidado);

        // Assert
        Assert.Equal(2, resumos.Count);
        Assert.Equal(1, resumos[0].TotalProcessado);
        Assert.Equal(1, resumos[1].TotalProcessado);

        streamConsolidado.Position = 0;
        var listaConsolidada = await JsonSerializer.DeserializeAsync(
            streamConsolidado,
            RelatorioJsonContext.Default.ListResumoRelatorio);

        Assert.NotNull(listaConsolidada);
        Assert.Equal(2, listaConsolidada.Count);
    }

    [Fact]
    public async Task ProcessarLoteJsonAsync_DeveRespeitarCancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Token já cancelado

        using var streamEntrada = new MemoryStream();
        using var streamSaida = new MemoryStream();
        await using var processador = new ProcessadorRelatoriosAsync();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await processador.ProcessarLoteJsonAsync(streamEntrada, streamSaida, cts.Token);
        });
    }

    [Fact]
    public async Task ExportarCsvAssincronoAsync_DeveUsarReflectionEFormatarInvariante()
    {
        // Arrange
        var itens = new List<ItemRelatorio>
        {
            new(10, "Arquitetura", "Renata", 9.75m, new DateOnly(2026, 10, 4), "Aprovado")
        };

        using var streamCsv = new MemoryStream();
        await using var processador = new ProcessadorRelatoriosAsync();

        // Act
        await processador.ExportarCsvAssincronoAsync(itens, streamCsv);

        // Assert
        streamCsv.Position = 0;
        using var leitor = new StreamReader(streamCsv, Encoding.UTF8);
        string conteudo = await leitor.ReadToEndAsync();
        string[] linhas = conteudo.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(2, linhas.Length);
        Assert.Equal("ID;Curso;Estudante;Nota Final;Conclusão;Status", linhas[0]);
        Assert.Equal("10;Arquitetura;Renata;9.75;2026-10-04;Aprovado", linhas[1]);
    }

    [Fact]
    public void ObterCabecalhoCsvReflectivo_DeveOrdenarPorPropriedadeOrdemDoAtributo()
    {
        // Act
        string cabecalho = ProcessadorRelatoriosAsync.ObterCabecalhoCsvReflectivo();

        // Assert
        Assert.Equal("ID;Curso;Estudante;Nota Final;Conclusão;Status", cabecalho);
    }

    [Fact]
    public async Task Dispose_EDisposeAsync_DevemBloquearChamadasSubsequentes()
    {
        // Arrange
        var processador = new ProcessadorRelatoriosAsync();
        using var sIn = new MemoryStream();
        using var sOut = new MemoryStream();

        // Act
        await processador.DisposeAsync();

        // Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
        {
            await processador.ProcessarLoteJsonAsync(sIn, sOut);
        });

        Assert.Throws<ObjectDisposedException>(() =>
        {
            processador.ObterBytesAuditoria();
        });
    }

    [Fact]
    public async Task AuditoriaEmMemoria_DeveRegistrarLogsCorretamente()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 10, 4, 15, 0, 0, TimeSpan.Zero));
        var itens = new List<ItemRelatorio>
        {
            new(1, "Testes", "Lucas", 8.0m, new DateOnly(2026, 10, 4), "Aprovado")
        };

        using var sIn = new MemoryStream();
        await JsonSerializer.SerializeAsync(sIn, itens, RelatorioJsonContext.Default.ListItemRelatorio);
        sIn.Position = 0;

        using var sOut = new MemoryStream();
        await using var processador = new ProcessadorRelatoriosAsync(fakeTime);

        // Act
        await processador.ProcessarLoteJsonAsync(sIn, sOut);
        byte[] auditBytes = processador.ObterBytesAuditoria();
        string auditLog = Encoding.UTF8.GetString(auditBytes);

        // Assert
        Assert.Contains("[AUDIT 2026-10-04T15:00:00.0000000+00:00]", auditLog);
        Assert.Contains("Lote de 1 itens processado", auditLog);
    }
}
