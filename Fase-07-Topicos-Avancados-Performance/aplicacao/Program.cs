using Fase07.AltaPerformance.Core.Modelos;
using Fase07.AltaPerformance.Core.Parsing;
using Fase07.AltaPerformance.Core.Pipeline;
using Fase07.AltaPerformance.Core.Validacao;

Console.WriteLine("=== Fase 07: Alta Performance e Pipelines Concorrentes ===");

string chave = "TRX-ABCD-1234";
bool chaveValida = ValidadorChaveTransacao.Validar(chave);
Console.WriteLine($"Validacao da chave '{chave}': {chaveValida}");

string textoParaSanitizar = "Payload seguro de auditoria";
bool contemInseguro = SanitizadorTexto.ContemCaracteresInseguros(textoParaSanitizar);
Console.WriteLine($"Contem caracteres inseguros: {contemInseguro}");

string linhaLog = "6ba7b810-9dad-11d1-80b4-00c04fd430c8|2026-10-04T10:30:00Z|Info|CatalogoService|Item consultado com sucesso|200|12.5";
if (SpanLogParser.TryParse(linhaLog, out var evento) && evento != null)
{
    Console.WriteLine($"Log analisado com sucesso: [{evento.Nivel}] {evento.Servico} : {evento.Mensagem} ({evento.DuracaoMs} ms)");

    var processador = new ProcessadorEventosChannel(capacidade: 50);

    var consumidorTask = Task.Run(async () =>
    {
        return await processador.ConsumirEstatisticasAsync();
    });

    await processador.PublicarAsync(evento);
    processador.ConcluirProducao();

    var stats = await consumidorTask;
    Console.WriteLine($"Processamento concluido: Total={stats.TotalEventos}, Sucesso={stats.TotalSucessos}, Erros={stats.TotalErrosHttp}");
}
else
{
    Console.WriteLine("Falha ao analisar linha de log.");
}
