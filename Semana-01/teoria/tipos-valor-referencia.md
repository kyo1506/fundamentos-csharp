# Tipos por Valor vs Tipos por Referência

> **Semana 1 — Teoria** | Tempo estimado: 20 minutos

---

## O que são "tipos"?

Em C#, todo dado tem um **tipo** que define:
- Quanta memória ocupa
- Quais valores pode assumir
- Quais operações podem ser feitas com ele

A divisão fundamental é:

```
┌─────────────────────────────────────────────────────────────┐
│                    TIPOS EM C#                              │
├────────────────────────┬────────────────────────────────────┤
│   TIPOS POR VALOR      │   TIPOS POR REFERÊNCIA             │
│   (Value Types)        │   (Reference Types)                │
├────────────────────────┼────────────────────────────────────┤
│ • int, double, bool    │ • class                            │
│ • char, decimal        │ • string                           │
│ • enum                 │ • array                            │
│ • struct               │ • interface                        │
│ • DateTime             │ • delegate                         │
├────────────────────────┼────────────────────────────────────┤
│ Guardam o VALOR        │ Guardam o ENDEREÇO do valor        │
│ diretamente na memória │ (ponteiro para a heap)             │
├────────────────────────┼────────────────────────────────────┤
│ Vivem na STACK         │ Referência na STACK                │
│                        │ Valor real na HEAP                 │
└────────────────────────┴────────────────────────────────────┘
```

---

## Tipos por Valor — Exemplos do Mercadinho

```csharp
// Struct: tipo por valor ideal para dados pequenos e imutáveis
public struct Dinheiro
{
    public decimal Valor { get; }
    public string Moeda { get; }
    
    public Dinheiro(decimal valor, string moeda = "BRL")
    {
        Valor = valor;
        Moeda = moeda;
    }
}

// Enum: tipo por valor para conjuntos fixos de valores
public enum UnidadeMedida
{
    Unidade,
    Kilograma,
    Litro
}

// Struct complexa: Produto do mercadinho
public struct ProdutoPerecivel
{
    public string Nome { get; set; }
    public string CodigoBarras { get; set; }
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public UnidadeMedida Unidade { get; set; }
}
```

### O que acontece na memória:

```
STACK (memória rápida, organizada)
┌──────────────────────────────────────┐
│  produto1: ProdutoPerecivel          │
│  ├─ Nome: "Arroz"         (string*)  │
│  ├─ CodigoBarras: "789"   (string*)  │
│  ├─ Preco: 15.90m         (decimal)  │
│  ├─ Quantidade: 100       (int)      │
│  └─ Unidade: Unidade      (enum)     │
│                                      │
│  * strings são referência, mas os    │
│    outros campos são valor           │
└──────────────────────────────────────┘
```

---

## Tipos por Referência — Exemplos do Mercadinho

```csharp
// Class: tipo por referência para entidades complexas
public class Estoque
{
    public string Nome { get; set; }
    public List<ProdutoPerecivel> Produtos { get; set; }
    public List<Transacao> Historico { get; set; }
    
    public Estoque(string nome)
    {
        Nome = nome;
        Produtos = new List<ProdutoPerecivel>();
        Historico = new List<Transacao>();
    }
}

// Outra class: histórico de transações
public class Transacao
{
    public DateTime Data { get; set; }
    public string Tipo { get; set; } // "Entrada" ou "Saída"
    public int Quantidade { get; set; }
    public ProdutoPerecivel Produto { get; set; }
}
```

### O que acontece na memória:

```
STACK                          HEAP (memória dinâmica)
┌─────────────────┐           ┌────────────────────────────────────┐
│ estoque: Estoque │──────────→│ Estoque object                     │
│ (referência)     │           │ ├─ Nome: "Mercadinho São José"    │
└─────────────────┘           │ ├─ Produtos: List<Produto>         │
                              │ │   ├─ [0] → ProdutoPerecivel      │
                              │ │   ├─ [1] → ProdutoPerecivel      │
                              │ │   └─ [2] → ProdutoPerecivel      │
                              │ └─ Historico: List<Transacao>      │
                              │     ├─ [0] → Transacao object      │
                              │     └─ [1] → Transacao object      │
                              └────────────────────────────────────┘
```

