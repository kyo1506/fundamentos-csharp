using Semana03.AnaliseDados;

namespace Semana03.Tests;

public class RepositorioVendasTests
{
    private readonly RepositorioVendas _repo;

    public RepositorioVendasTests()
    {
        _repo = new RepositorioVendas();
        _repo.GerarDadosExemplo();
    }

    [Fact]
    public void Repositorio_GerarDadosExemplo_DeveTer50Vendas()
    {
        Assert.Equal(50, _repo.TotalVendas);
    }

    [Fact]
    public void Repositorio_FiltrarPorCategoria_DeveRetornarApenasCategoria()
    {
        var eletronicos = _repo.FiltrarPorCategoria(CategoriaProduto.Eletronicos);
        
        Assert.All(eletronicos, v => Assert.Equal(CategoriaProduto.Eletronicos, v.Categoria));
    }

    [Fact]
    public void Repositorio_FiltrarPorValorMinimo_DeveRetornarVendasAcimaDoValor()
    {
        var filtradas = _repo.FiltrarPorValorMinimo(100m).ToList();
        
        Assert.All(filtradas, v => Assert.True(v.ValorTotal >= 100m));
    }

    [Fact]
    public void Repositorio_OrdenarPorData_DeveRetornarDecrescente()
    {
        var ordenadas = _repo.OrdenarPorData(decrescente: true).ToList();
        
        for (int i = 1; i < ordenadas.Count; i++)
        {
            Assert.True(ordenadas[i - 1].Data >= ordenadas[i].Data);
        }
    }

    [Fact]
    public void Repositorio_OrdenarPorValor_DeveRetornarDecrescente()
    {
        var ordenadas = _repo.OrdenarPorValor(decrescente: true).ToList();
        
        for (int i = 1; i < ordenadas.Count; i++)
        {
            Assert.True(ordenadas[i - 1].ValorTotal >= ordenadas[i].ValorTotal);
        }
    }

    [Fact]
    public void Repositorio_AgruparPorCategoria_DeveRetornarTodasCategorias()
    {
        var grupos = _repo.AgruparPorCategoria().ToList();
        
        Assert.True(grupos.Count > 0);
        Assert.Equal(5, grupos.Count); // 5 categorias diferentes
    }

    [Fact]
    public void Repositorio_CalcularFaturamentoTotal_DeveRetornarSoma()
    {
        decimal faturamento = _repo.CalcularFaturamentoTotal();
        
        Assert.True(faturamento > 0);
    }

    [Fact]
    public void Repositorio_CalcularTicketMedio_DeveRetornarMedia()
    {
        decimal ticket = _repo.CalcularTicketMedio();
        
        Assert.True(ticket > 0);
    }

    [Fact]
    public void Repositorio_MaiorVenda_DeveRetornarVendaExistente()
    {
        var maior = _repo.MaiorVenda();
        
        Assert.NotNull(maior);
    }

    [Fact]
    public void Repositorio_ProdutosMaisVendidos_DeveRetornarTopN()
    {
        var top3 = _repo.ProdutosMaisVendidos(3).ToList();
        
        Assert.Equal(3, top3.Count);
    }

    [Fact]
    public void Repositorio_ClientesMaisFrequentes_DeveRetornarTopN()
    {
        var top5 = _repo.ClientesMaisFrequentes(5).ToList();
        
        Assert.Equal(5, top5.Count);
    }

    [Fact]
    public void Repositorio_TemVendasAcimaDe_DeveRetornarTrueSeExistir()
    {
        bool tem = _repo.TemVendasAcimaDe(1m);
        
        Assert.True(tem);
    }

    [Fact]
    public void Repositorio_TodasVendasAcimaDe_DeveRetornarFalseSeAlgumaAbaixo()
    {
        bool todas = _repo.TodasVendasAcimaDe(10000m);
        
        Assert.False(todas);
    }

    [Fact]
    public void Repositorio_Paginar_DeveRetornarPaginaCorreta()
    {
        var pagina1 = _repo.Paginar(1, 5).ToList();
        var pagina2 = _repo.Paginar(2, 5).ToList();
        
        Assert.Equal(5, pagina1.Count);
        Assert.Equal(5, pagina2.Count);
        // Páginas devem ser diferentes
        Assert.NotEqual(pagina1[0].Id, pagina2[0].Id);
    }

    [Fact]
    public void Repositorio_CategoriasComVendas_DeveRetornarDistintas()
    {
        var categorias = _repo.CategoriasComVendas().ToList();
        
        Assert.True(categorias.Count > 0);
        Assert.Equal(categorias.Count, categorias.Distinct().Count());
    }
}

public class VendaTests
{
    [Fact]
    public void Venda_CalcularValorTotal_DeveRetornarMultiplicacao()
    {
        var venda = new Venda
        {
            Quantidade = 5,
            ValorUnitario = 10m
        };
        
        Assert.Equal(50m, venda.ValorTotal);
    }

