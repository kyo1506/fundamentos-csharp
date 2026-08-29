# Pilares da Orientação a Objetos

> **Semana 2 — Teoria** | Tempo estimado: 20 minutos

---

## O que é OO?

Programação Orientada a Objetos (OO) é um paradigma que organiza código em **objetos** que combinam **estado** (dados) e **comportamento** (métodos). Os 4 pilares tornam esse organização robusta e flexível.

```
┌─────────────────────────────────────────────────────────────────┐
│                    PILARES DA OO                                │
├──────────────────┬──────────────────────────────────────────────┤
│  ENCAPSULAMENTO  │  Esconder estado interno, expor apenas      │
│                  │  o necessário via propriedades/métodos.      │
├──────────────────┼──────────────────────────────────────────────┤
│  HERANÇA         │  Reaproveitar código criando hierarquias    │
│                  │  de classes (é-um).                         │
├──────────────────┼──────────────────────────────────────────────┤
│  POLIMORFISMO    │  Tratar objetos de diferentes classes de    │
│                  │  forma uniforme via interfaces/base.         │
├──────────────────┼──────────────────────────────────────────────┤
│  ABSTRAÇÃO       │  Focar no essencial, ignorar detalhes       │
│                  │  de implementação.                          │
└──────────────────┴──────────────────────────────────────────────┘
```

---

## 1. Encapsulamento — Esconder para Proteger

**Conceito:** O estado interno de um objeto deve ser acessado apenas por métodos controlados, não diretamente.

```csharp
// ❌ Sem encapsulamento — qualquer um pode bagunçar
public class FormularioInscricao
{
    public string Cpf;        // Campo público = caos
    public int Idade;
}

// ✅ Com encapsulamento — validação no setter
public class FormularioInscricao
{
    private string _cpf;
    
    public string Cpf
    {
        get => _cpf;
        set
        {
            if (!ValidarCpf(value))
                throw new ArgumentException("CPF inválido");
            _cpf = value;
        }
    }
    
    public int Idade
    {
        get;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentException("Idade inválida");
        }
    }
}
```

**Por que importa?**
- Garante que o objeto sempre esteja em estado válido
- Permite mudar a implementação interna sem quebrar quem usa
- Centraliza regras de validação

---

## 2. Herança — Reaproveitar Código

**Conceito:** Uma classe pode herdar características de outra, criando uma relação "é-um".

```csharp
// Classe base
public abstract class Participante
{
    public string Nome { get; set; }
    public string Email { get; set; }
    
    public abstract decimal CalcularValorInscricao();
    
    public virtual string ExibirResumo() =>
        $"{Nome} ({Email})";
}

// Derivada: Palestrante
public class Palestrante : Participante
{
    public string Especialidade { get; set; }
    
    public override decimal CalcularValorInscricao() => 0m; // Gratuito
    
    public override string ExibirResumo() =>
        $"Palestrante: {Nome} — {Especialidade}";
}

// Derivada: Ouvinte
public class Ouvinte : Participante
{
    public bool Estudante { get; set; }
    
    public override decimal CalcularValorInscricao() =>
        Estudante ? 50m : 100m;
}

// Derivada: VIP
public class Vip : Participante
{
    public string CodigoAcesso { get; set; }
    
    public override decimal CalcularValorInscricao() => 200m;
}
```

**Quando usar herança:**
- Relação clara de "é-um" (Palestrante **é um** Participante)
- Reaproveitamento significativo de código
- Comportamento polimórfico necessário

**Quando NÃO usar:**
- Apenas para reutilizar código sem relação "é-um"
- Hierarquias muito profundas (>3 níveis)

---

## 3. Polimorfismo — Múltiplas Formas

**Conceito:** Objetos de classes derivadas podem ser tratados como a classe base, com comportamentos específicos.

```csharp
// Polimorfismo em ação
List<Participantes> participantes = new()
{
    new Palestrante { Nome = "Maria", Especialidade = "C#" },
    new Ouvinte { Nome = "João", Estudante = true },
    new Vip { Nome = "Carlos", CodigoAcesso = "VIP123" }
};

// Cada um calcula de um jeito, mas a chamada é uniforme
foreach (var p in participantes)
{
    decimal valor = p.CalcularValorInscricao();
    Console.WriteLine($"{p.ExibirResumo()}: R${valor}");
}
```

### `virtual` vs `override` vs `new`

```csharp
public class Base
{
    public virtual void Metodo() => Console.WriteLine("Base");
}

public class DerivadaOverride : Base
{
    public override void Metodo() => Console.WriteLine("Override");
}

public class DerivadaNew : Base
{
    public new void Metodo() => Console.WriteLine("New"); // Esconde o da base
}
```

| Situação | Comportamento |
|----------|---------------|
| `override` | Sempre chama a versão da derivada |
| `new` | Chama a base se referenciado como base |

---

## 4. Abstração — Focar no Essencial

**Conceito:** Definir contratos (interfaces ou classes abstratas) sem se preocupar com implementação.

### Classes Abstratas vs Interfaces

```
┌────────────────────────────┬────────────────────────────────┐
│   CLASSE ABSTRATA          │   INTERFACE                    │
├────────────────────────────┼────────────────────────────────┤
│ Pode ter implementação     │ Apenas contrato (C# 8+ pode    │
│ concreta                   │ ter implementação padrão)      │
├────────────────────────────┼────────────────────────────────┤
│ Herança simples            │ Múltiplas interfaces           │
│ (uma classe só)            │ simultâneas                    │
├────────────────────────────┼────────────────────────────────┤
│ Pode ter campos           │ Sem campos (apenas             │
│                           │ propriedades/métodos)          │
├────────────────────────────┼────────────────────────────────┤
│ "É-um"                    │ "Tem-capacidade-de"            │
└────────────────────────────┴────────────────────────────────┘
```

