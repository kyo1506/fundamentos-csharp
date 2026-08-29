// ============================================
// MODELS — Análise de Vendas de Loja Online
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Semana03.AnaliseDados;

// ============================================
// MODELOS
// ============================================

public enum CategoriaProduto
{
    Alimentos,
    Eletronicos,
    Roupas,
    Bebidas,
    Limpeza
}

public struct Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public CategoriaProduto Categoria { get; set; }
    public decimal PrecoUnitario { get; set; }
    public int Estoque { get; set; }
}

public class Venda
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public CategoriaProduto Categoria { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;
    public DateTime Data { get; set; }
    public string Cliente { get; set; } = string.Empty;
}

// ============================================
// REPOSITÓRIO (Generics + LINQ)
// ============================================

public class RepositorioVendas
{
    private readonly List<Venda> _vendas = new();
    private readonly Dictionary<int, Produto> _catalogo = new();
    private int _proximoId = 1;

    public void AdicionarProduto(Produto produto)
    {
        _catalogo[produto.Id] = produto;
    }

    public void RegistrarVenda(Venda venda)
    {
        venda.Id = _proximoId++;
        _vendas.Add(venda);
    }

    // LINQ: filtrar vendas
    public IEnumerable<Venda> FiltrarPorCategoria(CategoriaProduto categoria)
    {
        return _vendas.Where(v => v.Categoria == categoria);
    }

    public IEnumerable<Venda> FiltrarPorPeriodo(DateTime inicio, DateTime fim)
    {
        return _vendas.Where(v => v.Data >= inicio && v.Data <= fim);
    }

    public IEnumerable<Venda> FiltrarPorValorMinimo(decimal valorMinimo)
    {
        return _vendas.Where(v => v.ValorTotal >= valorMinimo);
    }

    // LINQ: ordenar
    public IEnumerable<Venda> OrdenarPorData(bool decrescente = true)
    {
        return decrescente 
            ? _vendas.OrderByDescending(v => v.Data) 
            : _vendas.OrderBy(v => v.Data);
    }

    public IEnumerable<Venda> OrdenarPorValor(bool decrescente = true)
    {
        return decrescente 
            ? _vendas.OrderByDescending(v => v.ValorTotal) 
            : _vendas.OrderBy(v => v.ValorTotal);
    }

    // LINQ: agrupar
    public IEnumerable<dynamic> AgruparPorCategoria()
    {
        return _vendas
            .GroupBy(v => v.Categoria)
            .Select(g => new 
            {
                Categoria = g.Key,
                TotalVendas = g.Count(),
                ValorTotal = g.Sum(v => v.ValorTotal),
                TicketMedio = g.Average(v => v.ValorTotal),
                QuantidadeTotal = g.Sum(v => v.Quantidade)
            })
            .OrderByDescending(x => x.ValorTotal);
    }

    public IEnumerable<dynamic> AgruparPorMes()
    {
        return _vendas
            .GroupBy(v => new { v.Data.Year, v.Data.Month })
            .Select(g => new 
            {
                Ano = g.Key.Year,
                Mes = g.Key.Month,
                TotalVendas = g.Count(),
                ValorTotal = g.Sum(v => v.ValorTotal)
            })
            .OrderBy(x => x.Ano)
            .ThenBy(x => x.Mes);
    }

    // LINQ: agregação
    public decimal CalcularFaturamentoTotal()
    {
        return _vendas.Sum(v => v.ValorTotal);
    }

    public decimal CalcularTicketMedio()
    {
        return _vendas.Any() ? _vendas.Average(v => v.ValorTotal) : 0;
    }

    public Venda? MaiorVenda()
    {
        return _vendas.OrderByDescending(v => v.ValorTotal).FirstOrDefault();
    }

