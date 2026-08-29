using Semana01.ControleEstoque;
using Semana01.Desafios;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  CONTROLE DE ESTOQUE - MERCADINHÃO SÃO JOSÉ             ║");
Console.WriteLine("║  Demonstração: Tipos por Valor vs Referência            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

// ============================================
// PARTE 1: Structs (tipos por valor)
// ============================================
Console.WriteLine("\n━━━ PARTE 1: Criando produtos (structs - tipos por valor) ━━━");

ProdutoPerecivel arroz = new ProdutoPerecivel
{
    Nome = "Arroz Integral 5kg",
    CodigoBarras = "7891234567890",
    Preco = new Dinheiro(15.90m),
    QuantidadeEstoque = 100,
    Unidade = UnidadeMedida.Unidade,
    DataValidade = DateTime.Now.AddMonths(6)
};

ProdutoPerecivel feijao = new ProdutoPerecivel
{
    Nome = "Feijão Preto 1kg",
    CodigoBarras = "7891234567891",
    Preco = new Dinheiro(8.50m),
    QuantidadeEstoque = 80,
    Unidade = UnidadeMedida.Unidade,
    DataValidade = DateTime.Now.AddMonths(8)
};

ProdutoPerecivel leite = new ProdutoPerecivel
{
    Nome = "Leite Integral 1L",
    CodigoBarras = "7891234567892",
    Preco = new Dinheiro(4.75m),
    QuantidadeEstoque = 50,
    Unidade = UnidadeMedida.Litro,
    DataValidade = DateTime.Now.AddDays(15)
};

Console.WriteLine($"Produto 1: {arroz}");
Console.WriteLine($"Produto 2: {feijao}");
Console.WriteLine($"Produto 3: {leite}");

// ============================================
// PARTE 2: Cópia de structs (valor independente)
// ============================================
Console.WriteLine("\n━━━ PARTE 2: Cópia de structs (valor independente) ━━━");

ProdutoPerecivel copiaArroz = arroz;  // COPIA o valor inteiro
copiaArroz.Preco = new Dinheiro(18.90m); // Modifica apenas a cópia

Console.WriteLine($"Original: {arroz.Nome} - {arroz.Preco}");
Console.WriteLine($"Cópia:    {copiaArroz.Nome} - {copiaArroz.Preco}");
Console.WriteLine("→ A cópia é independente! O original não foi afetado.");

// ============================================
// PARTE 3: Classes (tipos por referência)
// ============================================
Console.WriteLine("\n━━━ PARTE 3: Criando estoque (class - tipo por referência) ━━━");

Estoque estoque = new Estoque("Mercadinhão São José");
estoque.AdicionarProduto(arroz);
estoque.AdicionarProduto(feijao);
estoque.AdicionarProduto(leite);

Console.WriteLine($"Estoque criado: {estoque.Nome}");
Console.WriteLine($"Produtos no estoque: {estoque.Produtos.Count}");

// ============================================
// PARTE 4: Referência compartilhada
// ============================================
Console.WriteLine("\n━━━ PARTE 4: Referência compartilhada ━━━");

Estoque referenciaEstoque = estoque; // COPIA a referência (não o objeto!)
referenciaEstoque.AdicionarProduto(new ProdutoPerecivel
{
    Nome = "Açúcar 1kg",
    CodigoBarras = "7891234567893",
    Preco = new Dinheiro(3.99m),
    QuantidadeEstoque = 60,
    Unidade = UnidadeMedida.Kilograma,
    DataValidade = DateTime.Now.AddYears(1)
});

Console.WriteLine($"estoque.Produtos.Count: {estoque.Produtos.Count}");
Console.WriteLine($"referenciaEstoque.Produtos.Count: {referenciaEstoque.Produtos.Count}");
Console.WriteLine("→ Ambas apontam para o MESMO objeto! Adicionar em uma afeta a outra.");

// ============================================
// PARTE 5: Registro de transações
// ============================================
Console.WriteLine("\n━━━ PARTE 5: Registrando entradas e saídas ━━━");

estoque.RegistrarEntrada("7891234567890", 50, "Reposição fornecedor");
estoque.RegistrarSaida("7891234567890", 30, "Venda balcão");
estoque.RegistrarSaida("7891234567892", 10, "Venda delivery");

foreach (var transacao in estoque.Historico)
{
    Console.WriteLine($"  {transacao}");
}

// ============================================
// PARTE 6: Ajuste de preços com ref
// ============================================
Console.WriteLine("\n━━━ PARTE 6: Ajuste de preços com ref ━━━");

ProdutoPerecivel[] produtosParaAjuste = 
{
    estoque.BuscarPorCodigo("7891234567890")!.Value,
    estoque.BuscarPorCodigo("7891234567891")!.Value,
    estoque.BuscarPorCodigo("7891234567892")!.Value
};

Console.WriteLine("Preços antes do ajuste:");
foreach (var p in produtosParaAjuste)
{
    Console.WriteLine($"  {p.Nome}: {p.Preco}");
}

// Ajuste de 5% usando ref
estoque.AjustarPrecos(ref produtosParaAjuste, 5.0m);

Console.WriteLine("\nPreços após ajuste de 5%:");
foreach (var p in produtosParaAjuste)
{
    Console.WriteLine($"  {p.Nome}: {p.Preco}");
}

// ============================================
// PARTE 7: Relatório com out
// ============================================
Console.WriteLine("\n━━━ PARTE 7: Relatório com out ━━━");

ProdutoPerecivel[] produtosRelatorio = 
{
    estoque.BuscarPorCodigo("7891234567890")!.Value,
    estoque.BuscarPorCodigo("7891234567891")!.Value
};

bool sucesso = estoque.TentarAjustarPrecosComRelatorio(
    ref produtosRelatorio, 
    -2.0m,  // desconto de 2%
    out string relatorio);

if (sucesso)
{
    Console.WriteLine(relatorio);
}

// ============================================
// PARTE 8: Demonstração de Boxing
// ============================================
Console.WriteLine("\n━━━ PARTE 8: Boxing e Performance ━━━");

var stopwatch = System.Diagnostics.Stopwatch.StartNew();

// Sem boxing: List<int>
var listaOtimizada = new List<int>();
for (int i = 0; i < 100_000; i++)
{
    listaOtimizada.Add(i); // int vai direto para o array interno
}
stopwatch.Stop();
Console.WriteLine($"List<int> (sem boxing): {stopwatch.ElapsedMilliseconds}ms");

// Com boxing: ArrayList
stopwatch.Restart();
var listaLenta = new System.Collections.ArrayList();
for (int i = 0; i < 100_000; i++)
{
    listaLenta.Add(i); // cada int é "embalado" (boxing) para a heap!
}
stopwatch.Stop();
Console.WriteLine($"ArrayList (com boxing): {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine("→ Boxing tem custo real de performance!");

// ============================================
// RELATÓRIO FINAL
// ============================================
Console.WriteLine("\n━━━ RELATÓRIO FINAL ━━━");
estoque.ExibirRelatorio();

// ============================================
// DESAFIOS SEMANAIS
// ============================================
Console.WriteLine("\n\n━━━ DESAFIOS SEMANAIS ━━━\n");

Console.WriteLine("Pressione 1 para Desafio 1 (Fácil)");
Console.WriteLine("Pressione 2 para Desafio 2 (Médio)");
Console.WriteLine("Pressione 3 para Desafio 3 (Desafio)");
Console.WriteLine("Pressione qualquer tecla para sair");

var key = Console.ReadKey();
Console.WriteLine();

switch (key.Key)
{
    case ConsoleKey.D1:
        new Semana01.Desafios.Desafio01_Fácil().Resolver();
        break;
    case ConsoleKey.D2:
        new Semana01.Desafios.Desafio02_Médio().Resolver();
        break;
    case ConsoleKey.D3:
        new Semana01.Desafios.Desafio03_Desafio().Resolver();
        break;
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