```csharp
// Interface: define capacidade
public interface IValidavel
{
    bool Validar();
    List<string> ObterErros();
}

public interface INotificavel
{
    void EnviarNotificacao(string mensagem);
}

// Classe abstrata: define base comum
public abstract class FormularioBase : IValidavel
{
    public DateTime DataCriacao { get; } = DateTime.Now;
    public bool Ativo { get; set; } = true;
    
    // Método abstrato: cada formulário implementa do seu jeito
    public abstract bool Validar();
    
    // Método concreto: compartilhado por todos
    public virtual List<string> ObterErros() => new();
}

// Implementação concreta
public class FormularioEvento : FormularioBase, INotificavel
{
    public string NomeEvento { get; set; }
    public int MaxParticipantes { get; set; }
    
    public override bool Validar()
    {
        return !string.IsNullOrEmpty(NomeEvento) 
            && MaxParticipantes > 0;
    }
    
    public void EnviarNotificacao(string mensagem)
    {
        Console.WriteLine($"Notificação: {mensagem}");
    }
}
```

---

## Princípios SOLID

SOLID são 5 princípios que complementam os pilares da OO:

```
┌─────────────────────────────────────────────────────────────────┐
│  S — Single Responsibility Principle (SRP)                      │
│      Uma classe deve ter APENAS UMA razão para mudar.           │
├─────────────────────────────────────────────────────────────────┤
│  O — Open/Closed Principle (OCP)                                │
│      Aberto para extensão, fechado para modificação.            │
├─────────────────────────────────────────────────────────────────┤
│  L — Liskov Substitution Principle (LSP)                        │
│      Derivadas devem poder substituir bases sem quebrar.        │
├─────────────────────────────────────────────────────────────────┤
│  I — Interface Segregation Principle (ISP)                      │
│      Interfaces pequenas e específicas > uma grande.            │
├─────────────────────────────────────────────────────────────────┤
│  D — Dependency Inversion Principle (DIP)                       │
│      Depender de abstrações, não de implementações.             │
└─────────────────────────────────────────────────────────────────┘
```

### SRP na prática

```csharp
// ❌ Violação: classe faz tudo
public class GerenciadorEvento
{
    public void CriarEvento() { }
    public void SalvarNoBanco() { }
    public void EnviarEmail() { }
    public void GerarRelatorio() { }
}

// ✅ SRP: cada classe tem uma responsabilidade
public class EventoService { public void CriarEvento() { } }
public class EventoRepository { public void Salvar() { } }
public class EmailService { public void Enviar() { } }
public class RelatorioService { public void Gerar() { } }
```

### OCP na prática

```csharp
// ✅ Aberto para extensão: adicionar novos validadores sem mudar existentes
public interface IValidador
{
    bool Validar(string valor);
}

public class ValidadorCpf : IValidador { /* ... */ }
public class ValidadorEmail : IValidador { /* ... */ }
public class ValidadorTelefone : IValidador { /* ... */ } // Novo, sem mudar os outros
```

### LSP na prática

```csharp
// ❌ Violação: Quebra expectativa
public class Pato
{
    public virtual void Voar() => Console.WriteLine("Voando...");
}

public class PatoDeBorracha : Pato
{
    public override void Voar() 
        => throw new NotImplementedException("Não voa!"); // ❌ Quebra LSP
}

// ✅ LSP: Mantém o contrato
public class PatoDeBorracha : Ave
{
    public override void Mover() => Console.WriteLine("Flutuando"); // ✅ Mantém expectativa
}
```

### ISP na prática

```csharp
// ❌ Interface gorda
public interface IRepositorio<T>
{
    void Adicionar(T item);
    void Remover(int id);
    T Buscar(int id);
    void RelatorioPDF();
    void RelatorioExcel();
}

// ✅ Interfaces segregadas
public interface IRepositorioLeitura<T>
{
    T Buscar(int id);
    IEnumerable<T> Listar();
}

public interface IRepositorioEscrita<T>
{
    void Adicionar(T item);
    void Remover(int id);
}

public interface IRelatorio
{
    void GerarPDF();
    void GerarExcel();
}
```

### DIP na prática

```csharp
// ❌ Depende de implementação concreta
public class EventoService
{
    private EventoRepository _repo = new(); // Acoplamento!
}

// ✅ Depende de abstração
public class EventoService
{
    private readonly IRepositorio<Evento> _repo;
    
    public EventoService(IRepositorio<Evento> repo) // Injeção de dependência
    {
        _repo = repo;
    }
}
```

---

## Resumo Visual

```
┌─────────────────────────────────────────────────────────────────┐
│                    OO + SOLID                                   │
├─────────────────────────────────────────────────────────────────┤
│  ENCAPSULAMENTO: protege estado interno                        │
│  HERANÇA: reusa código em hierarquias                           │
│  POLIMORFISMO: trata diferentes objetos uniformemente           │
│  ABSTRAÇÃO: foca no essencial, ignora detalhes                  │
├─────────────────────────────────────────────────────────────────┤
│  SRP: uma responsabilidade por classe                           │
│  OCP: extensível sem modificar existente                        │
│  LSP: derivadas substituem bases                                │
│  ISP: interfaces pequenas e específicas                         │
│  DIP: depende de abstrações                                     │
└─────────────────────────────────────────────────────────────────┘
```

---

*Anterior: [Stack vs Heap](../Semana-01/teoria/stack-heap.md) | Próximo: [SOLID em Detalhes](./solid.md)*
