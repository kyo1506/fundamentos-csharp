# SOLID — Princípios para Código Limpo

> **Semana 2 — Teoria** | Tempo estimado: 20 minutos

---

## O que é SOLID?

SOLID são 5 princípios que tornam código orientado a objetos mais **manutenível**, **extensível** e **testável**. Eles complementam os 4 pilares da OO.

```
┌─────────────────────────────────────────────────────────────────┐
│  S — Single Responsibility Principle (SRP)                      │
│      "Uma classe deve ter apenas uma razão para mudar."         │
│      Robert C. Martin                                           │
├─────────────────────────────────────────────────────────────────┤
│  O — Open/Closed Principle (OCP)                                │
│      "Entidades de software devem estar abertas para            │
│       extensão, mas fechadas para modificação."                 │
├─────────────────────────────────────────────────────────────────┤
│  L — Liskov Substitution Principle (LSP)                        │
│      "Se S é um subtipo de T, objetos do tipo T podem           │
│       ser substituídos por objetos do tipo S sem quebrar."      │
├─────────────────────────────────────────────────────────────────┤
│  I — Interface Segregation Principle (ISP)                      │
│      "Clientes não devem ser forçados a depender de             │
│       interfaces que não usam."                                 │
├─────────────────────────────────────────────────────────────────┤
│  D — Dependency Inversion Principle (DIP)                       │
│      "Módulos de alto nível não devem depender de               │
│       módulos de baixo nível. Ambos devem depender de           │
│       abstrações."                                              │
└─────────────────────────────────────────────────────────────────┘
```

---

## S — Single Responsibility Principle (SRP)

### O problema: classe fazendo tudo

```csharp
// ❌ God Class: sabe demais, faz demais, muda por qualquer motivo
public class FormularioInscricaoService
{
    // Validação
    public bool ValidarCpf(string cpf) { /* 20 linhas */ }
    public bool ValidarEmail(string email) { /* 15 linhas */ }
    
    // Persistência
    public void SalvarNoBanco(Formulario f) { /* SQL aqui */ }
    public Formulario BuscarDoBanco(int id) { /* mais SQL */ }
    
    // Notificação
    public void EnviarEmailConfirmacao(string email) { /* SMTP */ }
    public void EnviarSms(string telefone) { /* API externa */ }
    
    // Relatório
    public byte[] GerarPdf(Formulario f) { /* iTextSharp */ }
    public byte[] GerarExcel(Formulario f) { /* EPPlus */ }
    
    // Regra de negócio
    public decimal CalcularDesconto(Formulario f) { /* ... */ }
}
```

**Problemas:**
- Muda quando muda a regra de CPF → modifica esta classe
- Muda quando muda o banco → modifica esta classe
- Muda quando muda o provedor de email → modifica esta classe
- Impossível testar isoladamente
- Ninguém entende essa classe inteira

### A solução: separar responsabilidades

```csharp
// ✅ SRP: Cada classe tem UMA responsabilidade

public class ValidadorCpf
{
    public bool Validar(string cpf) { /* ... */ }
}

public class ValidadorEmail
{
    public bool Validar(string email) { /* ... */ }
}

public class FormularioRepository
{
    public void Salvar(Formulario f) { /* ... */ }
    public Formulario Buscar(int id) { /* ... */ }
}

public class EmailService
{
    public void EnviarConfirmacao(string email) { /* ... */ }
}

public class FormularioReport
{
    public byte[] GerarPdf(Formulario f) { /* ... */ }
    public byte[] GerarExcel(Formulario f) { /* ... */ }
}

public class CalculadoraDesconto
{
    public decimal Calcular(Formulario f) { /* ... */ }
}

// ✅ Orquestrador: coordena as dependências
public class FormularioInscricaoService
{
    private readonly ValidadorCpf _validadorCpf;
    private readonly ValidadorEmail _validadorEmail;
    private readonly FormularioRepository _repository;
    private readonly EmailService _emailService;
    
    public FormularioInscricaoService(
        ValidadorCpf validadorCpf,
        ValidadorEmail validadorEmail,
        FormularioRepository repository,
        EmailService emailService)
    {
        _validadorCpf = validadorCpf;
        _validadorEmail = validadorEmail;
        _repository = repository;
        _emailService = emailService;
    }
    
    public void ProcessarInscricao(Formulario f)
    {
        _validadorCpf.Validar(f.Cpf);
        _validadorEmail.Validar(f.Email);
        _repository.Salvar(f);
        _emailService.EnviarConfirmacao(f.Email);
    }
}
```

### Como identificar violações de SRP?

