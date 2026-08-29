# LINQ — Consultas Declarativas em C#

> **Semana 3 — Teoria** | Tempo estimado: 20 minutos

---

## O que é LINQ?

**LINQ (Language Integrated Query)** é uma forma declarativa de consultar dados diretamente em C#. Em vez de escrever loops `for` complexos, você expressa **o que** quer, não **como** fazer.

```
┌─────────────────────────────────────────────────────────────────┐
│  ABORDAGEM IMPERATIVA (como?)  │  ABORDAGEM DECLARATIVA (o quê?)│
├─────────────────────────────────────────────────────────────────┤
│  List<Produto> resultado = new(); │  var resultado = produtos    │
│  foreach (var p in produtos)     │      .Where(p => p.Preco > 100)│
│  {                                │      .OrderBy(p => p.Nome)    │
│      if (p.Preco > 100)          │      .ToList();               │
│      {                            │                               │
│          resultado.Add(p);        │  // O que você lê é o que     │
│      }                            │  // você obtém!               │
│  }                                │                               │
│  resultado.Sort();                │                               │
└─────────────────────────────────────────────────────────────────┘
```

---

## Sintaxe: Method vs Query

```csharp
List<Produto> produtos = GetProdutos();

// Method Syntax (mais comum, usa extensões LINQ)
var baratos = produtos
    .Where(p => p.Preco < 50)
    .OrderBy(p => p.Nome)
    .Select(p => new { p.Nome, p.Preco })
    .ToList();

// Query Syntax (SQL-like, menos usado)
var baratos2 = from p in produtos
               where p.Preco < 50
               orderby p.Nome
               select new { p.Nome, p.Preco };
```

---

## Operadores LINQ Essenciais

### 1. Where — Filtrar

```csharp
// Produtos com preço > 100
var caros = produtos.Where(p => p.Preco > 100);

// Múltiplas condições
var filtrados = produtos
    .Where(p => p.Preco > 50 && p.Categoria == "Eletrônicos");

// Index do elemento
var pares = numeros.Where((n, index) => index % 2 == 0);
```

### 2. Select — Projetar/Transformar

```csharp
// Extrair apenas nomes
var nomes = produtos.Select(p => p.Nome);

// Criar objeto anônimo
var resumo = produtos.Select(p => new 
{ 
    p.Nome, 
    PrecoComImposto = p.Preco * 1.1m 
});

// SelectMany — achatar listas de listas
var todasCategorias = produtos
    .SelectMany(p => p.Categorias)
    .Distinct();
```

### 3. OrderBy / OrderByDescending — Ordenar

```csharp
// Ordenar por preço (crescente)
var ordenados = produtos.OrderBy(p => p.Preco);

// Decrescente
var doMaior = produtos.OrderByDescending(p => p.Preco);

// Ordenação composta
var ordenados2 = produtos
    .OrderBy(p => p.Categoria)
    .ThenBy(p => p.Nome);
```

### 4. GroupBy — Agrupar

```csharp
// Agrupar produtos por categoria
var porCategoria = produtos.GroupBy(p => p.Categoria);

foreach (var grupo in porCategoria)
{
    Console.WriteLine($"Categoria: {grupo.Key}");
    Console.WriteLine($"  Itens: {grupo.Count()}");
    Console.WriteLine($"  Total: {grupo.Sum(p => p.Preco):C}");
}

// GroupBy com projeção
var resumoCategoria = produtos
    .GroupBy(p => p.Categoria)
    .Select(g => new 
    {
        Categoria = g.Key,
        Quantidade = g.Count(),
        PrecoMedio = g.Average(p => p.Preco),
        Total = g.Sum(p => p.Preco)
    });
```

### 5. Agregação — Sum, Average, Count, Min, Max

