// ============================================
// DESAFIOS SEMANAIS — Semana 1
// Controle de Estoque de Mercadinho
// ============================================

using Semana01.ControleEstoque;

namespace Semana01.Desafios;

/// <summary>
/// Desafio 1 — Fácil: Tipos por Valor
/// 
/// Você precisa registrar produtos de um mercadinho. Cada produto tem: 
/// nome (string), código de barras (string), preço (decimal), quantidade 
/// em estoque (int) e unidade de medida (enum: Unidade, Kilograma, Litro).
/// 
/// a) Crie um `struct` chamado `ProdutoPerecivel` com os campos acima.
/// b) Instancie 3 produtos diferentes.
/// c) Atualize a quantidade em estoque de cada um.
/// d) Explique por que `decimal` é um tipo por valor e não por referência.
/// </summary>
public class Desafio01_Fácil
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 1 — Fácil ===");
        
        // a) Struct criada em Models.cs
        
        // b) Instanciando 3 produtos
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
        
        // c) Atualizando estoque
        arroz.QuantidadeEstoque += 50;
        feijao.QuantidadeEstoque -= 10;
        leite.QuantidadeEstoque += 25;
        
        Console.WriteLine($"Arroz: {arroz.QuantidadeEstoque} unidades");
        Console.WriteLine($"Feijão: {feijao.QuantidadeEstoque} unidades");
        Console.WriteLine($"Leite: {leite.QuantidadeEstoque} unidades");
        
        // d) Explicação:
        Console.WriteLine("\nExplicação: decimal é um tipo por valor porque é um struct de 128 bits.");
        Console.WriteLine("Ele é armazenado diretamente na stack, não na heap.");
        Console.WriteLine("Isso significa que atribuir decimal a outra variável COPIA o valor,");
        Console.WriteLine("não a referência. Por isso é ideal para valores monetários.");
    }
}

/// <summary>
/// Desafio 2 — Médio: Cópia vs Referência
/// 
/// O mercadinho está fazendo inventário. Você precisa criar uma cópia do 
/// estoque atual para comparação futura.
/// 
/// a) Crie uma `class` chamada `Estoque` que contém uma lista de `ProdutoPerecivel`.
/// b) Crie um método `AdicionarProduto` e `BuscarPorCodigo`.
/// c) O que acontece se você alterar o produto retornado? A lista é afetada?
/// d) Implemente `ClonarEstoque` com deep copy.
/// e) Explique shallow copy vs deep copy.
/// </summary>
public class Desafio02_Médio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 2 — Médio ===");
        
        Estoque estoque = new Estoque("Mercadinho Test");
        estoque.AdicionarProduto(new ProdutoPerecivel
        {
            Nome = "Arroz",
            CodigoBarras = "123",
            Preco = new Dinheiro(15.90m),
            QuantidadeEstoque = 100
        });
        
        // c) O que acontece ao alterar o produto retornado?
        var produto = estoque.BuscarPorCodigo("123");
        if (produto != null)
        {
            var copia = produto.Value;
            copia.Preco = new Dinheiro(99.90m);
            // A lista NÃO é afetada! Struct é tipo por valor.
            Console.WriteLine($"Cópia alterada: {copia.Preco}");
            Console.WriteLine($"Original: {estoque.BuscarPorCodigo("123")?.Preco}");
        }
        
        // d) Deep copy seria copiar cada elemento da lista para uma nova lista
        // Como struct é por valor, cada cópia já é independente
        
        // e) Shallow copy: copia apenas a referência do objeto
        // Deep copy: copia o objeto inteiro e todos os objetos aninhados
        Console.WriteLine("\nShallow copy = copiar referência");
        Console.WriteLine("Deep copy = copiar valor recursivamente");
    }
}

/// <summary>
/// Desafio 3 — Desafio: Atualização em Lote com ref e Boxing
/// 
/// O mercadinho recebeu uma remessa e precisa atualizar o preço de vários 
/// produtos simultaneamente (ajuste inflacionário de 5%).
/// 
/// a) Crie um método `AjustarPrecos` usando `ref` para modificar diretamente.
/// b) O que aconteceria sem `ref`? Demonstre.
/// c) Implemente versão com `out` que retorna relatório.
/// d) Crie `List<object>` com 100.000 preços (boxing) e meça o tempo.
///    Depois faça com `List<decimal>` e compare.
/// </summary>
public class Desafio03_Desafio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 3 — Desafio ===");
        
        Estoque estoque = new Estoque("Test");
        
        ProdutoPerecivel[] produtos = 
        {
            new ProdutoPerecivel { Nome = "Arroz", Preco = new Dinheiro(10.00m) },
            new ProdutoPerecivel { Nome = "Feijão", Preco = new Dinheiro(20.00m) },
            new ProdutoPerecivel { Nome = "Leite", Preco = new Dinheiro(5.00m) }
        };
        
        // a) Com ref
        Console.WriteLine("Sem ajuste:");
        foreach (var p in produtos) Console.WriteLine($"  {p.Nome}: {p.Preco}");
        
        estoque.AjustarPrecos(ref produtos, 5.0m);
        
        Console.WriteLine("Com ajuste de 5%:");
        foreach (var p in produtos) Console.WriteLine($"  {p.Nome}: {p.Preco}");
        
        // b) Sem ref: o array original não seria modificado
        // porque arrays são referência, mas os elementos seriam copiados
        
        // c) Com out
        ProdutoPerecivel[] produtos2 = 
        {
            new ProdutoPerecivel { Nome = "Arroz", Preco = new Dinheiro(10.00m) }
        };
        
        estoque.TentarAjustarPrecosComRelatorio(ref produtos2, 10.0m, out string relatorio);
        Console.WriteLine(relatorio);
        
        // d) Boxing vs genérico
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var listaBoxing = new System.Collections.ArrayList();
        for (int i = 0; i < 100_000; i++) listaBoxing.Add((decimal)i);
        sw.Stop();
        Console.WriteLine($"ArrayList (boxing): {sw.ElapsedMilliseconds}ms");
        
        sw.Restart();
        var listaGenerica = new List<decimal>();
        for (int i = 0; i < 100_000; i++) listaGenerica.Add((decimal)i);
        sw.Stop();
        Console.WriteLine($"List<decimal>: {sw.ElapsedMilliseconds}ms");
    }
}