| Sinal | Pergunta |
|-------|----------|
| Classe tem muitos métodos | "O que essa classe faz?" — se a resposta tem "e", tem SRP violado |
| Muda por muitos motivos | "Por que essa classe mudou?" — se há mais de uma razão, tem SRP violado |
| Depende de muitas coisas | >5 dependências geralmente indicam SRP violado |
| Ninguém entende sozinha | Se precisa de >1 pessoa para explicar, está grande demais |

---

## O — Open/Closed Principle (OCP)

### O problema: modificar para estender

```csharp
// ❌ Toda vez que adicionar um tipo de participante, muda aqui
public class CalculadoraInscricao
{
    public decimal Calcular(string tipoParticipante)
    {
        switch (tipoParticipante)
        {
            case "Estudante": return 50m;
            case "Profissional": return 150m;
            case "VIP": return 250m;
            // case "Idoso": ... // Precisa MODIFICAR esta classe!
            default: throw new ArgumentException();
        }
    }
}
```

### A solução: aberto para extensão

```csharp
// ✅ OCP: estender sem modificar

// Contrato aberto
public interface ICalculadoraInscricao
{
    decimal Calcular();
    bool AplicaSe(Participante participante);
}

// Implementações específicas
public class InscricaoEstudante : ICalculadoraInscricao
{
    public decimal Calcular() => 50m;
    public bool AplicaSe(Participante p) => p.Estudante;
}

public class InscricaoProfissional : ICalculadoraInscricao
{
    public decimal Calcular() => 150m;
    public bool AplicaSe(Participante p) => !p.Estudante;
}

public class InscricaoVip : ICalculadoraInscricao
{
    public decimal Calcular() => 250m;
    public bool AplicaSe(Participante p) => p.IsVip;
}

// Orquestrador: NUNCA muda para novos tipos
public class ServicoInscricao
{
    private readonly List<ICalculadoraInscricao> _calculadoras;
    
    public ServicoInscricao(List<ICalculadoraInscricao> calculadoras)
    {
        _calculadoras = calculadoras;
    }
    
    public decimal CalcularValor(Participante participante)
    {
        return _calculadoras
            .First(c => c.AplicaSe(participante))
            .Calcular();
    }
}

// Adicionar Idoso? Apenas cria uma nova classe:
public class InscricaoIdoso : ICalculadoraInscricao
{
    public decimal Calcular() => 25m; // Desconto de 50%
    public bool AplicaSe(Participante p) => p.Idade >= 60;
}
// Nenhuma classe existente foi modificada! ✅
```

---

## L — Liskov Substitution Principle (LSP)

### O problema: herança que quebra expectativa

```csharp
// ❌ LSP violado: PatoDeBorracha precisa "voar", mas não voa
public abstract class Ave
{
    public abstract void Voar();
    public abstract void Comer();
}

public class PatoDeBorracha : Ave
{
    public override void Voar() => 
        throw new NotImplementedException(); // ❌ Não voa!
    
    public override void Comer() => 
        throw new NotImplementedException(); // ❌ Não come!
}

// ❌ Quebra o código
Ave ave = new PatoDeBorracha();
ave.Voor(); // 💥 Exceção!
```

### A solução: manter o contrato

```csharp
// ✅ LSP: contratos pequenos e específicos

public interface IVoador
{
    void Voar();
}

public interface IComedor
{
    void Comer();
}

public interface IAve
{
    string Especie { get; }
}

public class Pato : IAve, IVoador, IComedor
{
    public string Especie => "Pato-real";
    public void Voar() => Console.WriteLine("Voando baixo...");
    public void Comer() => Comendo ração...");
}

public class PatoDeBorracha : IAve
{
    public string Especie => "Pato de borracha (brinquedo)";
    // Não implementa IVoador porque não voa! ✅
}

// Uso correto:
IAve ave = GetAve();
if (ave is IVoador voador)
    voador.Voor(); // Só chama se realmente voa!
```

### Como identificar violações de LSP?

| Sinal | Explicação |
|-------|------------|
| `throw new NotImplementedException()` | Classe derivada não honra o contrato |
| `if (obj is TipoDerivado)` | Código verifica tipo concreto, quebrando polimorfismo |
| Cliente evita métodos da base | Se o código cliente ignora métodos, o design está errado |
| Derivada é mais restritiva que base | Se a derivada faz menos que a base, tem problema |

---

## I — Interface Segregation Principle (ISP)

### O problema: interface gorda

