using Semana01.ControleEstoque;

namespace Semana01.Tests;

public class DinheiroTests
{
    [Fact]
    public void Dinheiro_CriarValorValido_DeveArmazenarValor()
    {
        var dinheiro = new Dinheiro(15.90m);
        
        Assert.Equal(15.90m, dinheiro.Valor);
        Assert.Equal("BRL", dinheiro.Moeda);
    }

    [Fact]
    public void Dinheiro_CriarValorNegativo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Dinheiro(-10.0m));
    }

    [Fact]
    public void Dinheiro_MoedaInvalida_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Dinheiro(10.0m, "JPY"));
    }

    [Fact]
    public void Dinheiro_SomarMesmaMoeda_DeveRetornarSoma()
    {
        var a = new Dinheiro(10.0m);
        var b = new Dinheiro(5.0m);
        
        var resultado = a + b;
        
        Assert.Equal(15.0m, resultado.Valor);
    }

    [Fact]
    public void Dinheiro_SomarMoedasDiferentes_DeveLancarExcecao()
    {
        var a = new Dinheiro(10.0m, "BRL");
        var b = new Dinheiro(5.0m, "USD");
        
        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void Dinheiro_MultiplicarPorQuantidade_DeveRetornarTotal()
    {
        var preco = new Dinheiro(4.50m);
        
        var total = preco * 3;
        
        Assert.Equal(13.50m, total.Valor);
    }

    [Fact]
    public void Dinheiro_ToString_DeveFormatarCorretamente()
    {
        var dinheiro = new Dinheiro(15.90m);
        
        Assert.Contains("15.90", dinheiro.ToString());
    }
}

public class ProdutoPerecivelTests
{
    [Fact]
    public void Produto_CriarStruct_DeveTerValorPadrao()
    {
        var produto = new ProdutoPerecivel();
        
        Assert.Null(produto.Nome); // string em struct é null por padrão
        Assert.Equal(0, produto.QuantidadeEstoque);
    }

    [Fact]
    public void Produto_Copia_DeveSerIndependente()
    {
        var original = new ProdutoPerecivel
        {
            Nome = "Arroz",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };

        var copia = original;
        copia.Preco = new Dinheiro(20.00m);
        copia.QuantidadeEstoque = 50;

        Assert.Equal(15.90m, original.Preco.Valor);
        Assert.Equal(100, original.QuantidadeEstoque);
        Assert.Equal(20.00m, copia.Preco.Valor);
        Assert.Equal(50, copia.QuantidadeEstoque);
    }

    [Fact]
    public void Produto_Vencido_DeveRetornarTrue()
    {
        var produto = new ProdutoPerecivel
        {
            DataValidade = DateTime.Now.AddDays(-1)
        };

        Assert.True(produto.EstaVencido);
    }

    [Fact]
    public void Produto_NaoVencido_DeveRetornarFalse()
    {
        var produto = new ProdutoPerecivel
        {
            DataValidade = DateTime.Now.AddDays(30)
        };

        Assert.False(produto.EstaVencido);
    }
}

public class EstoqueTests
{
    [Fact]
    public void Estoque_Criar_DeveTerNome()
    {
        var estoque = new Estoque("Mercadinho");
        
        Assert.Equal("Mercadinho", estoque.Nome);
        Assert.Empty(estoque.Produtos);
    }

