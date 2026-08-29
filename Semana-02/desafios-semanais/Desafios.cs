// ============================================
// DESAFIOS SEMANAIS — Semana 2
// Gestão de Inscrições em Eventos
// ============================================

using Semana02.ValidadorFormulario;

namespace Semana02.Desafios;

/// <summary>
/// Desafio 1 — Fácil: Encapsulamento
/// 
/// Crie uma classe `Evento` com as seguintes propriedades: 
/// Nome (string), Data (DateTime), Local (string), MaxParticipantes (int), 
/// PrecoBase (decimal), Ativo (bool).
/// 
/// a) O `set` de `Nome` deve lançar exceção se for nulo ou vazio.
/// b) O `set` de `MaxParticipantes` deve ser > 0.
/// c) O `set` de `PrecoBase` deve ser >= 0.
/// d) Crie um método `Cancelar()` que define Ativo como false.
/// e) Por que é importante validar no setter em vez de validar externamente?
/// </summary>
public class Desafio01_Fácil
{
    public class Evento
    {
        private string _nome;
        private int _maxParticipantes;
        private decimal _precoBase;
        
        public string Nome
        {
            get => _nome;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Nome não pode ser vazio");
                _nome = value;
            }
        }
        
        public DateTime Data { get; set; }
        public string Local { get; set; }
        
        public int MaxParticipantes
        {
            get => _maxParticipantes;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Máximo de participantes deve ser > 0");
                _maxParticipantes = value;
            }
        }
        
        public decimal PrecoBase
        {
            get => _precoBase;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Preço base não pode ser negativo");
                _precoBase = value;
            }
        }
        
        public bool Ativo { get; private set; } = true;
        
        public Evento(string nome, DateTime data, string local, int maxParticipantes, decimal precoBase)
        {
            Nome = nome;
            Data = data;
            Local = local;
            MaxParticipantes = maxParticipantes;
            PrecoBase = precoBase;
        }
        
        public void Cancelar() => Ativo = false;
        
        public override string ToString() =>
            $"{Nome} em {Local} ({Data:dd/MM/yyyy}) - R${PrecoBase:C} - {(Ativo ? "Ativo" : "Cancelado")}";
    }

    public void Resolver()
    {
        Console.WriteLine("=== Desafio 1 — Fácil: Encapsulamento ===\n");
        
        var evento = new Evento(
            "Workshop C#",
            DateTime.Now.AddDays(30),
            "Auditório Principal",
            50,
            100.00m);
        
        Console.WriteLine(evento);
        
        evento.Cancelar();
        Console.WriteLine($"Após cancelar: {evento}");
        
        try
        {
            evento.Nome = "";
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
        
        Console.WriteLine("\nValidação no setter garante que o objeto NUNCA fica em estado inválido.");
    }
}

/// <summary>
/// Desafio 2 — Médio: Herança e Polimorfismo
/// 
/// Crie uma hierarquia para diferentes tipos de eventos:
/// 
/// a) Classe abstrata `EventoBase` com métodos abstratos `CalcularValorInscricao()` e `ObterTipo()`.
/// b) `EventoPresencial` — tem `CustoLocal` (decimal), valor = PrecoBase + CustoLocal.
/// c) `EventoOnline` — tem `Plataforma` (string), valor = PrecoBase * 0.7 (30% desconto).
/// d) `EventoHibrido` — tem CustoLocal e Plataforma, valor = PrecoBase + CustoLocal * 0.5.
/// e) Crie uma lista de `EventoBase` e itere, chamando `CalcularValorInscricao()` para cada um.
/// </summary>
public class Desafio02_Médio
{
    public abstract class EventoBase
    {
        public string Nome { get; set; }
        public decimal PrecoBase { get; set; }
        
        public abstract decimal CalcularValorInscricao();
        public abstract string ObterTipo();
        
        public virtual string ExibirResumo() =>
            $"{ObterTipo()}: {Nome} - R${CalcularValorInscricao():C}";
    }
    
    public class EventoPresencial : EventoBase
    {
        public decimal CustoLocal { get; set; }
        
        public override decimal CalcularValorInscricao() => PrecoBase + CustoLocal;
        public override string ObterTipo() => "Presencial";
    }
    
    public class EventoOnline : EventoBase
    {
        public string Plataforma { get; set; }
        
        public override decimal CalcularValorInscricao() => PrecoBase * 0.7m;
        public override string ObterTipo() => "Online";
    }
    
    public class EventoHibrido : EventoBase
    {
        public decimal CustoLocal { get; set; }
        public string Plataforma { get; set; }
        
        public override decimal CalcularValorInscricao() => PrecoBase + CustoLocal * 0.5m;
        public override string ObterTipo() => "Híbrido";
    }

