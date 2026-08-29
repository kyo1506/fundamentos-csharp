// ============================================
// DESAFIOS SEMANAIS — Semana 3
// Análise de Vendas de Loja Online
// ============================================

using Semana03.AnaliseDados;

namespace Semana03.Desafios;

/// <summary>
/// Desafio 1 — Fácil: Generics e Dictionary
/// 
/// Crie um repositório genérico para armazenar dados da loja:
/// 
/// a) Crie uma interface `IRepositorio<T>` com métodos `Adicionar(T item)`, 
///    `Remover(int id)`, `Listar()` e `Buscar(Predicate<T> criterio)`.
/// b) Implemente `RepositorioMemoria<T>` que usa `List<T>` internamente.
/// c) Use o repositório para armazenar `Produto` e `Venda`.
/// d) Crie um `Dictionary<int, Produto>` para buscas por Id em O(1).
/// e) Crie um `Dictionary<string, List<Produto>>` que agrupa produtos por categoria.
/// </summary>
public class Desafio01_Fácil
{
    // a) Interface genérica
    public interface IRepositorio<T>
    {
        void Adicionar(T item);
        void Remover(int id);
        IEnumerable<T> Listar();
        IEnumerable<T> Buscar(Predicate<T> criterio);
    }

    // b) Implementação em memória
    public class RepositorioMemoria<T> : IRepositorio<T>
    {
        private readonly List<T> _items = new();
        private int _proximoId = 1;

        public void Adicionar(T item) => _items.Add(item);
        public void Remover(int id) => _items.RemoveAt(id - 1);
        public IEnumerable<T> Listar() => _items.AsReadOnly();
        public IEnumerable<T> Buscar(Predicate<T> criterio) => _items.Where(criterio);
    }

    public void Resolver()
    {
        Console.WriteLine("=== Desafio 1 — Fácil: Generics e Dictionary ===\n");

        // c) Usando repositório para Produto
        var repoProdutos = new RepositorioMemoria<Produto>();
        repoProdutos.Adicionar(new Produto { Id = 1, Nome = "Arroz", Categoria = CategoriaProduto.Alimentos, PrecoUnitario = 15.90m });
        repoProdutos.Adicionar(new Produto { Id = 2, Nome = "Feijão", Categoria = CategoriaProduto.Alimentos, PrecoUnitario = 8.50m });
        repoProdutos.Adicionar(new Produto { Id = 3, Nome = "Notebook", Categoria = CategoriaProduto.Eletronicos, PrecoUnitario = 3500m });

        Console.WriteLine("Produtos no repositório:");
        foreach (var p in repoProdutos.Listar())
            Console.WriteLine($"  {p.Id}: {p.Nome} - {p.PrecoUnitario:C}");

        // d) Dictionary para busca O(1)
        var catalogo = repoProdutos.Listar().ToDictionary(p => p.Id);
        Console.WriteLine($"\nBusca por ID 2: {catalogo[2].Nome}");

        // e) Agrupamento por categoria
        var porCategoria = repoProdutos.Listar().GroupBy(p => p.Categoria.ToString()).ToDictionary(g => g.Key, g => g.ToList());
        Console.WriteLine("\nProdutos por categoria:");
        foreach (var kvp in porCategoria)
            Console.WriteLine($"  {kvp.Key}: {string.Join(", ", kvp.Value.Select(p => p.Nome))}");
    }
}

