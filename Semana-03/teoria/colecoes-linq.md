# Coleções e Generics — Armazenando Dados em Escala

> **Semana 3 — Teoria** | Tempo estimado: 20 minutos

---

## Por que coleções?

Quando temos 1 produto, usamos uma variável. Quando temos 1.000 produtos? **Coleções**.

```
┌─────────────────────────────────────────────────────────────────┐
│                    COLEÇÕES EM C#                               │
├──────────────────────────┬──────────────────────────────────────┤
│   NÃO-GENÉRICAS          │   GENÉRICAS (Recomendadas)           │
│   (System.Collections)   │   (System.Collections.Generic)       │
├──────────────────────────┼──────────────────────────────────────┤
│ • ArrayList              │ • List<T>                            │
│ • Hashtable              │ • Dictionary<TKey, TValue>           │
│ • Queue                  │ • HashSet<T>                         │
│ • Stack                  │ • Queue<T>                           │
│ • SortedList             │ • Stack<T>                           │
├──────────────────────────┼──────────────────────────────────────┤
│ Armazenam object         │ Armazenam T (tipo específico)        │
│ (boxing!)                │ (sem boxing!)                        │
├──────────────────────────┼──────────────────────────────────────┤
│ Tipo seguro em runtime   │ Tipo seguro em compile-time          │
└──────────────────────────┴──────────────────────────────────────┘
```

---

## List<T> — A Coleção Mais Usada

```csharp
// Genérico: tipo seguro + sem boxing
List<Produto> produtos = new List<Produto>();
produtos.Add(new Produto { Nome = "Arroz", Preco = 15.90m });
produtos.Add(new Produto { Nome = "Feijão", Preco = 8.50m });

// Acesso por índice
Produto primeiro = produtos[0];

// Iteração
foreach (var p in produtos)
    Console.WriteLine(p.Nome);

// Não-genérico: boxing + tipo não-seguro
ArrayList lista = new ArrayList();
lista.Add(42);        // int é "embalado" (boxing) para object
lista.Add("texto");   // Pode adicionar qualquer coisa!
int valor = (int)lista[0]; // Unboxing + risco de InvalidCastException
```

### Operações comuns de List<T>

```csharp
List<int> numeros = new() { 5, 2, 8, 1, 9 };

// Adicionar/Remover
numeros.Add(3);
numeros.Remove(5);
numeros.RemoveAt(0);
numeros.Sort();
numeros.Reverse();

// Buscar
bool temOito = numeros.Contains(8);  // true
int index = numeros.IndexOf(8);       // 3

// Capacidade
numeros.TrimExcess(); // Libera memória não usada
```

---

## Dictionary<TKey, TValue> — Busca Rápida

**Quando usar:** Precisa buscar por chave (não por índice).

```csharp
// Chave → Valor (busca O(1) em média)
Dictionary<int, Produto> catalogo = new();
catalogo.Add(1, new Produto { Nome = "Arroz", Preco = 15.90m });
catalogo.Add(2, new Produto { Nome = "Feijão", Preco = 8.50m });

// Busca instantânea por chave
Produto produto = catalogo[1]; // O(1) — muito rápido!

// Verificar se chave existe
if (catalogo.ContainsKey(1))
{
    Console.WriteLine(catalogo[1].Nome);
}

// Iteração
foreach (var kvp in catalogo)
{
    Console.WriteLine($"Chave: {kvp.Key}, Valor: {kvp.Value.Nome}");
}
```

### Comparação de performance

```
┌──────────────────────────────────────────────────────────────┐
│  Operação         │  List<T>        │  Dictionary<K,V>      │
├───────────────────┼─────────────────┼────────────────────────┤
│  Add()            │  O(1) amort.    │  O(1) amort.          │
│  Busca por índice │  O(1)           │  N/A                  │
│  Busca por valor  │  O(n)           │  O(1) por chave       │
│  Remove           │  O(n)           │  O(1)                 │
└──────────────────────────────────────────────────────────────┘
```

