using Semana03.AnaliseDados;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ANÁLISE DE VENDAS — LOJA ONLINE                        ║");
Console.WriteLine("║  Demonstra: Coleções, Generics, LINQ                    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

var repo = new RepositorioVendas();
repo.GerarDadosExemplo();

// ============================================
// PARTE 1: Filtros
// ============================================
Console.WriteLine("\n━━━ PARTE 1: Filtros com Where ━━━\n");

Console.WriteLine("Vendas de Eletrônicos:");
foreach (var v in repo.FiltrarPorCategoria(CategoriaProduto.Eletronicos).Take(3))
{
    Console.WriteLine($"  {v.ProdutoNome} - {v.Quantidade}x {v.ValorUnitario:C} = {v.ValorTotal:C}");
}

Console.WriteLine($"\nVendas acima de R$100: {repo.FiltrarPorValorMinimo(100m).Count()}");

// ============================================
// PARTE 2: Ordenação
// ============================================
Console.WriteLine("\n━━━ PARTE 2: Ordenação com OrderBy ━━━\n");

Console.WriteLine("Top 5 vendas por valor:");
foreach (var v in repo.OrdenarPorValor().Take(5))
{
    Console.WriteLine($"  {v.ProdutoNome} - {v.ValorTotal:C} ({v.Data:dd/MM/yyyy})");
}

// ============================================
// PARTE 3: Agrupamento
// ============================================
Console.WriteLine("\n━━━ PARTE 3: Agrupamento com GroupBy ━━━\n");

Console.WriteLine("Faturamento por categoria:");
foreach (var grupo in repo.AgruparPorCategoria())
{
    Console.WriteLine($"  {grupo.Categoria}: {grupo.TotalVendas} vendas, " +
                      $"Total: {grupo.ValorTotal:C}, Ticket Médio: {grupo.TicketMedio:C}");
}

// ============================================
// PARTE 4: Agregação
// ============================================
Console.WriteLine("\n━━━ PARTE 4: Agregação com Sum, Average, Max ━━━\n");

Console.WriteLine($"Faturamento total: {repo.CalcularFaturamentoTotal():C}");
Console.WriteLine($"Ticket médio: {repo.CalcularTicketMedio():C}");

var maior = repo.MaiorVenda();
if (maior != null)
    Console.WriteLine($"Maior venda: {maior.ProdutoNome} - {maior.ValorTotal:C}");

// ============================================
// PARTE 5: Rankings
// ============================================
Console.WriteLine("\n━━━ PARTE 5: Rankings com LINQ ━━━\n");

Console.WriteLine("Produtos mais vendidos:");
foreach (var p in repo.ProdutosMaisVendidos(5))
    Console.WriteLine($"  {p}");

Console.WriteLine("\nClientes mais frequentes:");
foreach (var c in repo.ClientesMaisFrequentes(5))
    Console.WriteLine($"  {c}");

// ============================================
// PARTE 6: Quantificadores
// ============================================
Console.WriteLine("\n━━━ PARTE 6: Quantificadores ━━━\n");

Console.WriteLine($"Tem vendas acima de R$500? {repo.TemVendasAcimaDe(500m)}");
Console.WriteLine($"Todas as vendas acima de R$1? {repo.TodasVendasAcimaDe(1m)}");

// ============================================
// PARTE 7: Paginação
// ============================================
Console.WriteLine("\n━━━ PARTE 7: Paginação com Skip/Take ━━━\n");

Console.WriteLine("Página 1 (3 itens):");
foreach (var v in repo.Paginar(1, 3))
    Console.WriteLine($"  {v.ProdutoNome} - {v.ValorTotal:C}");

Console.WriteLine("Página 2 (3 itens):");
foreach (var v in repo.Paginar(2, 3))
    Console.WriteLine($"  {v.ProdutoNome} - {v.ValorTotal:C}");

// ============================================
// PARTE 8: Performance com LINQ
// ============================================
Console.WriteLine("\n━━━ PARTE 8: Performance com LINQ ━━━\n");

var sw = System.Diagnostics.Stopwatch.StartNew();

// Gerar 1.000.000 de vendas
var repoGrande = new RepositorioVendas();
var random = new Random(42);
for (int i = 0; i < 1_000_000; i++)
{
    repoGrande.RegistrarVenda(new Venda
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

sw.Stop();
Console.WriteLine($"Geradas {repoGrande.TotalVendas:N0} vendas em {sw.ElapsedMilliseconds}ms");

// Query com deferred execution
sw.Restart();
var query = repoGrande.Vendas
    .Where(v => v.ValorTotal > 500)
    .OrderByDescending(v => v.ValorTotal)
    .Take(10);
sw.Stop();
Console.WriteLine($"Query definida em {sw.ElapsedMilliseconds}ms (ainda não executou!)");

sw.Restart();
var resultado = query.ToList(); // Executa aqui
sw.Stop();
Console.WriteLine($"Query executada (ToList) em {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Resultado: {resultado.Count} vendas");

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