/// <summary>
/// Desafio 2 — Médio: LINQ
/// 
/// Com uma lista de 50 vendas (gere dados fictícios):
/// 
/// a) Filtre vendas com valor total > R$500.
/// b) Selecione apenas os nomes dos produtos vendidos (sem repetição).
/// c) Agrupe vendas por categoria e calcule o total por categoria.
/// d) Encontre o produto mais vendido (por quantidade).
/// e) Calcule o ticket médio (valor médio por venda).
/// f) Crie um objeto anônimo com `select new { ... }` para um relatório resumido.
/// </summary>
public class Desafio02_Médio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 2 — Médio: LINQ ===\n");

        var repo = new RepositorioVendas();
        repo.GerarDadosExemplo();

        // a) Vendas > R$500
        var vendasCaras = repo.FiltrarPorValorMinimo(500m).ToList();
        Console.WriteLine($"Vendas acima de R$500: {vendasCaras.Count}");

        // b) Nomes sem repetição
        var nomesUnicos = repo.Vendas.Select(v => v.ProdutoNome).Distinct().ToList();
        Console.WriteLine($"\nProdutos vendidos: {string.Join(", ", nomesUnicos)}");

        // c) Agrupar por categoria
        var porCategoria = repo.AgruparPorCategoria();
        Console.WriteLine("\nFaturamento por categoria:");
        foreach (var g in porCategoria)
            Console.WriteLine($"  {g.Categoria}: {g.ValorTotal:C} ({g.TotalVendas} vendas)");

        // d) Produto mais vendido
        var maisVendido = repo.ProdutosMaisVendidos(1).FirstOrDefault();
        Console.WriteLine($"\nProduto mais vendido: {maisVendido}");

        // e) Ticket médio
        Console.WriteLine($"\nTicket médio: {repo.CalcularTicketMedio():C}");

        // f) Relatório com objeto anônimo
        var relatorio = repo.Vendas.Select(v => new 
        {
            v.ProdutoNome,
            v.Quantidade,
            v.ValorTotal,
            Mes = v.Data.ToString("MM/yyyy")
        }).OrderByDescending(r => r.ValorTotal).Take(5).ToList();

        Console.WriteLine("\nTop 5 vendas (objeto anônimo):");
        foreach (var r in relatorio)
            Console.WriteLine($"  {r.ProdutoNome} - {r.Quantidade}x - {r.ValorTotal:C} ({r.Mes})");
    }
}

/// <summary>
/// Desafio 3 — Desafio: Performance com LINQ
/// 
/// A loja cresceu e agora tem 1.000.000 de vendas:
/// 
/// a) Gere 1.000.000 de vendas fictícias (use `Random` e `Enumerable.Range`).
/// b) Execute uma query LINQ complexa (filtro + ordenação + agrupamento) 
///    sem `ToList()` no final. Meça o tempo.
/// c) Agora execute a mesma query com `ToList()` no final. Meça o tempo.
/// d) Explique deferred execution com suas palavras.
/// e) O que é `IQueryable` e quando usá-lo em vez de `IEnumerable`?
/// </summary>
public class Desafio03_Desafio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 3 — Desafio: Performance com LINQ ===\n");

        var random = new Random(42);
        var repo = new RepositorioVendas();

        // a) Gerar 100.000 vendas (1mi é muito para demo)
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

        Console.WriteLine($"Vendas geradas: {repo.TotalVendas:N0}");

        // b) Query sem ToList (deferred)
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var query = repo.Vendas
            .Where(v => v.ValorTotal > 500)
            .OrderByDescending(v => v.ValorTotal)
            .GroupBy(v => v.Categoria);
        sw.Stop();
        Console.WriteLine($"\nQuery definida (sem ToList): {sw.ElapsedMilliseconds}ms");

        // c) Query com ToList (materializada)
        sw.Restart();
        var resultado = query.ToList();
        sw.Stop();
        Console.WriteLine($"Query executada (ToList): {sw.ElapsedMilliseconds}ms");

        // d) Explicação
        Console.WriteLine("\n━━━ Deferred Execution ━━━");
        Console.WriteLine("A query LINQ não é executada imediatamente.");
        Console.WriteLine("Ela é executada apenas quando iteramos sobre o resultado.");
        Console.WriteLine("Isso permite otimizar e combinar múltiplas operações.");

        // e) IQueryable
        Console.WriteLine("\n━━━ IEnumerable vs IQueryable ━━━");
        Console.WriteLine("IEnumerable: execução em memória (LINQ to Objects)");
        Console.WriteLine("IQueryable: execução no banco (LINQ to Entities/SQL)");
        Console.WriteLine("Use IQueryable com Entity Framework para filtrar no banco.");
    }
}
