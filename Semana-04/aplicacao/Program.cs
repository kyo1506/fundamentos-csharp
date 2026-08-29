using Semana04.APIControleEstoque;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  API CONTROLE DE ESTOQUE — SEMANA 4                     ║");
Console.WriteLine("║  Minimal API + Deploy + CI/CD                           ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

// ============================================
// DI: Configuração de dependências
// ============================================
IRepositorioProduto repositorio = new RepositorioProdutoMemoria();
IProdutoService service = new ProdutoService(repositorio);
var api = new ApiSimulator(service);

// ============================================
// Seed: Produtos iniciais
// ============================================
Console.WriteLine("\n━━━ Cadastrando produtos iniciais ━━━\n");

api.CriarProduto(new ProdutoRequest("Arroz 5kg", "Alimentos", 15.90m, 100));
api.CriarProduto(new ProdutoRequest("Feijão 1kg", "Alimentos", 8.50m, 80));
api.CriarProduto(new ProdutoRequest("Notebook", "Eletrônicos", 3500.00m, 10));
api.CriarProduto(new ProdutoRequest("Mouse", "Eletrônicos", 89.90m, 50));
api.CriarProduto(new ProdutoRequest("Camiseta", "Roupas", 49.90m, 30));

// ============================================
// PARTE 1: Listar todos os produtos
// ============================================
Console.WriteLine("\n━━━ PARTE 1: GET /produtos ━━━\n");

var lista = api.ListarProdutos();
if (lista.Sucesso)
{
    foreach (var p in lista.Dado!)
    {
        Console.WriteLine($"  [{p.Id}] {p.Nome} - {p.Categoria} - {p.Preco:C} - Estoque: {p.QuantidadeEstoque}");
    }
}

// ============================================
// PARTE 2: Buscar produto específico
// ============================================
Console.WriteLine("\n━━━ PARTE 2: GET /produtos/1 ━━━\n");

var busca = api.BuscarProduto(1);
Console.WriteLine(busca.Sucesso 
    ? $"  Encontrado: {busca.Dado!.Nome}" 
    : $"  Erro: {busca.Mensagem}");

// ============================================
// PARTE 3: Criar novo produto
// ============================================
Console.WriteLine("\n━━━ PARTE 3: POST /produtos ━━━\n");

var criacao = api.CriarProduto(new ProdutoRequest("Refrigerante 2L", "Bebidas", 7.90m, 60));
Console.WriteLine(criacao.Sucesso 
    ? $"  Criado: {criacao.Dado!.Nome} (ID: {criacao.Dado!.Id})" 
    : $"  Erro: {criacao.Mensagem}");

// ============================================
// PARTE 4: Atualizar produto
// ============================================
Console.WriteLine("\n━━━ PARTE 4: PUT /produtos/1 ━━━\n");

var atualizacao = api.AtualizarProduto(1, new ProdutoRequest("Arroz Integral 5kg", "Alimentos", 17.90m, 120));
Console.WriteLine(atualizacao.Sucesso 
    ? $"  Atualizado: {atualizacao.Dado!.Nome} - Novo preço: {atualizacao.Dado!.Preco:C}" 
    : $"  Erro: {atualizacao.Mensagem}");

// ============================================
// PARTE 5: Remover produto
// ============================================
Console.WriteLine("\n━━━ PARTE 5: DELETE /produtos/5 ━━━\n");

var remocao = api.RemoverProduto(5);
Console.WriteLine(remocao.Sucesso 
    ? $"  {remocao.Mensagem}" 
    : $"  Erro: {remocao.Mensagem}");

// ============================================
// PARTE 6: Filtrar por categoria
// ============================================
Console.WriteLine("\n━━━ PARTE 6: GET /produtos/categoria/Eletrônicos ━━━\n");

var filtro = api.FiltrarPorCategoria("Eletrônicos");
if (filtro.Sucesso)
{
    foreach (var p in filtro.Dado!)
    {
        Console.WriteLine($"  {p.Nome} - {p.Preco:C}");
    }
}

// ============================================
// PARTE 7: Estatísticas
// ============================================
Console.WriteLine("\n━━━ PARTE 7: GET /stats ━━━\n");

var stats = api.Estatisticas();
if (stats.Sucesso)
{
    Console.WriteLine($"  {stats.Dado}");
}

// ============================================
// PARTE 8: Teste de validação
// ============================================
Console.WriteLine("\n━━━ PARTE 8: Teste de Validação ━━━\n");

var invalido = api.CriarProduto(new ProdutoRequest("", "Teste", -10m, -5));
Console.WriteLine($"  Tentativa com dados inválidos: {invalido.Mensagem}");

// ============================================
// PARTE 9: Listagem final
// ============================================
Console.WriteLine("\n━━━ PARTE 9: Listagem Final ━━━\n");

var final = api.ListarProdutos();
if (final.Sucesso)
{
    foreach (var p in final.Dado!)
    {
        Console.WriteLine($"  [{p.Id}] {p.Nome} - {p.Categoria} - {p.Preco:C} - Estoque: {p.QuantidadeEstoque}");
    }
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