    [Fact]
    public void Venda_ValorTotal_QuantidadeZero_DeveRetornarZero()
    {
        var venda = new Venda
        {
            Quantidade = 0,
            ValorUnitario = 100m
        };
        
        Assert.Equal(0m, venda.ValorTotal);
    }
}

public class ProdutoTests
{
    [Fact]
    public void Produto_CriarStruct_DeveTerValorPadrao()
    {
        var produto = new Produto();
        
        Assert.Equal(0, produto.Id);
        Assert.Null(produto.Nome);
        Assert.Equal(0m, produto.PrecoUnitario);
    }

    [Fact]
    public void Produto_Copia_DeveSerIndependente()
    {
        var original = new Produto 
        { 
            Id = 1, 
            Nome = "Arroz", 
            PrecoUnitario = 15.90m 
        };
        
        var copia = original;
        copia.PrecoUnitario = 20m;
        
        Assert.Equal(15.90m, original.PrecoUnitario);
        Assert.Equal(20m, copia.PrecoUnitario);
    }
}

public class LinqOperationsTests
{
    private readonly List<Venda> _vendas;

    public LinqOperationsTests()
    {
        _vendas = new List<Venda>
        {
            new() { Id = 1, ProdutoNome = "Arroz", Categoria = CategoriaProduto.Alimentos, Quantidade = 10, ValorUnitario = 15.90m, Data = new DateTime(2026, 1, 15) },
            new() { Id = 2, ProdutoNome = "Feijão", Categoria = CategoriaProduto.Alimentos, Quantidade = 5, ValorUnitario = 8.50m, Data = new DateTime(2026, 2, 20) },
            new() { Id = 3, ProdutoNome = "Notebook", Categoria = CategoriaProduto.Eletronicos, Quantidade = 1, ValorUnitario = 3500m, Data = new DateTime(2026, 3, 10) },
            new() { Id = 4, ProdutoNome = "Mouse", Categoria = CategoriaProduto.Eletronicos, Quantidade = 3, ValorUnitario = 89.90m, Data = new DateTime(2026, 4, 5) },
            new() { Id = 5, ProdutoNome = "Camiseta", Categoria = CategoriaProduto.Roupas, Quantidade = 2, ValorUnitario = 49.90m, Data = new DateTime(2026, 5, 1) },
        };
    }

    [Fact]
    public void Where_FiltrarPorCategoria_DeveRetornarCorreto()
    {
        var alimentos = _vendas.Where(v => v.Categoria == CategoriaProduto.Alimentos).ToList();
        
        Assert.Equal(2, alimentos.Count);
    }

    [Fact]
    public void OrderByDescending_OrdenarPorValorTotal_DeveRetornarOrdenado()
    {
        var ordenados = _vendas.OrderByDescending(v => v.ValorTotal).ToList();
        
        Assert.Equal("Notebook", ordenados[0].ProdutoNome); // 3500
        Assert.Equal("Mouse", ordenados[1].ProdutoNome);    // 269.70
    }

    [Fact]
    public void GroupBy_AgruparPorCategoria_DeveRetornarGrupos()
    {
        var grupos = _vendas.GroupBy(v => v.Categoria).ToList();
        
        Assert.Equal(3, grupos.Count);
    }

    [Fact]
    public void Select_ProjetarNomes_DeveRetornarApenasNomes()
    {
        var nomes = _vendas.Select(v => v.ProdutoNome).ToList();
        
        Assert.Equal(5, nomes.Count);
        Assert.Contains("Arroz", nomes);
    }

    [Fact]
    public void Sum_CalcularTotal_DeveRetornarSoma()
    {
        decimal total = _vendas.Sum(v => v.ValorTotal);
        
        decimal esperado = 15.90m * 10 + 8.50m * 5 + 3500m + 89.90m * 3 + 49.90m * 2;
        Assert.Equal(esperado, total);
    }

    [Fact]
    public void Average_CalcularMedia_DeveRetornarMedia()
    {
        decimal media = _vendas.Average(v => v.ValorTotal);
        
        Assert.True(media > 0);
    }

    [Fact]
    public void Any_VerificarExistencia_DeveRetornarTrue()
    {
        bool temEletronicos = _vendas.Any(v => v.Categoria == CategoriaProduto.Eletronicos);
        
        Assert.True(temEletronicos);
    }

    [Fact]
    public void All_VerificarTodos_DeveRetornarFalse()
    {
        bool todosAlimentos = _vendas.All(v => v.Categoria == CategoriaProduto.Alimentos);
        
        Assert.False(todosAlimentos);
    }

    [Fact]
    public void FirstOrDefault_BuscarPrimeiro_DeveRetornarPrimeiro()
    {
        var primeiro = _vendas.FirstOrDefault(v => v.Categoria == CategoriaProduto.Alimentos);
        
        Assert.NotNull(primeiro);
        Assert.Equal("Arroz", primeiro.ProdutoNome);
    }

