using Fase01.CSharpBasico;
using Xunit;

namespace Fase01.CSharpBasico.Tests;

public class BasicoTests
{
    // ==========================================
    // 01.1 & 01.6: Struct Dinheiro e Imutabilidade
    // ==========================================
    [Fact]
    public void Dinheiro_SomaValores_MesmaMoeda_RetornaCorreto()
    {
        var d1 = new Dinheiro(100.50m, "BRL");
        var d2 = new Dinheiro(50.25m, "BRL");
        var soma = d1 + d2;

        Assert.Equal(150.75m, soma.Valor);
        Assert.Equal("BRL", soma.Moeda);
    }

    [Fact]
    public void Dinheiro_SubtracaoComSaldoSuficiente_RetornaCorreto()
    {
        var d1 = new Dinheiro(100.00m, "BRL");
        var d2 = new Dinheiro(40.00m, "BRL");
        var sub = d1 - d2;

        Assert.Equal(60.00m, sub.Valor);
    }

    [Fact]
    public void Dinheiro_SubtracaoComSaldoInsuficiente_LancaSaldoInsuficienteException()
    {
        var d1 = new Dinheiro(50.00m, "BRL");
        var d2 = new Dinheiro(80.00m, "BRL");

        var ex = Assert.Throws<SaldoInsuficienteException>(() => _ = d1 - d2);
        Assert.Equal(50.00m, ex.SaldoAtual);
        Assert.Equal(80.00m, ex.ValorTentado);
    }

    [Fact]
    public void Dinheiro_MoedasDiferentes_LancaInvalidOperationException()
    {
        var d1 = new Dinheiro(100m, "BRL");
        var d2 = new Dinheiro(100m, "USD");

        Assert.Throws<InvalidOperationException>(() => _ = d1 + d2);
    }

    [Fact]
    public void Dinheiro_MultiplicacaoPorFator_ArredondaComercial()
    {
        var d = new Dinheiro(10.55m, "BRL");
        var resultado = d * 1.5m; // 15.825 -> 15.83m

        Assert.Equal(15.83m, resultado.Valor);
    }

    // ==========================================
    // 01.2: Strings e StringBuilder
    // ==========================================
    [Theory]
    [InlineData("amor", "roma")]
    [InlineData("C#", "#C")]
    [InlineData("radar", "radar")]
    [InlineData("", "")]
    public void InverterTexto_RetornaStringInvertida(string entrada, string esperado)
    {
        string resultado = Basico.InverterTexto(entrada);
        Assert.Equal(esperado, resultado);
    }

    // ==========================================
    // 01.3: Números e Parcelamento Exato
    // ==========================================
    [Theory]
    [InlineData(100.00, 3, 33.34, 33.33, 33.33)]
    [InlineData(50.00, 2, 25.00, 25.00, 0)]
    public void DividirParcelas_GaranteSomaExata(double totalDouble, int qtdParcelas, double p1Double, double p2Double, double p3Double)
    {
        decimal total = (decimal)totalDouble;
        var parcelas = Basico.DividirParcelas(total, qtdParcelas);

        Assert.Equal(qtdParcelas, parcelas.Count);
        Assert.Equal(total, parcelas.Sum());
        Assert.Equal((decimal)p1Double, parcelas[0]);
        Assert.Equal((decimal)p2Double, parcelas[1]);
        if (qtdParcelas == 3)
            Assert.Equal((decimal)p3Double, parcelas[2]);
    }

