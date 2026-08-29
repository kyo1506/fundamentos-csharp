# Stack vs Heap — Como a Memória Funciona

> **Semana 1 — Teoria** | Tempo estimado: 20 minutos

---

## Por que importa entender memória?

Quando você entende **onde** seus dados vivem na memória, consegue:
- Prever o comportamento do código (por que uma variável muda e outra não?)
- Evitar problemas de performance (alocações desnecessárias)
- Debugar problemas complexos com confiança
- Entender tipos por valor vs referência de verdade

---

## As Duas Regiões de Memória

```
┌─────────────────────────────────────────────────────────────────┐
│                    MEMÓRIA DO PROGRAMA                          │
├────────────────────────────┬────────────────────────────────────┤
│         STACK              │              HEAP                  │
│         (Pilha)            │           (Monte)                  │
├────────────────────────────┼────────────────────────────────────┤
│ • Rápida como fogo         │ • Mais lenta                       │
│ • Organizada (LIFO)        │ • Desorganizada                    │
│ • Tamanho fixo por variável│ • Tamanho dinâmico                 │
│ • Aloca/desaloca automática│ • Coletada pelo GC (Garbage Coll.) │
│ • Tipos por valor          │ • Tipos por referência             │
│ • Escopo definido          │ • Vive enquanto houver referência  │
└────────────────────────────┴────────────────────────────────────┘
```

---

## A Stack — A Pilha de Pratos

Pense na stack como uma **pilha de pratos**:
- Você só coloca pratos no **topo**
- Você só tira pratos do **topo**
- LIFO: Last In, First Out (último a entrar, primeiro a sair)

```
STACK — Execução do método CalcularTotal()

┌──────────────────────────────────────────┐
│  Endereço  │  Conteúdo                   │
├──────────────────────────────────────────┤
│  0x1000    │  retorno: decimal (espaço)  │  ← topo da stack
│  0x0FF8    │  total: decimal = 0.0m      │
│  0x0FF0    │  i: int = 0                 │
│  0x0FE8    │  produtos: referência ------│──→ (aponta para heap)
│  0x0FE0    │  parâmetro: método          │
└──────────────────────────────────────────┘
     ↑
  Stack Pointer (aponta para o topo)
```

### Características da Stack:

1. **Alocação instantânea** — mover o stack pointer é uma operação de CPU
2. **Desalocação automática** — quando o método termina, tudo é removido
3. **Tamanho limitado** — geralmente 1MB por thread (pode estourar com recursão infinita)
4. **Thread-safe** — cada thread tem sua própria stack

---

## A Heap — A Mesa de Trabalho

Pense na heap como uma **mesa grande**:
- Você coloca coisas **em qualquer lugar**
- O Garbage Collector (GC) limpa o que não está mais sendo usado
- Objetos podem ficar espalhados (fragmentação)

```
HEAP — Objetos alocados durante a execução

┌─────────────────────────────────────────────────────────────┐
│  Endereço    │  Conteúdo                                   │
├─────────────────────────────────────────────────────────────┤
│  0xA000      │  List<ProdutoPerecivel>                     │
│              │  ├─ Capacity: 4                              │
│              │  ├─ Count: 3                                 │
│              │  └─ _items: array ──────────────────────────│──→
│  0xA040      │  ProdutoPerecivel[0] — "Arroz"              │
│  0xA050      │  ProdutoPerecivel[1] — "Feijão"             │
│  0xA060      │  ProdutoPerecivel[2] — "Macarrão"           │
│  0xA070      │  string "Arroz" (interned)                  │
│  0xA080      │  string "Feijão" (interned)                 │
│  0xA090      │  string "Macarrão" (interned)               │
│  ...         │  (espaço livre para novos objetos)          │
└─────────────────────────────────────────────────────────────┘
```

### Características da Heap:

1. **Alocação mais cara** — precisa encontrar espaço livre
2. **Coleta automática** — GC remove objetos sem referência
3. **Tamanho grande** — limitado pela memória do sistema
4. **Compartilhada** — todas as threads acessam a mesma heap
5. **Fragmentação** — objetos podem ficar espalhados

---

## A Dança Stack + Heap na Prática

### Cenário: Criando um produto no estoque

```csharp
// Este código executa em um método
public void AdicionarProduto()
{
    // 1. Struct é criada DIRETAMENTE na stack
    ProdutoPerecivel produto = new ProdutoPerecivel
    {
        Nome = "Arroz",
        Preco = 15.90m
    };
    
    // 2. Class requer alocação na heap
    List<ProdutoPerecivel> lista = new List<ProdutoPerecivel>();
    //    ↑ referência na stack
    //    ↑ objeto na heap
    
    // 3. Adicionar struct à lista
    lista.Add(produto);
    //    ↑ struct é COPIADO para dentro do array interno da lista (na heap)
}
```