```csharp
decimal total = produtos.Sum(p => p.Preco);
decimal media = produtos.Average(p => p.Preco);
int quantidade = produtos.Count();
decimal max = produtos.Max(p => p.Preco);
decimal min = produtos.Min(p => p.Preco);

// Count com condição
int caros = produtos.Count(p => p.Preco > 100);

// Aggregate — acumulação personalizada
string lista = produtos
    .Select(p => p.Nome)
    .Aggregate((a, b) => $"{a}, {b}");
```

### 6. Quantificadores — Any, All, Contains

```csharp
// Any: pelo menos um satisfaz?
bool temCaro = produtos.Any(p => p.Preco > 1000);
bool temProduto = produtos.Any(); // não vazio?

// All: todos satisfazem?
bool todosBaratos = produtos.All(p => p.Preco < 100);

// Contains: contém elemento específico?
bool temArroz = produtos.Contains(produtoArroz);
```

### 7. First / FirstOrDefault / Single / SingleOrDefault

```csharp
// First: primeiro elemento (exceção se vazio)
var primeiro = produtos.First();

// FirstOrDefault: primeiro ou padrão (null para reference types)
var primeiroCaro = produtos.FirstOrDefault(p => p.Preco > 100);
// Retorna null se nenhum encontrado (não lança exceção!)

// Single: exatamente um (exceção se 0 ou >1)
var unico = produtos.Single(p => p.Codigo == "UNICO-001");

// SingleOrDefault: zero ou um (exceção se >1)
var talvez = produtos.SingleOrDefault(p => p.Codigo == "XYZ");
```

### 8. Skip / Take — Paginação

```csharp
// Paginação: página 2 com 10 itens por página
var pagina2 = produtos
    .Skip(20)   // Pular os primeiros 20
    .Take(10)   // Pegar os próximos 10
    .ToList();
```

### 9. Distinct / Union / Intersect / Except

```csharp
// Distinct: remover duplicatas
var categorias = produtos.Select(p => p.Categoria).Distinct();

// Union: combinar duas listas (sem duplicatas)
var todos = lista1.Union(lista2);

// Intersect: elementos em ambas
var comuns = lista1.Intersect(lista2);

// Except: elementos da primeira não na segunda
var diferenca = lista1.Except(lista2);
```

---

## Deferred Execution — Execução Diferida

**Conceito:** A query LINQ não é executada imediatamente. Ela é executada **quando você itera** sobre o resultado.

```csharp
// A query NÃO é executada aqui — apenas definida
var query = produtos.Where(p => p.Preco > 100);

// A query é executada AQUI — quando iteramos
foreach (var p in query)  // ← Execução aqui!
{
    Console.WriteLine(p.Nome);
}

// Forçar execução imediata com ToList(), ToArray(), Count(), etc.
var lista = query.ToList(); // ← Execução aqui!
```

### Por que isso importa?

```csharp
// ❌ Problema: query re-executada a cada iteração
var query = produtos.Where(p => p.Preco > 100);

Console.WriteLine(query.Count()); // Executa a query
Console.WriteLine(query.Count()); // Executa NOVAMENTE!

// ✅ Solução: materializar com ToList()
var lista = produtos.Where(p => p.Preco > 100).ToList();

Console.WriteLine(lista.Count); // Usa lista em memória
Console.WriteLine(lista.Count); // Usa lista em memória (rápido!)
```

---

## IEnumerable vs IQueryable

```
┌─────────────────────────────────────────────────────────────────┐
│  IEnumerable<T>                │  IQueryable<T>                 │
├────────────────────────────────────┼────────────────────────────┤
│  Execução em memória (LINQ to     │  Execução no banco de dados │
│  Objects)                         │  (LINQ to Entities/SQL)     │
├────────────────────────────────────┼────────────────────────────┤
│  Filtra dados JÁ carregados       │  Traduz query para SQL      │
├────────────────────────────────────┼────────────────────────────┤
│  Carrega TUDO do banco, depois    │  Carrega só o que atende ao │
│  filtra em memória                │  filtro                     │
├────────────────────────────────────┼────────────────────────────┤
│  Use para listas em memória       │  Use com Entity Framework   │
└─────────────────────────────────────────────────────────────────┘
```