    [Fact]
    public void DividirParcelas_ParcelasZeroOuNegativas_LancaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Basico.DividirParcelas(100m, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Basico.DividirParcelas(100m, -1));
    }

    // ==========================================
    // 01.4: Coleções e Validador de Delimitadores
    // ==========================================
    [Fact]
    public void ContarFrequenciaPalavras_IgnoraCaseEPontuacao()
    {
        string texto = "C#, c# e C#! dotnet é rápido, muito rápido.";
        var contagem = Basico.ContarFrequenciaPalavras(texto);

        Assert.Equal(3, contagem["c#"]);
        Assert.Equal(2, contagem["rápido"]);
        Assert.Equal(1, contagem["dotnet"]);
    }

    [Theory]
    [InlineData("()", true)]
    [InlineData("([{}])", true)]
    [InlineData("({[]})", true)]
    [InlineData("({[)]}", false)]
    [InlineData("(((", false)]
    [InlineData(")))", false)]
    [InlineData("", true)]
    public void ValidarDelimitadores_IdentificaBalanceamento(string expressao, bool esperado)
    {
        bool resultado = Basico.ValidarDelimitadores(expressao);
        Assert.Equal(esperado, resultado);
    }

    // ==========================================
    // 01.7: Pattern Matching e Descontos
    // ==========================================
    [Fact]
    public void CalcularPercentualDesconto_ClienteVipEAltoValor_Retorna25Porcento()
    {
        var pedido = new Pedido(1, 1500m, "BR", ClienteVip: true, QuantidadeItens: 2);
        decimal desconto = Basico.CalcularPercentualDesconto(pedido);

        Assert.Equal(0.25m, desconto);
    }

    [Fact]
    public void CalcularPercentualDesconto_ClienteVipApenas_Retorna15Porcento()
    {
        var pedido = new Pedido(2, 300m, "US", ClienteVip: true, QuantidadeItens: 1);
        decimal desconto = Basico.CalcularPercentualDesconto(pedido);

        Assert.Equal(0.15m, desconto);
    }

    [Fact]
    public void CalcularPercentualDesconto_NaoVipMasPaisBRCincoItens_Retorna10Porcento()
    {
        var pedido = new Pedido(3, 200m, "BR", ClienteVip: false, QuantidadeItens: 5);
        decimal desconto = Basico.CalcularPercentualDesconto(pedido);

        Assert.Equal(0.10m, desconto);
    }

    [Fact]
    public void CalcularPercentualDesconto_PedidoSemRegrasEspecificas_RetornaZero()
    {
        var pedido = new Pedido(4, 100m, "AR", ClienteVip: false, QuantidadeItens: 1);
        decimal desconto = Basico.CalcularPercentualDesconto(pedido);

        Assert.Equal(0.00m, desconto);
    }

    // ==========================================
    // 01.9: Métodos de Extensão
    // ==========================================
    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(0, true)]
    [InlineData(-4, true)]
    public void EhPar_MetodoDeExtensao_RetornaEsperado(int numero, bool esperado)
    {
        Assert.Equal(esperado, numero.EhPar());
    }

    [Fact]
    public void Truncar_StringMaiorQueLimite_AplicaSufixo()
    {
        string texto = "Desenvolvimento de Software";
        string resultado = texto.Truncar(15);

        Assert.Equal("Desenvolvimento...", resultado);
    }

    [Fact]
    public void Truncar_StringMenorQueLimite_RetornaIntacta()
    {
        string texto = "Olá";
        Assert.Equal("Olá", texto.Truncar(10));
    }

    // ==========================================
    // 01.10: Datas e Dias Úteis
    // ==========================================
    [Fact]
    public void CalcularDiasUteis_SemanaCompleta_RetornaCincoDias()
    {
        // Segunda-feira (14/09/2026) a Domingo (20/09/2026) -> 5 dias úteis (Seg a Sex)
        var segunda = new DateOnly(2026, 9, 14);
        var domingo = new DateOnly(2026, 9, 20);

        int dias = Basico.CalcularDiasUteis(segunda, domingo);
        Assert.Equal(5, dias);
    }

    [Fact]
    public void CalcularDiasUteis_FinalDeSemana_RetornaZero()
    {
        var sabado = new DateOnly(2026, 9, 19);
        var domingo = new DateOnly(2026, 9, 20);

        int dias = Basico.CalcularDiasUteis(sabado, domingo);
        Assert.Equal(0, dias);
    }
}