### Visualização passo a passo:

```
PASSO 1: Produto criado na stack
┌─────────────────────────────┐
│ STACK                       │
│ ┌─────────────────────────┐ │
│ │ produto: ProdutoPerecivel│ │
│ │ ├─ Nome: "Arroz" ───────│─┼──→ string na heap (interned)
│ │ ├─ Preco: 15.90m       │ │
│ │ └─ Quantidade: 0       │ │
│ └─────────────────────────┘ │
└─────────────────────────────┘

PASSO 2: Lista criada (referência stack → objeto heap)
┌─────────────────────────────┐     ┌─────────────────────────────┐
│ STACK                       │     │ HEAP                        │
│ ┌─────────────────────────┐ │     │ ┌─────────────────────────┐ │
│ │ produto: ProdutoPerecivel│ │     │ │ List<ProdutoPerecivel>  │ │
│ │ ├─ Nome: "Arroz"       │ │     │ │ ├─ _items: array ───────┼─┼──→
│ │ └─ Preco: 15.90m       │ │     │ │ ├─ Capacity: 4         │ │
│ ├─────────────────────────┤ │     │ │ └─ Count: 0            │ │
│ │ lista: referência ──────│─┼────→│ └─────────────────────────┘ │
│ └─────────────────────────┘ │     └─────────────────────────────┘
└─────────────────────────────┘

PASSO 3: produto.Add(produto) — struct é copiado para o array na heap
┌─────────────────────────────┐     ┌─────────────────────────────┐
│ STACK                       │     │ HEAP                        │
│ ┌─────────────────────────┐ │     │ ┌─────────────────────────┐ │
│ │ produto: ProdutoPerecivel│ │     │ │ List<ProdutoPerecivel>  │ │
│ │ ├─ Nome: "Arroz"       │ │     │ │ ├─ _items: array ───────┼─┼──→
│ │ └─ Preco: 15.90m       │ │     │ │ │  ├─ [0]: CÓPIA do      │ │
│ ├─────────────────────────┤ │     │ │ │  │   produto (15.90m)   │ │
│ │ lista: referência ──────│─┼────→│ │ ├─ Capacity: 4         │ │
│ └─────────────────────────┘ │     │ │ └─ Count: 1            │ │
└─────────────────────────────┘     │ └─────────────────────────────┘
                                    │ ┌─────────────────────────┐ │
                                    │ │ Array interno:          │ │
                                    │ │ [0]: ProdutoPerecivel   │ │
                                    │ │ [1]: (vazio)            │ │
                                    │ │ [2]: (vazio)            │ │
                                    │ │ [3]: (vazio)            │ │
                                    │ └─────────────────────────┘ │
                                    └─────────────────────────────┘
```

---

## Garbage Collector — O Faxineiro

O GC é o responsável por limpar a heap. Ele funciona em gerações:

```
┌─────────────────────────────────────────────────────────────┐
│                    GERAÇÕES DO GC                           │
├─────────────────────────────────────────────────────────────┤
│  Gen 0: Objetos recém-criados (coletados frequentemente)    │
│  Gen 1: Objetos que sobreviveram a uma coleta               │
│  Gen 2: Objetos de longa vida (coletados raramente)         │
│  Large Object Heap (LOH): Objetos > 85.000 bytes            │
└─────────────────────────────────────────────────────────────┘
```

### Quando o GC atinge um objeto:

```csharp
Estoque e1 = new Estoque("Mercadinho");
e1 = null; // O objeto Estoque na heap agora está "órfão"

// Em algum momento, o GC vai:
// 1. Detectar que não há mais referências para o objeto
// 2. Marcar o objeto como "lixo"
// 3. Liberar a memória ocupada
```

---

## Boxing e Unboxing — O Custo Oculto

**Boxing** é quando um tipo por valor é "embalado" para a heap:

```csharp
// Sem boxing — rápido
int numero = 42;
double valor = 3.14;

// Com boxing — lento (aloca na heap!)
object obj1 = 42;        // int → object (boxing)
object obj2 = 3.14;      // double → object (boxing)

// Unboxing — também tem custo
int deVolta = (int)obj1; // object → int (unboxing)
```

### Por que é lento?