```csharp
// IEnumerable: carrega TUDO, depois filtra
IEnumerable<Produto> todos = db.Produtos; // SELECT * FROM Produtos
var caros = todos.Where(p => p.Preco > 100); // Filtra em memória

// IQueryable: traduz para SQL, filtra no banco
IQueryable<Produto> query = db.Produtos; // Nada executado ainda
var caros2 = query.Where(p => p.Preco > 100); // Adiciona WHERE ao SQL
var lista = caros2.ToList(); // SELECT * FROM Produtos WHERE Preco > 100
```

---

## Exemplos Práticos com Dados de Vendas

```csharp
// Dados de exemplo
List<Venda> vendas = new()
{
    new() { Produto = "Arroz", Categoria = "Alimentos", Quantidade = 10, ValorUnitario = 15.90m, Data = new DateTime(2026, 8, 1) },
    new() { Produto = "Feijão", Categoria = "Alimentos", Quantidade = 5, ValorUnitario = 8.50m, Data = new DateTime(2026, 8, 15) },
    new() { Produto = "Notebook", Categoria = "Eletrônicos", Quantidade = 1, ValorUnitario = 3500m, Data = new DateTime(2026, 8, 20) },
    new() { Produto = "Mouse", Categoria = "Eletrônicos", Quantidade = 3, ValorUnitario = 89.90m, Data = new DateTime(2026, 8, 25) },
};

// 1. Total de vendas por categoria
var totalPorCategoria = vendas
    .GroupBy(v => v.Categoria)
    .Select(g => new 
    {
        Categoria = g.Key,
        Total = g.Sum(v => v.Quantidade * v.ValorUnitario),
        QuantidadeVendas = g.Count()
    });

// 2. Produtos com valor total > 100
var vendasGrandes = vendas
    .Where(v => v.Quantidade * v.ValorUnitario > 100)
    .OrderByDescending(v => v.Quantidade * v.ValorUnitario);

// 3. Ticket médio
var ticketMedio = vendas.Average(v => v.Quantidade * v.ValorUnitario);

// 4. Top 3 produtos mais vendidos (por quantidade)
var top3 = vendas
    .OrderByDescending(v => v.Quantidade)
    .Take(3)
    .Select(v => v.Produto);

// 5. Vendas do mês de agosto
var agosto = vendas
    .Where(v => v.Data.Month == 8 && v.Data.Year == 2026)
    .Sum(v => v.Quantidade * v.ValorUnitario);
```

---

## Resumo dos Operadores

| Categoria | Operadores |
|-----------|------------|
| **Filtrar** | `Where` |
| **Projetar** | `Select`, `SelectMany` |
| **Ordenar** | `OrderBy`, `OrderByDescending`, `ThenBy`, `Reverse` |
| **Agrupar** | `GroupBy` |
| **Agregar** | `Sum`, `Average`, `Count`, `Min`, `Max`, `Aggregate` |
| **Quantificar** | `Any`, `All`, `Contains` |
| **Elemento** | `First`, `FirstOrDefault`, `Single`, `SingleOrDefault`, `Last`, `ElementAt` |
| **Conjunto** | `Distinct`, `Union`, `Intersect`, `Except` |
| **Paginação** | `Skip`, `Take` |
| **Conversão** | `ToList`, `ToArray`, `ToDictionary`, `ToLookup` |

---

## Dicas de Performance

1. **Use `FirstOrDefault()` em vez de `First()`** quando não tiver certeza se há resultados
2. **Materialize com `ToList()`** se for iterar múltiplas vezes
3. **Filtre antes de ordenar** — menos elementos para ordenar
4. **Use `Any()` em vez of `Count() > 0`** — para verificar existência
5. **Cuidado com `SingleOrDefault()`** — verifica toda a coleção para garantir unicidade

---

*Anterior: [Coleções e Generics](./colecoes-linq.md) | Próximo: [Exemplo Prático — Análise de Vendas](./praticas/ExemploRelatorioVendas.cs)*