---

## HashSet<T> — Conjunto Sem Duplicatas

```csharp
// Garante unicidade
HashSet<string> categorias = new();
categorias.Add("Eletrônicos");
categorias.Add("Roupas");
categorias.Add("Eletrônicos"); // Ignadado — já existe!

Console.WriteLine(categorias.Count); // 2

// Operações de conjunto
HashSet<int> a = new() { 1, 2, 3, 4 };
HashSet<int> b = new() { 3, 4, 5, 6 };

a.IntersectWith(b); // a = { 3, 4 }
a.UnionWith(b);     // a = { 3, 4, 5, 6 }
a.ExceptWith(b);    // a = {}
```

---

## Queue<T> e Stack<T> — Estruturas LIFO/FIFO

```csharp
// Queue: First In, First Out (Fila)
Queue<string> fila = new();
fila.Enqueue("Primeiro");
fila.Enqueue("Segundo");
fila.Enqueue("Terceiro");

string proximo = fila.Dequeue(); // "Primeiro"

// Stack: Last In, First Out (Pilha)
Stack<string> pilha = new();
pilha.Push("Primeiro");
pilha.Push("Segundo");
pilha.Push("Terceiro");

string topo = pilha.Pop(); // "Terceiro"
```

---

## Generics — Por que `List<T>` é melhor que `ArrayList`?

### O problema: boxing em coleções não-genéricas

```csharp
// ArrayList: tudo é object
ArrayList lista = new ArrayList();
lista.Add(42);        // Boxing! int → object
lista.Add(3.14);      // Boxing! double → object

int soma = (int)lista[0] + (int)lista[1]; // Unboxing + erro em runtime!
```

### A solução: Generics

```csharp
// List<T>: tipo seguro em compile-time
List<int> numeros = new List<int>();
numeros.Add(42);      // Sem boxing!
numeros.Add(100);

int soma = numeros[0] + numeros[1]; // Sem unboxing, sem erro
```

### Criando seus próprios Generics

```csharp
// Interface genérica
public interface IRepositorio<T> where T : class
{
    void Adicionar(T item);
    T? Buscar(int id);
    IEnumerable<T> Listar();
}

// Classe genérica
public class RepositorioMemoria<T> : IRepositorio<T> where T : class
{
    private readonly List<T> _items = new();
    
    public void Adicionar(T item) => _items.Add(item);
    public T? Buscar(int id) => id < _items.Count ? _items[id] : null;
    public IEnumerable<T> Listar() => _items.AsReadOnly();
}

// Uso
var repoProdutos = new RepositorioMemoria<Produto>();
var repoClientes = new RepositorioMemoria<Cliente>();
```

---

## Quando Usar Cada Coleção

| Situação | Coleção recomendada |
|----------|---------------------|
| Lista simples de itens | `List<T>` |
| Busca rápida por chave | `Dictionary<K,V>` |
| Evitar duplicatas | `HashSet<T>` |
| Fila de processamento | `Queue<T>` |
| Histórico/desfazer | `Stack<T>` |
| Cache com acesso frequente | `Dictionary<K,V>` |
| Ordenação automática | `SortedList<K,V>` |

---

## Resumo Visual

```
┌─────────────────────────────────────────────────────────────────┐
│                    COLEÇÕES GENÉRICAS                           │
├─────────────────────────────────────────────────────────────────┤
│  List<T>          │  Lista indexada, permite duplicatas        │
│  Dictionary<K,V>  │  Pares chave-valor, busca O(1)             │
│  HashSet<T>       │  Conjunto sem duplicatas                   │
│  Queue<T>         │  FIFO (primeiro a entrar, primeiro a sair) │
│  Stack<T>         │  LIFO (último a entrar, primeiro a sair)   │
└─────────────────────────────────────────────────────────────────┘
```

---

*Anterior: [SOLID](../Semana-02/teoria/solid.md) | Próximo: [LINQ — Consultas Declarativas](./linq.md)*