```csharp
// ❌ Interface com métodos que nem todos usam
public interface IRepositorio<T>
{
    void Adicionar(T item);
    void Remover(int id);
    void Atualizar(T item);
    T Buscar(int id);
    IEnumerable<T> Listar();
    void RelatorioPDF();
    void RelatorioExcel();
    void ExportarCSV();
    void ImportarJSON();
}

// ❌ Leitura forçada a implementar escrita
public class RepositorioSomenteLeitura<T> : IRepositorio<T>
{
    public void Adicionar(T item) => throw new NotSupportedException();
    public void Remover(int id) => throw new NotSupportedOperation();
    public void Atualizar(T item) => throw new NotSupportedOperation();
    // ...
}
```

### A solução: interfaces pequenas

```csharp
// ✅ ISP: interfaces específicas e coesas

public interface ILeitura<T>
{
    T Buscar(int id);
    IEnumerable<T> Listar();
    IEnumerable<T> BuscarPor(Func<T, bool> predicado);
}

public interface IEscrita<T>
{
    void Adicionar(T item);
    void Remover(int id);
    void Atualizar(T item);
}

public interface IRelatorio
{
    byte[] GerarPDF();
    byte[] GerarExcel();
    string GerarCSV();
}

// Cada classe implementa APENAS o que precisa
public class EventoRepositorio : ILeitura<Evento>, IEscrita<Evento>
{
    // Implementa leitura e escrita
}

public class RelatorioService : IRelatorio
{
    // Implementa apenas relatórios
}

public class ApiLeitura : ILeitura<Evento>
{
    // API que só lê dados — não precisa de escrita!
}
```

---

## D — Dependency Inversion Principle (DIP)

### O problema: dependência de implementação

```csharp
// ❌ Classe de alto nível depende de classe de baixo nível
public class EventoService
{
    // ❌ Acoplamento: se mudar o repositório, quebra aqui
    private EventoRepository _repo = new EventoRepository();
    
    public void CriarEvento(Evento evento)
    {
        _repo.Salvar(evento);
    }
}

// ❌ Se quiser testar, precisa do banco real
// ❌ Se quiser trocar por MongoDB, precisa modificar esta classe
```

### A solução: injeção de dependência

```csharp
// ✅ DIP: depender de abstrações

// Abstração
public interface IRepositorioEvento
{
    void Salvar(Evento evento);
    Evento Buscar(int id);
}

// Implementação de baixo nível
public class EventoRepositorioSql : IRepositorioEvento { /* ... */ }
public class EventoRepositorioMongo : IRepositorioEvento { /* ... */ }

// Classe de alto nível depende da abstração
public class EventoService
{
    private readonly IRepositorioEvento _repo;
    
    // ✅ Injeção de dependência via construtor
    public EventoService(IRepositorioEvento repo)
    {
        _repo = repo;
    }
    
    public void CriarEvento(Evento evento)
    {
        _repo.Salvar(evento);
    }
}

// Testes com mock fácil:
var mockRepo = new Mock<IRepositorioEvento>();
var service = new EventoService(mockRepo.Object);

// Trocar implementação sem mudar serviço:
var serviceComMongo = new EventoService(new EventoRepositorioMongo());
```

---

## Resumo dos 5 Princípios

| Princípio | Em uma frase | Benefício |
|-----------|--------------|-----------|
| **SRP** | Uma classe, uma responsabilidade | Código mais simples e fácil de entender |
| **OCP** | Estender sem modificar | Adicionar features sem risco de quebrar |
| **LSP** | Derivadas substituem bases | Polimorfismo seguro |
| **ISP** | Interfaces pequenas | Classes não implementam o que não usam |
| **DIP** | Depende de abstrações | Código testável e desacoplado |

---

## Relação com os Pilares da OO

```
┌─────────────────────────────────────────────────────────────────┐
│  PILARES DA OO          │  SOLID                               │
├─────────────────────────┼──────────────────────────────────────┤
│  Encapsulamento         │  SRP (esconde o que é responsabilidade│
│                         │  de outra classe)                    │
├─────────────────────────┼──────────────────────────────────────┤
│  Herança                │  LSP (garante que herança funcione)  │
│                         │  OCP (herança como extensão)         │
├─────────────────────────┼──────────────────────────────────────┤
│  Polimorfismo           │  DIP (via abstrações)                │
│                         │  LSP (polimorfismo seguro)           │
├─────────────────────────┼──────────────────────────────────────┤
│  Abstração              │  ISP (abstrações enxutas)            │
│                         │  DIP (abstrações para desacoplar)    │
└─────────────────────────┴──────────────────────────────────────┘
```

---

*Anterior: [Pilares da OO](./pilares-oo.md) | Próximo: [Exemplo Prático — Validação de Formulários](./praticas/ExemploInterfaces.cs)*
