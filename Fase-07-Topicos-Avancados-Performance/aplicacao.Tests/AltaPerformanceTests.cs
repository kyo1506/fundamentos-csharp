using Fase07.AltaPerformance.Core.Modelos;
using Fase07.AltaPerformance.Core.Parsing;
using Fase07.AltaPerformance.Core.Pipeline;
using Fase07.AltaPerformance.Core.Validacao;
using Xunit;

namespace Fase07.AltaPerformance.Tests;

public class AltaPerformanceTests
{
    [Fact]
    public void SpanLogParser_LinhaValida_DeveRetornarRegistroCompleto()
    {
        // Arrange
        var idEsperado = Guid.NewGuid();
        string linha = $"{idEsperado}|2026-10-04T14:30:00Z|Critico|AuthService|Falha irrecuperavel de conexao|503|128.75";

        // Act
        bool sucesso = SpanLogParser.TryParse(linha.AsSpan(), out var evento);

        // Assert
        Assert.True(sucesso);
        Assert.NotNull(evento);
        Assert.Equal(idEsperado, evento.Id);
        Assert.Equal(NivelLog.Critico, evento.Nivel);
        Assert.Equal("AuthService", evento.Servico);
        Assert.Equal("Falha irrecuperavel de conexao", evento.Mensagem);
        Assert.Equal(503, evento.CodigoHttp);
        Assert.Equal(128.75, evento.DuracaoMs);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalido")]
    [InlineData("not-a-guid|2026-10-04T14:30:00Z|Info|Service|Msg|200|10.0")]
    [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8|data-invalida|Info|Service|Msg|200|10.0")]
    [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8|2026-10-04T14:30:00Z|NivelInexistente|Service|Msg|200|10.0")]
    [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8|2026-10-04T14:30:00Z|Info|Service|Msg|nao-numero|10.0")]
    [InlineData("6ba7b810-9dad-11d1-80b4-00c04fd430c8|2026-10-04T14:30:00Z|Info|Service|Msg|200|nao-double")]
    public void SpanLogParser_LinhaInvalida_DeveRetornarFalso(string linhaInvalida)
    {
        // Act
        bool sucesso = SpanLogParser.TryParse(linhaInvalida.AsSpan(), out var evento);

        // Assert
        Assert.False(sucesso);
        Assert.Null(evento);
    }

    [Fact]
    public void LogTokenizer_MultiplosTokens_DeveIterarCorretamente()
    {
        // Arrange
        string dados = "alfa|beta|gama";
        var tokenizer = new LogTokenizer(dados.AsSpan(), '|');

        // Act & Assert
        Assert.True(tokenizer.TryGetNext(out var t1));
        Assert.Equal("alfa", t1.ToString());

        Assert.True(tokenizer.TryGetNext(out var t2));
        Assert.Equal("beta", t2.ToString());

        Assert.True(tokenizer.TryGetNext(out var t3));
        Assert.Equal("gama", t3.ToString());

        Assert.False(tokenizer.TryGetNext(out _));
    }

    [Theory]
    [InlineData("TRX-ABCD-1234", true)]
    [InlineData("TRX-1A2B-9876", true)]
    [InlineData("TRX-XXXX-0000", true)]
    [InlineData("trx-abcd-1234", false)] // minusculo
    [InlineData("TRX-ABC-1234", false)]  // 3 chars
    [InlineData("TRX-ABCDE-1234", false)] // 5 chars
    [InlineData("TRX-ABCD-123", false)]  // 3 digitos
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ValidadorChaveTransacao_DeveValidarFormatoComPrecisao(string? chave, bool resultadoEsperado)
    {
        // Act
        bool valido = ValidadorChaveTransacao.Validar(chave);

        // Assert
        Assert.Equal(resultadoEsperado, valido);
    }

    [Fact]
    public void SanitizadorTexto_ComCaracteresInseguros_DeveIdentificarCorretamente()
    {
        // Arrange
        string textoSeguro = "Requisicao normal de telemetria 123";
        string textoInseguro = "Script <alert> perigoso & manipulado";

        // Act & Assert
        Assert.False(SanitizadorTexto.ContemCaracteresInseguros(textoSeguro.AsSpan()));
        Assert.True(SanitizadorTexto.ContemCaracteresInseguros(textoInseguro.AsSpan()));
        Assert.Equal(7, SanitizadorTexto.IndicePrimeiroCaractereInseguro(textoInseguro.AsSpan()));
    }

    [Fact]
    public async Task ProcessadorEventosChannel_PipelineConcorrente_DeveProcessarComSucesso()
    {
        // Arrange
        const int totalItens = 200;
        var processador = new ProcessadorEventosChannel(capacidade: 20);

        // Act: Consumidor em background
        var tarefaConsumo = Task.Run(async () =>
        {
            return await processador.ConsumirEstatisticasAsync();
        });

        // Act: Produtor enviando eventos
        for (int i = 1; i <= totalItens; i++)
        {
            int status = i % 5 == 0 ? 500 : 200;
            var evento = new RegistroEvento(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow,
                status >= 400 ? NivelLog.Erro : NivelLog.Info,
                "CheckoutApi",
                $"Processamento de item {i}",
                status,
                DuracaoMs: 10.0);

            await processador.PublicarAsync(evento);
        }

        processador.ConcluirProducao();
        var estatisticas = await tarefaConsumo;

        // Assert
        Assert.Equal(totalItens, estatisticas.TotalEventos);
        Assert.Equal(40, estatisticas.TotalErrosHttp); // 200 / 5 = 40
        Assert.Equal(160, estatisticas.TotalSucessos);
        Assert.Equal(10.0, estatisticas.DuracaoMediaMs);
    }
}
