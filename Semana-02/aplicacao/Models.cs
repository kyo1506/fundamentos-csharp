// ============================================
// MODELS — Validação de Formulários
// Demonstra: Encapsulamento, Herança, Polimorfismo, Abstração + SOLID
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Semana02.ValidadorFormulario;

// ============================================
// ENUMS
// ============================================

public enum TipoParticipante
{
    Estudante,
    Profissional,
    Vip,
    Idoso
}

public enum TipoNotificacao
{
    Email,
    Sms
}

// ============================================
// MODELOS DE DOMÍNIO (Encapsulamento)
// ============================================

public class Participante
{
    private string _nome;
    private string _cpf;
    private string _email;
    private string _telefone;
    private int _idade;
    
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
    
    public string Email
    {
        get => _email;
        set
        {
            if (!ValidarEmail(value))
                throw new ArgumentException("E-mail inválido");
            _email = value;
        }
    }
    
    public string Telefone
    {
        get => _telefone;
        set
        {
            if (!ValidarTelefone(value))
                throw new ArgumentException("Telefone inválido");
            _telefone = value;
        }
    }
    
    public int Idade
    {
        get => _idade;
        set
        {
            if (value < 0 || value > 150)
                throw new ArgumentException("Idade inválida");
            _idade = value;
        }
    }
    
    public TipoParticipante Tipo { get; set; }
    public DateTime DataInscricao { get; } = DateTime.Now;
    
    public Participante(string nome, string cpf, string email, int idade, TipoParticipante tipo)
    {
        Nome = nome;
        Cpf = cpf;
        Email = email;
        Idade = idade;
        Tipo = tipo;
        Telefone = "(11) 99999-9999"; // Padrão para simplificar
    }
    
    private bool ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        string numeros = Regex.Replace(cpf, @"[^\d]", "");
        return numeros.Length == 11 && numeros.Distinct().Count() > 1;
    }
    
    private bool ValidarEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return email.Contains('@') && email.Contains('.');
    }
    
    private bool ValidarTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone)) return false;
        string numeros = Regex.Replace(telefone, @"[^\d]", "");
        return numeros.Length >= 10 && numeros.Length <= 11;
    }
    
    public override string ToString() => $"{Nome} (CPF: {Cpf})";
}

// ============================================
// INTERFACES (Abstração + ISP)
// ============================================

public interface IValidador<T>
{
    bool Validar(T valor);
    string ObterErro();
}

public interface IRepositorio<T> where T : class
{
    void Adicionar(T item);
    void Remover(int id);
    T? Buscar(int id);
    IEnumerable<T> Listar();
}

public interface INotificacaoService
{
    void Enviar(string destinatario, string mensagem);
}

public interface IRelatorioService
{
    string GerarRelatorio(IEnumerable<Participante> participantes);
}

// ============================================
// VALIDADORES (SRP — Um validador por classe)
// ============================================

public class ValidadorCpf : IValidador<string>
{
    public bool Validar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        string cpf = Regex.Replace(valor, @"[^\d]", "");
        if (cpf.Length != 11 || cpf.Distinct().Count() == 1) return false;
        
        // Primeiro dígito verificador
        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int soma = 0;
        for (int i = 0; i < 9; i++) soma += (cpf[i] - '0') * mult1[i];
        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        if (cpf[9] - '0' != resto) return false;
        
        // Segundo dígito verificador
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 10; i++) soma += (cpf[i] - '0') * mult2[i];
        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        return cpf[10] - '0' == resto;
    }
    
    public string ObterErro() => "CPF inválido";
}

public class ValidadorEmail : IValidador<string>
{
    public bool Validar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        return Regex.IsMatch(valor, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    public string ObterErro() => "E-mail inválido";
}

public class ValidadorIdade : IValidador<int>
{
    private readonly int _minima;
    private readonly int _maxima;
    
    public ValidadorIdade(int minima = 0, int maxima = 150)
    {
        _minima = minima;
        _maxima = maxima;
    }
    
    public bool Validar(int valor) => valor >= _minima && valor <= _maxima;
    public string ObterErro() => $"Idade deve estar entre {_minima} e {_maxima}";
}

// ============================================
// REPOSITÓRIO (DIP — Abstração para persistência)
// ============================================

public class RepositorioMemoria<T> : IRepositorio<T> where T : class
{
    private readonly List<T> _items = new();
    private int _proximoId = 1;
    
