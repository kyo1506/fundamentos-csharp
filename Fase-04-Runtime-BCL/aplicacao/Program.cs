using System.Text;
using System.Text.Json;
using Fase04.RelatoriosAsyncCli;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("====================================================================");
Console.WriteLine("  Fase 04: .NET Runtime & BCL: Relatórios Assíncronos e Concorrentes");
Console.WriteLine("====================================================================");
Console.WriteLine();

// 1. Preparação de dados de teste em memória
var lote1 = new List<ItemRelatorio>
{
    new(1, "C# Básico", "Ana Silva", 9.5m, new DateOnly(2026, 9, 15), "Aprovado"),
    new(2, "C# Básico", "Bruno Costa", 5.0m, new DateOnly(2026, 9, 16), "Reprovado"),
    new(3, "POO & SOLID", "Carla Dias", 8.8m, new DateOnly(2026, 9, 20), "Aprovado"),
};

var lote2 = new List<ItemRelatorio>
{
    new(4, "Coleções & LINQ", "Daniel Faria", 7.2m, new DateOnly(2026, 9, 25), "Aprovado"),
    new(5, "Runtime & BCL", "Eduardo Santos", 9.0m, new DateOnly(2026, 9, 28), "Aprovado"),
};

// 2. Serialização em memória para simular arquivos de entrada recebidos via Stream
using var streamLote1 = new MemoryStream();
using var streamLote2 = new MemoryStream();

await JsonSerializer.SerializeAsync(streamLote1, lote1, RelatorioJsonContext.Default.ListItemRelatorio);
await JsonSerializer.SerializeAsync(streamLote2, lote2, RelatorioJsonContext.Default.ListItemRelatorio);

streamLote1.Position = 0;
streamLote2.Position = 0;

Console.WriteLine("1. Processando lotes concorrentemente com SemaphoreSlim e Task.WhenAll...");

await using (var processador = new ProcessadorRelatoriosAsync(TimeProvider.System, maxConcorrencia: 2))
{
    using var streamSaidaConsolidada = new MemoryStream();
    var resumos = await processador.ProcessarMultiplosStreamsAsync(
        new[] { streamLote1, streamLote2 },
        streamSaidaConsolidada);

    Console.WriteLine($"   Total de lotes processados: {resumos.Count}");
    foreach (var r in resumos)
    {
        Console.WriteLine($"   - Itens: {r.TotalProcessado}, Média: {r.MediaGeral:F2}, Aprovados: {r.TotalAprovados}, Timestamp: {r.GeradoEmUtc:O}");
    }

    Console.WriteLine();
    Console.WriteLine("2. Exportando dados em formato CSV utilizando Reflection...");
    using var streamCsv = new MemoryStream();
    var todosItens = lote1.Concat(lote2);
    await processador.ExportarCsvAssincronoAsync(todosItens, streamCsv);

    streamCsv.Position = 0;
    using var leitorCsv = new StreamReader(streamCsv);
    Console.WriteLine(await leitorCsv.ReadToEndAsync());

    Console.WriteLine("3. Auditoria interna gravada em memória:");
    string auditoria = Encoding.UTF8.GetString(processador.ObterBytesAuditoria());
    Console.Write(auditoria);
}

Console.WriteLine();
Console.WriteLine("4. Testando cancelamento cooperativo com CancellationToken...");
try
{
    using var cts = new CancellationTokenSource();
    cts.Cancel(); // Cancela imediatamente antes da execução

    using var streamVazio = new MemoryStream();
    using var streamSaida = new MemoryStream();
    await using var procCancel = new ProcessadorRelatoriosAsync();

    await procCancel.ProcessarLoteJsonAsync(streamVazio, streamSaida, cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("   Cancelamento interceptado com sucesso: a operação respeitou o token.");
}

Console.WriteLine();
Console.WriteLine("Execução concluída com sucesso!");