    // LINQ: busca
    public IEnumerable<string> ProdutosMaisVendidos(int topN = 5)
    {
        return _vendas
            .GroupBy(v => v.ProdutoId)
            .Select(g => new 
            {
                ProdutoId = g.Key,
                Quantidade = g.Sum(v => v.Quantidade),
                Nome = g.First().ProdutoNome
            })
            .OrderByDescending(x => x.Quantidade)
            .Take(topN)
            .Select(x => $"{x.Nome} ({x.Quantidade} unidades)");
    }

    public IEnumerable<string> ClientesMaisFrequentes(int topN = 5)
    {
        return _vendas
            .GroupBy(v => v.Cliente)
            .Select(g => new 
            {
                Cliente = g.Key,
                Compras = g.Count(),
                TotalGasto = g.Sum(v => v.ValorTotal)
            })
            .OrderByDescending(x => x.Compras)
            .Take(topN)
            .Select(x => $"{x.Cliente} ({x.Compras} compras, {x.TotalGasto:C})");
    }

    // LINQ: quantificadores
    public bool TemVendasAcimaDe(decimal valor)
    {
        return _vendas.Any(v => v.ValorTotal > valor);
    }

    public bool TodasVendasAcimaDe(decimal valor)
    {
        return _vendas.All(v => v.ValorTotal > valor);
    }

    // LINQ: paginação
    public IEnumerable<Venda> Paginar(int pagina, int itensPorPagina)
    {
        return _vendas
            .Skip((pagina - 1) * itensPorPagina)
            .Take(itensPorPagina);
    }

    // LINQ: distinct
    public IEnumerable<string> CategoriasComVendas()
    {
        return _vendas.Select(v => v.Categoria.ToString()).Distinct();
    }

    // Gerar dados de exemplo
    public void GerarDadosExemplo()
    {
        var produtos = new[]
        {
            new Produto { Id = 1, Nome = "Arroz 5kg", Categoria = CategoriaProduto.Alimentos, PrecoUnitario = 15.90m, Estoque = 100 },
            new Produto { Id = 2, Nome = "Feijão 1kg", Categoria = CategoriaProduto.Alimentos, PrecoUnitario = 8.50m, Estoque = 80 },
            new Produto { Id = 3, Nome = "Notebook", Categoria = CategoriaProduto.Eletronicos, PrecoUnitario = 3500m, Estoque = 10 },
            new Produto { Id = 4, Nome = "Mouse", Categoria = CategoriaProduto.Eletronicos, PrecoUnitario = 89.90m, Estoque = 50 },
            new Produto { Id = 5, Nome = "Camiseta", Categoria = CategoriaProduto.Roupas, PrecoUnitario = 49.90m, Estoque = 30 },
            new Produto { Id = 6, Nome = "Refrigerante 2L", Categoria = CategoriaProduto.Bebidas, PrecoUnitario = 7.90m, Estoque = 60 },
            new Produto { Id = 7, Nome = "Detergente", Categoria = CategoriaProduto.Limpeza, PrecoUnitario = 3.50m, Estoque = 40 },
        };

        foreach (var p in produtos) AdicionarProduto(p);

        var random = new Random(42);
        var clientes = new[] { "Maria", "João", "Ana", "Carlos", "Pedro", "Lucia", "Fernando" };

        // Gerar 50 vendas aleatórias
        for (int i = 0; i < 50; i++)
        {
            var produto = produtos[random.Next(produtos.Length)];
            var venda = new Venda
            {
                ProdutoId = produto.Id,
                ProdutoNome = produto.Nome,
                Categoria = produto.Categoria,
                Quantidade = random.Next(1, 10),
                ValorUnitario = produto.PrecoUnitario,
                Data = new DateTime(2026, random.Next(1, 13), random.Next(1, 28)),
                Cliente = clientes[random.Next(clientes.Length)]
            };
            RegistrarVenda(venda);
        }
    }

    public int TotalVendas => _vendas.Count;
    public IEnumerable<Venda> Vendas => _vendas.AsReadOnly();
}