---

## A Diferença Crucial: Copiar vs Referenciar

### Com struct (tipo por valor):

```csharp
ProdutoPerecivel p1 = new ProdutoPerecivel 
{ 
    Nome = "Arroz", 
    Preco = 15.90m 
};

ProdutoPerecivel p2 = p1; // COPIA o valor inteiro
p2.Preco = 20.00m;       // Modifica apenas p2

Console.WriteLine(p1.Preco); // 15.90 — p1 não foi afetado!
Console.WriteLine(p2.Preco); // 20.00
```

```
STACK
┌─────────────────────┐     ┌─────────────────────┐
│ p1: ProdutoPerecivel │     │ p2: ProdutoPerecivel │
│ ├─ Nome: "Arroz"    │     │ ├─ Nome: "Arroz"    │
│ └─ Preco: 15.90     │     │ └─ Preco: 20.00     │  ← valor independente
└─────────────────────┘     └─────────────────────┘
```

### Com class (tipo por referência):

```csharp
Estoque e1 = new Estoque("Mercadinho");
Estoque e2 = e1;        // COPIA a referência (não o objeto!)
e2.Nome = "Supermercado"; // Modifica o objeto compartilhado

Console.WriteLine(e1.Nome); // "Supermercado" — e1 foi afetado!
Console.WriteLine(e2.Nome); // "Supermercado"
```

```
STACK                      HEAP
┌────────────┐            ┌──────────────────────┐
│ e1 ────────│───────────→│ Estoque object       │
└────────────┘            │ └─ Nome: "Supermercado"│ ← mesmo objeto!
┌────────────┐            └──────────────────────┘
│ e2 ────────│───────────→ (mesma referência)
└────────────┘
```

---

## Quando Usar Cada Um?

| Situação | Recomendação | Por quê? |
|----------|--------------|----------|
| Dados pequenos e imutáveis (preço, coordenada, cor) | `struct` | Sem overhead de alocação na heap |
| Entidade com identidade própria (cliente, pedido) | `class` | Referência permite compartilhar estado |
| Coleção de itens | `class` | Arrays e List sempre são referência |
| Interface para comportamento | `interface` | Sempre referência |
| Valor padrão deve ser zero/vazio | `struct` | Struct não pode ser null (sem Nullable<T>) |
| Precisa de herança | `class` | Struct não suporta herança |

---

## Resumo Visual

```
┌─────────────────────────────────────────────────────────────┐
│                     TIPO POR VALOR                          │
│  • Valor copiado em atribuições                             │
│  • Cada variável tem sua própria cópia                      │
│  • Vive na stack (mais rápido)                              │
│  • Não pode ser null (sem Nullable<T>)                      │
│  • Ex: int, double, bool, struct, enum, DateTime            │
├─────────────────────────────────────────────────────────────┤
│                   TIPO POR REFERÊNCIA                       │
│  • Referência copiada em atribuições                        │
│  • Múltiplas variáveis podem apontar para o mesmo objeto    │
│  • Referência na stack, objeto na heap                      │
│  • Pode ser null                                            │
│  • Ex: class, string, array, interface, delegate            │
└─────────────────────────────────────────────────────────────┘
```

---

## Exercício Rápido de Fixação

O que será impresso?

```csharp
int a = 10;
int b = a;
b = 20;
Console.WriteLine($"a={a}, b={b}");

var lista1 = new List<int> { 1, 2, 3 };
var lista2 = lista1;
lista2.Add(4);
Console.WriteLine($"lista1.Count={lista1.Count}, lista2.Count={lista2.Count}");
```

<details>
<summary>Resposta</summary>

```
a=10, b=20        ← int é valor, b foi copiado independente
lista1.Count=4, lista2.Count=4  ← List é referência, ambas apontam para a mesma lista
```

</details>

---

*Próximo: [Stack vs Heap](./stack-heap.md)*
