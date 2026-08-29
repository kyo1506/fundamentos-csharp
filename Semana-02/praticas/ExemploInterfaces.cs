// ============================================
// EXEMPLO INTERFACES — Validação de Formulários
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Semana02.ExemploInterfaces;

// ============================================
// INTERFACES (Abstrações)
// ============================================

/// <summary>
/// Contrato para qualquer validador de campo de formulário.
/// </summary>
public interface IValidador
{
    bool Validar(string valor);
    string ObterMensagemErro();
}

/// <summary>
/// Contrato para formulários que podem ser validados.
/// </summary>
public interface IFormulario
{
    string Nome { get; }
    bool Validar(out List<string> erros);
}

/// <summary>
/// Contrato para envio de notificações.
/// </summary>
public interface INotificacaoService
{
    void Enviar(string destinatario, string mensagem);
}

// ============================================
// IMPLEMENTAÇÕES DE VALIDADORES (SRP + OCP)
// ============================================

public class ValidadorCpf : IValidador
{
    public bool Validar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        
        // Remove caracteres não numéricos
        string cpf = Regex.Replace(valor, @"[^\d]", "");
        
        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false; // 111.111.111-11
        
        // Cálculo do dígito verificador
        int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        
        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;
        
        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];
        
        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        
        if (int.Parse(cpf[9].ToString()) != resto) return false;
        
        // Segundo dígito
        soma = 0;
        tempCpf = cpf.Substring(0, 10);
        
        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];
        
        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;
        
        return int.Parse(cpf[10].ToString()) == resto;
    }
    
    public string ObterMensagemErro() => "CPF inválido";
}

public class ValidadorEmail : IValidador
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);
    
    public bool Validar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        return EmailRegex.IsMatch(valor);
    }
    
    public string ObterMensagemErro() => "E-mail inválido";
}

public class ValidadorTelefone : IValidador
{
    public bool Validar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) return false;
        string telefone = Regex.Replace(valor, @"[^\d]", "");
        return telefone.Length >= 10 && telefone.Length <= 11;
    }
    
    public string ObterMensagemErro() => "Telefone inválido";
}

public class ValidadorIdade : IValidador
{
    private readonly int _idadeMinima;
    private readonly int _idadeMaxima;
    
    public ValidadorIdade(int minima = 0, int maxima = 150)
    {
        _idadeMinima = minima;
        _idadeMaxima = maxima;
    }
    
    public bool Validar(string valor)
    {
        if (!int.TryParse(valor, out int idade)) return false;
        return idade >= _idadeMinima && idade <= _idadeMaxima;
    }
    
    public string ObterMensagemErro() => 
        $"Idade deve ser entre {_idadeMinima} e {_idadeMaxima}";
}

// ============================================
// FORMULÁRIOS CONCRETOS (Polimorfismo + LSP)
// ============================================

public abstract class FormularioBase : IFormulario
{
    public string Nome { get; protected set; }
    protected List<IValidador> Validadores { get; } = new();
    
    public virtual bool Validar(out List<string> erros)
    {
        erros = new List<string>();
        
        foreach (var validador in Validadores)
        {
            // Cada validador sabe validar seu campo
            // Não sabemos qual é o validador específico (polimorfismo)
        }
        
        return erros.Count == 0;
    }
}

public class FormularioInscricaoEvento : FormularioBase
{
    public string NomeParticipante { get; set; }
    public string Cpf { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public int Idade { get; set; }
    public bool Estudante { get; set; }
    
    public FormularioInscricaoEvento()
    {
        Nome = "Inscrição de Evento";
        Validadores.Add(new ValidadorCpf());
        Validadores.Add(new ValidadorEmail());
        Validadores.Add(new ValidadorTelefone());
    }
    
    public override bool Validar(out List<string> erros)
    {
        erros = new List<string>();
        
        if (string.IsNullOrWhiteSpace(NomeParticipante))
            erros.Add("Nome do participante é obrigatório");
        
        if (!new ValidadorCpf().Validar(Cpf))
            erros.Add("CPF inválido");
        
        if (!new ValidadorEmail().Validar(Email))
            erros.Add("E-mail inválido");
        
        if (!new ValidadorTelefone().Validar(Telefone))
            erros.Add("Telefone inválido");
        
        if (Idade < 0 || Idade > 150)
            erros.Add("Idade inválida");
        
        return erros.Count == 0;
    }
}

public class FormularioContato : FormularioBase
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Mensagem { get; set; }
    
    public FormularioContato()
    {
        Nome = "Formulário de Contato";
        Validadores.Add(new ValidadorEmail());
    }
    