    public void Adicionar(T item) => _items.Add(item);
    public void Remover(int id) => _items.RemoveAt(id - 1);
    public T? Buscar(int id) => id > 0 && id <= _items.Count ? _items[id - 1] : null;
    public IEnumerable<T> Listar() => _items.AsReadOnly();
}

// ============================================
// SERVIÇOS DE NOTIFICAÇÃO (OCP + DIP)
// ============================================

public class EmailNotificacaoService : INotificacaoService
{
    public void Enviar(string destinatario, string mensagem)
    {
        Console.WriteLine($"  [EMAIL] → {destinatario}: {mensagem}");
    }
}

public class SmsNotificacaoService : INotificacaoService
{
    public void Enviar(string destinatario, string mensagem)
    {
        Console.WriteLine($"  [SMS] → {destinatario}: {mensagem}");
    }
}

// ============================================
// SERVIÇO DE RELATÓRIO (SRP)
// ============================================

public class RelatorioParticipanteService : IRelatorioService
{
    public string GerarRelatorio(IEnumerable<Participante> participantes)
    {
        var lista = participantes.ToList();
        var sb = new System.Text.StringBuilder();
        
        sb.AppendLine("╔══════════════════════════════════════════════════════════╗");
        sb.AppendLine("║  RELATÓRIO DE PARTICIPANTES                             ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════╝");
        sb.AppendLine($"Total: {lista.Count} participantes");
        sb.AppendLine();
        
        // Agrupar por tipo
        var grupos = lista.GroupBy(p => p.Tipo);
        foreach (var grupo in grupos)
        {
            sb.AppendLine($"  {grupo.Key}: {grupo.Count()}");
            foreach (var p in grupo)
            {
                sb.AppendLine($"    - {p.Nome} ({p.Idade} anos)");
            }
        }
        
        return sb.ToString();
    }
}

// ============================================
// SERVIÇO PRINCIPAL (Orquestrador — DIP)
// ============================================

public class ServicoInscricao
{
    private readonly IRepositorio<Participante> _repositorio;
    private readonly INotificacaoService _notificacao;
    private readonly IRelatorioService _relatorio;
    
    // DIP: recebe abstrações, não implementações concretas
    public ServicoInscricao(
        IRepositorio<Participante> repositorio,
        INotificacaoService notificacao,
        IRelatorioService relatorio)
    {
        _repositorio = repositorio;
        _notificacao = notificacao;
        _relatorio = relatorio;
    }
    
    public bool Inscrever(Participante participante)
    {
        // Validar
        if (!ValidarParticipante(participante, out var erros))
        {
            Console.WriteLine("Erros na inscrição:");
            foreach (var erro in erros) Console.WriteLine($"  - {erro}");
            return false;
        }
        
        // Salvar
        _repositorio.Adicionar(participante);
        
        // Notificar
        _notificacao.Enviar(participante.Email, 
            $"Olá {participante.Nome}, sua inscrição foi confirmada!");
        
        return true;
    }
    
    public string GerarRelatorio()
    {
        return _relatorio.GerarRelatorio(_repositorio.Listar());
    }
    
    private bool ValidarParticipante(Participante p, out List<string> erros)
    {
        erros = new List<string>();
        
        var validadorCpf = new ValidadorCpf();
        var validadorEmail = new ValidadorEmail();
        var validadorIdade = new ValidadorIdade(0, 150);
        
        if (!validadorCpf.Validar(p.Cpf))
            erros.Add(validadorCpf.ObterErro());
        
        if (!validadorEmail.Validar(p.Email))
            erros.Add(validadorEmail.ObterErro());
        
        if (!validadorIdade.Validar(p.Idade))
            erros.Add(validadorIdade.ObterErro());
        
        return erros.Count == 0;
    }
}

// ============================================
// CALCULADORA DE DESCONTO (OCP + Polimorfismo)
// ============================================

public interface ICalculadoraDesconto
{
    decimal CalcularDesconto(Participante participante);
    bool AplicaSe(Participante participante);
}

public class DescontoEstudante : ICalculadoraDesconto
{
    public decimal CalcularDesconto(Participante p) => 0.5m; // 50%
    public bool AplicaSe(Participante p) => p.Tipo == TipoParticipante.Estudante;
}

public class DescontoIdoso : ICalculadoraDesconto
{
    public decimal CalcularDesconto(Participante p) => 0.4m; // 40%
    public bool AplicaSe(Participante p) => p.Idade >= 60;
}

public class DescontoVip : ICalculadoraDesconto
{
    public decimal CalcularDesconto(Participante p) => 0.0m; // Sem desconto, mas prioridade
    public bool AplicaSe(Participante p) => p.Tipo == TipoParticipante.Vip;
}

public class CalculadoraPrecoFinal
{
    private readonly List<ICalculadoraDesconto> _descontos = new();
    
    public void AdicionarDesconto(ICalculadoraDesconto desconto)
    {
        _descontos.Add(desconto); // OCP: adicionar sem modificar
    }
    
    public decimal Calcular(Participante participante, decimal precoBase)
    {
        var desconto = _descontos
            .Where(d => d.AplicaSe(participante))
            .OrderByDescending(d => d.CalcularDesconto(participante))
            .FirstOrDefault();
        
        return precoBase * (1 - (desconto?.CalcularDesconto(participante) ?? 0));
    }
}