    public void Resolver()
    {
        Console.WriteLine("=== Desafio 2 — Médio: Herança e Polimorfismo ===\n");
        
        List<EventoBase> eventos = new()
        {
            new EventoPresencial { Nome = "Workshop C#", PrecoBase = 100m, CustoLocal = 50m },
            new EventoOnline { Nome = "Masterclass .NET", PrecoBase = 80m, Plataforma = "Zoom" },
            new EventoHibrido { Nome = "Conferência Dev", PrecoBase = 150m, CustoLocal = 30m, Plataforma = "Teams" }
        };
        
        foreach (var evento in eventos)
        {
            Console.WriteLine(evento.ExibirResumo());
        }
        
        Console.WriteLine("\nPolimorfismo: cada evento calcula o valor de um jeito, mas a chamada é uniforme!");
    }
}

/// <summary>
/// Desafio 3 — Desafio: SOLID
/// 
/// O código atual viola vários princípios SOLID. Refatore:
/// 
/// a) **Single Responsibility:** Separe a lógica de notificação da lógica de inscrição.
/// b) **Open/Closed:** Adicione um novo tipo `EventoWorkshop` sem modificar código existente.
/// c) **Liskov Substitution:** Garanta que qualquer `EventoBase` pode ser tratado como base sem quebrar.
/// d) **Interface Segregation:** Divida `IEventoService` em `IEventoLeitura` e `IEventoEscrita`.
/// e) **Dependency Inversion:** O serviço de inscrição deve depender de `IRepositorio<EventoBase>`.
/// </summary>
public class Desafio03_Desafio
{
    // ISP: Interfaces segregadas
    public interface IEventoLeitura
    {
        EventoBase Buscar(int id);
        IEnumerable<EventoBase> Listar();
    }
    
    public interface IEventoEscrita
    {
        void Criar(EventoBase evento);
        void Atualizar(EventoBase evento);
        void Excluir(int id);
    }
    
    // DIP: Repositório abstrato
    public interface IRepositorioEvento : IEventoLeitura, IEventoEscrita { }
    
    // SRP: Repositório só persiste
    public class RepositorioEventoMemoria : IRepositorioEvento
    {
        private readonly List<EventoBase> _eventos = new();
        private int _proximoId = 1;
        
        public EventoBase Buscar(int id) => _eventos.Find(e => e.GetHashCode() == id);
        public IEnumerable<EventoBase> Listar() => _eventos.AsReadOnly();
        public void Criar(EventoBase evento) => _eventos.Add(evento);
        public void Atualizar(EventoBase evento) { /* implementação */ }
        public void Excluir(int id) => _eventos.RemoveAll(e => e.GetHashCode() == id);
    }
    
    // SRP: Serviço de notificação só notifica
    public interface INotificacaoEvento
    {
        void NotificarCriacao(EventoBase evento);
        void NotificarCancelamento(EventoBase evento);
    }
    
    // OCP: Novo tipo sem modificar existente
    public class EventoWorkshop : Desafio02_Médio.EventoBase
    {
        public int HorasDuracao { get; set; }
        public string Instrutor { get; set; }
        
        public override decimal CalcularValorInscricao() => PrecoBase * HorasDuracao;
        public override string ObterTipo() => "Workshop";
    }
    
    // DIP: Serviço depende de abstrações
    public class ServicoEvento
    {
        private readonly IRepositorioEvento _repositorio;
        private readonly INotificacaoEvento _notificacao;
        
        public ServicoEvento(IRepositorioEvento repositorio, INotificacaoEvento notificacao)
        {
            _repositorio = repositorio;
            _notificacao = notificacao;
        }
        
        public void CriarEvento(EventoBase evento)
        {
            _repositorio.Criar(evento);
            _notificacao.NotificarCriacao(evento);
        }
    }

    public void Resolver()
    {
        Console.WriteLine("=== Desafio 3 — Desafio: SOLID ===\n");
        
        Console.WriteLine("✅ SRP: Separei RepositorioEvento (persistência) de ServicoEvento (negócio)");
        Console.WriteLine("✅ OCP: EventoWorkshop adicionado sem modificar código existente");
        Console.WriteLine("✅ LSP: Todos os eventos podem ser tratados como EventoBase");
        Console.WriteLine("✅ ISP: IEventoLeitura e IEventoEscrita separadas");
        Console.WriteLine("✅ DIP: ServicoEvento depende de IRepositorioEvento, não de implementação");
        
        var repo = new RepositorioEventoMemoria();
        var servico = new ServicoEvento(repo, null!);
        
        var workshop = new EventoWorkshop
        {
            Nome = "Workshop C# Avançado",
            PrecoBase = 50m,
            HorasDuracao = 4,
            Instrutor = "Maria Silva"
        };
        
        servico.CriarEvento(workshop);
        Console.WriteLine($"\nEvento criado: {workshop.ExibirResumo()}");
    }
}