    public override bool Validar(out List<string> erros)
    {
        erros = new List<string>();
        
        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("Nome é obrigatório");
        
        if (!new ValidadorEmail().Validar(Email))
            erros.Add("E-mail inválido");
        
        if (string.IsNullOrWhiteSpace(Mensagem) || Mensagem.Length < 10)
            erros.Add("Mensagem deve ter pelo menos 10 caracteres");
        
        return erros.Count == 0;
    }
}

// ============================================
// SERVIÇO DE NOTIFICAÇÃO (DIP)
// ============================================

public class EmailNotificacaoService : INotificacaoService
{
    public void Enviar(string destinatario, string mensagem)
    {
        // Em produção: SMTP, SendGrid, etc.
        Console.WriteLine($"[EMAIL] Para: {destinatario}");
        Console.WriteLine($"[EMAIL] Mensagem: {mensagem}");
    }
}

public class SmsNotificacaoService : INotificacaoService
{
    public void Enviar(string destinatario, string mensagem)
    {
        // Em produção: Twilio, etc.
        Console.WriteLine($"[SMS] Para: {destinatario}");
        Console.WriteLine($"[SMS] Mensagem: {mensagem}");
    }
}

// ============================================
// SERVIÇO DE INSCRIÇÃO (Orquestrador)
// ============================================

public class ServicoInscricao
{
    private readonly INotificacaoService _notificacaoService;
    
    // DIP: depende da abstração, não da implementação
    public ServicoInscricao(INotificacaoService notificacaoService)
    {
        _notificacaoService = notificacaoService;
    }
    
    public bool ProcessarInscricao(FormularioInscricaoEvento formulario)
    {
        if (!formulario.Validar(out var erros))
        {
            Console.WriteLine("Erros na inscrição:");
            foreach (var erro in erros)
                Console.WriteLine($"  - {erro}");
            return false;
        }
        
        // Salvar no banco (não implementado)
        Console.WriteLine($"Inscrição de {formulario.NomeParticipante} salva com sucesso!");
        
        // Enviar confirmação
        _notificacaoService.Enviar(
            formulario.Email,
            $"Olá {formulario.NomeParticipante}, sua inscrição foi confirmada!");
        
        return true;
    }
}

// ============================================
// PROGRAMA PRINCIPAL
// ============================================

public class Program
{
    public static void Main()
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  VALIDAÇÃO DE FORMULÁRIOS — EXEMPLO COM INTERFACES      ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        
        // Criando formulários
        var inscricao = new FormularioInscricaoEvento
        {
            NomeParticipante = "Maria Silva",
            Cpf = "529.982.247-25", // CPF válido
            Email = "maria@email.com",
            Telefone = "(11) 99999-8888",
            Idade = 25,
            Estudante = true
        };
        
        var contato = new FormularioContato
        {
            Nome = "João Santos",
            Email = "joao@email.com",
            Mensagem = "Gostaria de mais informações sobre o evento."
        };
        
        // Validação polimorfica
        Console.WriteLine("\n━━━ Validando Formulários ━━━\n");
        
        ValidarFormulario(inscricao);
        ValidarFormulario(contato);
        
        // Processar inscrição com notificação
        Console.WriteLine("\n━━━ Processando Inscrição ━━━\n");
        
        var servicoEmail = new ServicoInscricao(new EmailNotificacaoService());
        servicoEmail.ProcessarInscricao(inscricao);
        
        Console.WriteLine();
        
        var servicoSms = new ServicoInscricao(new SmsNotificacaoService());
        // servicoSms.ProcessarInscricao(inscricao); // Mesma lógica, notificação diferente
        
        // Teste com dados inválidos
        Console.WriteLine("\n━━━ Teste com Dados Inválidos ━━━\n");
        
        var inscricaoInvalida = new FormularioInscricaoEvento
        {
            NomeParticipante = "",
            Cpf = "111.111.111-11",
            Email = "email-invalido",
            Telefone = "123",
            Idade = -5
        };
        
        ValidarFormulario(inscricaoInvalida);
    }
    
    private static void ValidarFormulario(IFormulario formulario)
    {
        Console.WriteLine($"Validando: {formulario.Nome}");
        
        if (formulario.Validar(out var erros))
        {
            Console.WriteLine("  ✅ Formulário válido!");
        }
        else
        {
            Console.WriteLine("  ❌ Erros encontrados:");
            foreach (var erro in erros)
                Console.WriteLine($"     - {erro}");
        }
        
        Console.WriteLine();
    }
}