```
STACK                    HEAP
┌──────────────┐         ┌──────────────────────┐
│ obj1: ref ───│────────→│ System.Int32 boxed   │
└──────────────┘         │ Valor: 42            │
                         │ Tipo: System.Int32   │
                         │ SyncBlock            │
                         │ VTable               │
                         └──────────────────────┘
                         
Custo: alocação na heap + cópia do valor + overhead de tipo
```

### Exemplo prático com performance:

```csharp
// RÁPIDO: sem boxing
var listaOtimizada = new List<int>();
for (int i = 0; i < 1_000_000; i++)
{
    listaOtimizada.Add(i); // int vai direto para o array interno
}

// LENTO: com boxing
var listaLenta = new ArrayList();
for (int i = 0; i < 1_000_000; i++)
{
    listaLenta.Add(i); // cada int é "embalado" (boxing) para a heap!
}
```

---

## `ref`, `out` e `in` — Passando Referências Explicitamente

### `ref` — Passa a referência da variável

```csharp
public void AplicarDesconto(ref ProdutoPerecivel produto, decimal percentual)
{
    produto.Preco *= (1 - percentual / 100);
    // Modifica o ORIGINAL, não uma cópia
}

// Uso:
ProdutoPerecivel arroz = new ProdutoPerecivel { Preco = 20.00m };
AplicarDesconto(ref arroz, 10);
Console.WriteLine(arroz.Preco); // 18.00 — original modificado!
```

### `out` — Retorna múltiplos valores

```csharp
public bool TentarBuscarProduto(string codigo, out ProdutoPerecivel produto)
{
    if (codigo == "123")
    {
        produto = new ProdutoPerecivel { Nome = "Arroz", Preco = 15.90m };
        return true;
    }
    produto = default;
    return false;
}

// Uso:
if (TentarBuscarProduto("123", out var produto))
{
    Console.WriteLine(produto.Nome); // "Arroz"
}
```

### `in` — Referência somente leitura

```csharp
public decimal CalcularValorTotal(in ProdutoPerecivel produto, int quantidade)
{
    // produto não pode ser modificado aqui
    return produto.Preco * quantidade;
}
```

---

## Resumo Visual Completo

```
┌─────────────────────────────────────────────────────────────────┐
│                    STACK                                        │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • Variáveis locais                                       │  │
│  │ • Referências (ponteiros para heap)                      │  │
│  │ • Tipos por valor (structs, enums, primitivos)           │  │
│  │ • Parâmetros de método                                    │  │
│  │ • Endereço de retorno                                     │  │
│  └───────────────────────────────────────────────────────────┘  │
├─────────────────────────────────────────────────────────────────┤
│                    HEAP                                         │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • Objetos de classe                                       │  │
│  │ • Arrays (mesmo que de structs)                           │  │
│  │ • Strings                                                 │  │
│  │ • Objetos boxed                                           │  │
│  │ • Delegates                                               │  │
│  └───────────────────────────────────────────────────────────┘  │
├─────────────────────────────────────────────────────────────────┤
│                    GC (Garbage Collector)                       │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ • Limpa objetos sem referência                            │  │
│  │ • Compacta a heap (defragmentação)                        │  │
│  │ • Gerações: 0, 1, 2 + LOH                                 │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Analogias para Fixar

| Conceito | Analogia |
|----------|----------|
| **Stack** | Pilha de pratos — só coloca/tira do topo |
| **Heap** | Mesa de trabalho — espalha coisas, depois limpa |
| **Referência** | Endereço de uma casa — você guarda o endereço, não a casa |
| **Valor** | O objeto em si — você tem o carro, não a placa |
| **Boxing** | Colocar um objeto pequeno numa caixa grande para enviar |
| **GC** | Faxineiro que joga fora o que não está mais sendo usado |

---

## Exercício Rápido

O que será impresso?

```csharp
int x = 5;
int y = x;
y = 10;
Console.WriteLine($"x={x}, y={y}");

var listaA = new List<int> { 1, 2 };
var listaB = listaA;
listaB.Add(3);
Console.WriteLine($"A.Count={listaA.Count}, B.Count={listaB.Count}");

var struct1 = new Point { X = 1, Y = 2 };
var struct2 = struct1;
struct2.X = 10;
Console.WriteLine($"struct1.X={struct1.X}, struct2.X={struct2.X}");
```

<details>
<summary>Resposta</summary>

```
x=5, y=10                    ← int é valor, independente
A.Count=3, B.Count=3         ← List é referência, mesma lista
struct1.X=1, struct2.X=10    ← struct é valor, cópia independente
```

</details>

---

*Anterior: [Tipos por Valor vs Referência](./tipos-valor-referencia.md)*