    [Fact]
    public void FirstOrDefault_BuscarInexistente_DeveRetornarNull()
    {
        var resultado = _vendas.FirstOrDefault(v => v.ProdutoNome == "Inexistente");
        
        Assert.Null(resultado);
    }

    [Fact]
    public void Take_PegarPrimeiros_DeveRetornarQuantidadeCorreta()
    {
        var top3 = _vendas.Take(3).ToList();
        
        Assert.Equal(3, top3.Count);
    }

    [Fact]
    public void Skip_PularPrimeiros_DeveRetornarRestante()
    {
        var restantes = _vendas.Skip(2).ToList();
        
        Assert.Equal(3, restantes.Count);
    }

    [Fact]
    public void Distinct_CategoriasUnicas_DeveRetornarDistintas()
    {
        var categorias = _vendas.Select(v => v.Categoria).Distinct().ToList();
        
        Assert.Equal(3, categorias.Count);
    }

    [Fact]
    public void Count_ComCondicao_DeveRetornarQuantidade()
    {
        int caros = _vendas.Count(v => v.ValorTotal > 100);
        
        Assert.Equal(3, caros); // Arroz (159), Notebook (3500), Mouse (269.70)
    }

    [Fact]
    public void SelectMany_NaoAplicavelAqui_MasTestaComListaDeListas()
    {
        var listas = new List<List<int>> { new() { 1, 2 }, new() { 3, 4 } };
        var achatado = listas.SelectMany(l => l).ToList();
        
        Assert.Equal(new[] { 1, 2, 3, 4 }, achatado);
    }

    [Fact]
    public void Aggregate_AcumularValores_DeveRetornarTotal()
    {
        var numeros = new List<int> { 1, 2, 3, 4, 5 };
        int soma = numeros.Aggregate((a, b) => a + b);
        
        Assert.Equal(15, soma);
    }

    [Fact]
    public void ToDictionary_CriarDicionario_DeveMapearCorretamente()
    {
        var dicionario = _vendas.ToDictionary(v => v.Id);
        
        Assert.Equal(5, dicionario.Count);
        Assert.Equal("Arroz", dicionario[1].ProdutoNome);
    }

    [Fact]
    public void ToLookup_CriarLookup_DeveAgrupar()
    {
        var lookup = _vendas.ToLookup(v => v.Categoria);
        
        Assert.Equal(3, lookup.Count);
        Assert.Equal(2, lookup[CategoriaProduto.Alimentos].Count());
    }
}

public class DeferredExecutionTests
{
    [Fact]
    public void DeferredExecution_QueryNaoExecutaImediatamente()
    {
        var execuções = 0;
        var numeros = new List<int> { 1, 2, 3, 4, 5 };
        
        var query = numeros.Where(n =>
        {
            execuções++;
            return n > 3;
        });
        
        // Query ainda não foi executada
        Assert.Equal(0, execuções);
        
        // Ao iterar, executa
        var resultado = query.ToList();
        Assert.True(execuções > 0);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public void DeferredExecution_MultiplasIteracoes_ExecutaNovamente()
    {
        var execuções = 0;
        var numeros = new List<int> { 1, 2, 3 };
        
        var query = numeros.Where(n =>
        {
            execuções++;
            return n > 1;
        });
        
        query.ToList(); // Primeira execução
        int execuções1 = execuções;
        
        query.ToList(); // Segunda execução
        Assert.True(execuções > execuções1); // Executou novamente
    }

    [Fact]
    public void MaterializedExecution_ToListaExecutaUmaVez()
    {
        var numeros = new List<int> { 1, 2, 3, 4, 5 };
        
        var lista = numeros.Where(n => n > 2).ToList();
        
        // Lista está em memória — não re-executa
        Assert.Equal(3, lista.Count);
        Assert.Equal(3, lista[0]);
    }
}

public class PerformanceTests
{
    [Fact]
    public void Performance_1MilhaoVendas_Linq_Rapido()
    {
        var repo = new RepositorioVendas();
        var random = new Random(42);
        
        // Gerar 100.000 vendas (1 milhão pode ser lento em CI)
        for (int i = 0; i < 100_000; i++)
        {
            repo.RegistrarVenda(new Venda
            {
                ProdutoId = random.Next(1, 100),
                ProdutoNome = $"Produto {random.Next(1, 100)}",
                Categoria = (CategoriaProduto)random.Next(5),
                Quantidade = random.Next(1, 10),
                ValorUnitario = (decimal)(random.NextDouble() * 1000),
                Data = new DateTime(2026, random.Next(1, 13), random.Next(1, 28)),
                Cliente = $"Cliente {random.Next(1, 1000)}"
            });
        }
        
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        var resultado = repo.Vendas
            .Where(v => v.ValorTotal > 500)
            .OrderByDescending(v => v.ValorTotal)
            .Take(10)
            .ToList();
        
        sw.Stop();
        
        Assert.True(sw.ElapsedMilliseconds < 5000, $"LINQ demorou {sw.ElapsedMilliseconds}ms — muito lento!");
        Assert.True(resultado.Count <= 10);
    }
}