    [Fact]
    public void Estoque_AdicionarProduto_DeveAdicionar()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };

        estoque.AdicionarProduto(produto);

        Assert.Single(estoque.Produtos);
    }

    [Fact]
    public void Estoque_AdicionarProdutoExistente_DeveSomarQuantidade()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };

        estoque.AdicionarProduto(produto);
        estoque.AdicionarProduto(produto);

        Assert.Single(estoque.Produtos);
        Assert.Equal(200, estoque.Produtos[0].QuantidadeEstoque);
    }

    [Fact]
    public void Estoque_BuscarPorCodigo_DeveRetornarProduto()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };
        estoque.AdicionarProduto(produto);

        var encontrado = estoque.BuscarPorCodigo("123");

        Assert.NotNull(encontrado);
        Assert.Equal("Arroz", encontrado.Value.Nome);
    }

    [Fact]
    public void Estoque_BuscarPorCodigoInvalido_DeveRetornarNull()
    {
        var estoque = new Estoque("Test");

        var encontrado = estoque.BuscarPorCodigo("999");

        Assert.Null(encontrado);
    }

    [Fact]
    public void Estoque_RegistrarEntrada_DeveAumentarEstoque()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };
        estoque.AdicionarProduto(produto);

        bool resultado = estoque.RegistrarEntrada("123", 50);

        Assert.True(resultado);
        Assert.Equal(150, estoque.Produtos[0].QuantidadeEstoque);
        Assert.Single(estoque.Historico);
    }

    [Fact]
    public void Estoque_RegistrarSaida_DeveDiminuirEstoque()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };
        estoque.AdicionarProduto(produto);

        bool resultado = estoque.RegistrarSaida("123", 30);

        Assert.True(resultado);
        Assert.Equal(70, estoque.Produtos[0].QuantidadeEstoque);
    }

    [Fact]
    public void Estoque_RegistrarSaidaInsuficiente_DeveRetornarFalse()
    {
        var estoque = new Estoque("Test");
        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 10
        };
        estoque.AdicionarProduto(produto);

        bool resultado = estoque.RegistrarSaida("123", 50);

        Assert.False(resultado);
        Assert.Equal(10, estoque.Produtos[0].QuantidadeEstoque);
    }

    [Fact]
    public void Estoque_AjustarPrecosComRef_DeveModificarOriginal()
    {
        var estoque = new Estoque("Test");
        var produtos = new[]
        {
            new ProdutoPerecivel { Nome = "Arroz", Preco = new Dinheiro(10.00m) },
            new ProdutoPerecivel { Nome = "Feijão", Preco = new Dinheiro(20.00m) }
        };

        estoque.AjustarPrecos(ref produtos, 10.0m);

        Assert.Equal(11.00m, produtos[0].Preco.Valor);
        Assert.Equal(22.00m, produtos[1].Preco.Valor);
    }

    [Fact]
    public void Estoque_TentarAjustarPrecosComRelatorio_DeveRetornarRelatorio()
    {
        var estoque = new Estoque("Test");
        var produtos = new[]
        {
            new ProdutoPerecivel { Nome = "Arroz", Preco = new Dinheiro(10.00m) }
        };

        bool sucesso = estoque.TentarAjustarPrecosComRelatorio(ref produtos, 10.0m, out string relatorio);

        Assert.True(sucesso);
        Assert.NotNull(relatorio);
        Assert.Contains("Arroz", relatorio);
    }

    [Fact]
    public void Estoque_TentarAjustarPrecosArrayVazio_DeveRetornarFalse()
    {
        var estoque = new Estoque("Test");
        var produtos = Array.Empty<ProdutoPerecivel>();

        bool sucesso = estoque.TentarAjustarPrecosComRelatorio(ref produtos, 10.0m, out string relatorio);

        Assert.False(sucesso);
        Assert.Contains("Nenhum produto", relatorio);
    }

    [Fact]
    public void Estoque_ReferenciaCompartilhada_DeveAfectarAmbas()
    {
        var estoque1 = new Estoque("Test");
        Estoque estoque2 = estoque1; // cópia da referência

        var produto = new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        };
        estoque2.AdicionarProduto(produto);

        Assert.Single(estoque1.Produtos); // Ambas apontam para o mesmo objeto
        Assert.Single(estoque2.Produtos);
    }
}

public class TransacaoTests
{
    [Fact]
    public void Transacao_CriarEntrada_DeveTerTipoEntrada()
    {
        var produto = new ProdutoPerecivel { Nome = "Arroz" };
        var transacao = new Transacao("Entrada", 10, produto);

        Assert.Equal("Entrada", transacao.Tipo);
        Assert.Equal(10, transacao.Quantidade);
    }

    [Fact]
    public void Transacao_CriarSaida_DeveTerTipoSaida()
    {
        var produto = new ProdutoPerecivel { Nome = "Arroz" };
        var transacao = new Transacao("Saída", 5, produto);

        Assert.Equal("Saída", transacao.Tipo);
    }

    [Fact]
    public void Transacao_TipoInvalido_DeveLancarExcecao()
    {
        var produto = new ProdutoPerecivel { Nome = "Arroz" };
        Assert.Throws<ArgumentException>(() => new Transacao("Inválido", 10, produto));
    }

    [Fact]
    public void Transacao_QuantidadeNegativa_DeveLancarExcecao()
    {
        var produto = new ProdutoPerecivel { Nome = "Arroz" };
        Assert.Throws<ArgumentException>(() => new Transacao("Entrada", -5, produto));
    }

    [Fact]
    public void Transacao_Id_DeveSerUnico()
    {
        var produto = new ProdutoPerecivel { Nome = "Arroz" };
        var t1 = new Transacao("Entrada", 10, produto);
        var t2 = new Transacao("Entrada", 10, produto);

        Assert.NotEqual(t1.Id, t2.Id);
    }
}
