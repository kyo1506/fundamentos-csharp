using System.Globalization;
using System.Text;

namespace Fase01.CSharpBasico;

/// <summary>
/// Exceção de domínio lançada quando uma operação financeira excede o saldo disponível.
/// </summary>
public class SaldoInsuficienteException : Exception
{
    public decimal SaldoAtual { get; }
    public decimal ValorTentado { get; }

    public SaldoInsuficienteException(decimal saldoAtual, decimal valorTentado)
        : base($"Saldo de {saldoAtual:C} é insuficiente para a operação de {valorTentado:C}.")
    {
        SaldoAtual = saldoAtual;
        ValorTentado = valorTentado;
    }
}

/// <summary>
/// Representa uma quantia monetária imutável (Value Object) com moeda explícita.
/// </summary>
public readonly struct Dinheiro : IEquatable<Dinheiro>
{
    public decimal Valor { get; }
    public string Moeda { get; }

    public Dinheiro(decimal valor, string moeda = "BRL")
    {
        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor monetário não pode ser negativo.");

        Valor = valor;
        Moeda = string.IsNullOrWhiteSpace(moeda) ? "BRL" : moeda.ToUpperInvariant();
    }

    public static Dinheiro operator +(Dinheiro a, Dinheiro b)
    {
        if (a.Moeda != b.Moeda)
            throw new InvalidOperationException($"Não é possível somar moedas diferentes: {a.Moeda} e {b.Moeda}.");

        return new Dinheiro(a.Valor + b.Valor, a.Moeda);
    }

    public static Dinheiro operator -(Dinheiro a, Dinheiro b)
    {
        if (a.Moeda != b.Moeda)
            throw new InvalidOperationException($"Não é possível subtrair moedas diferentes: {a.Moeda} e {b.Moeda}.");

        if (a.Valor < b.Valor)
            throw new SaldoInsuficienteException(a.Valor, b.Valor);

        return new Dinheiro(a.Valor - b.Valor, a.Moeda);
    }

    public static Dinheiro operator *(Dinheiro a, decimal fator)
    {
        if (fator < 0)
            throw new ArgumentOutOfRangeException(nameof(fator), "O fator multiplicador não pode ser negativo.");

        return new Dinheiro(Math.Round(a.Valor * fator, 2, MidpointRounding.AwayFromZero), a.Moeda);
    }

    public bool Equals(Dinheiro other) => Valor == other.Valor && Moeda == other.Moeda;
    public override bool Equals(object? obj) => obj is Dinheiro other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Valor, Moeda);
    public static bool operator ==(Dinheiro left, Dinheiro right) => left.Equals(right);
    public static bool operator !=(Dinheiro left, Dinheiro right) => !left.Equals(right);

    public override string ToString() => $"{Moeda} {Valor:N2}";
}

/// <summary>
/// Record posicional para modelagem de pedidos e aplicação de regras via pattern matching.
/// </summary>
public record Pedido(int Id, decimal ValorTotal, string Pais, bool ClienteVip, int QuantidadeItens);

/// <summary>
/// Métodos de extensão utilitários para tipos primitivos e strings.
/// </summary>
public static class BasicoExtensions
{
    public static bool EhPar(this int numero) => numero % 2 == 0;

    public static string Truncar(this string? texto, int tamanhoMaximo, string sufixo = "...")
    {
        if (string.IsNullOrEmpty(texto) || texto.Length <= tamanhoMaximo)
            return texto ?? string.Empty;

        return string.Concat(texto.AsSpan(0, tamanhoMaximo), sufixo);
    }

    public static string ToMoedaReal(this decimal valor)
    {
        return valor.ToString("C", new CultureInfo("pt-BR"));
    }
}

/// <summary>
/// Funções puras que cobrem os temas fundamentais da Fase 01.
/// </summary>
public static class Basico
{
    // ---- 01.2: Strings e StringBuilder ----
    public static string InverterTexto(string entrada)
    {
        if (string.IsNullOrEmpty(entrada)) return entrada ?? string.Empty;
        var sb = new StringBuilder(entrada.Length);
        for (int i = entrada.Length - 1; i >= 0; i--)
        {
            sb.Append(entrada[i]);
        }
        return sb.ToString();
    }

    // ---- 01.3: Números e Arredondamento ----
    public static decimal ArredondarComercial(decimal valor, int casas = 2)
    {
        return Math.Round(valor, casas, MidpointRounding.AwayFromZero);
    }

    public static List<decimal> DividirParcelas(decimal total, int parcelas)
    {
        if (parcelas <= 0)
            throw new ArgumentOutOfRangeException(nameof(parcelas), "Número de parcelas deve ser maior que zero.");

        decimal valorBase = Math.Floor((total / parcelas) * 100m) / 100m;
        decimal resto = total - (valorBase * parcelas);

        var lista = new List<decimal>(parcelas);
        for (int i = 0; i < parcelas; i++)
        {
            // O centavo restante é incorporado na primeira parcela para exatidão
            decimal parcela = valorBase;
            if (resto > 0)
            {
                parcela += 0.01m;
                resto -= 0.01m;
            }
            lista.Add(parcela);
        }
        return lista;
    }

    // ---- 01.4: Coleções Fundamentais ----
    public static Dictionary<string, int> ContarFrequenciaPalavras(string texto)
    {
        var frequencias = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(texto)) return frequencias;

        var tokens = texto.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            var palavra = token.Trim(',', '.', '!', '?', ';', ':', '"', '(', ')');
            if (!string.IsNullOrEmpty(palavra))
            {
                frequencias[palavra] = frequencias.GetValueOrDefault(palavra, 0) + 1;
            }
        }

        return frequencias;
    }

    public static bool ValidarDelimitadores(string expressao)
    {
        if (string.IsNullOrEmpty(expressao)) return true;

        var pilha = new Stack<char>();
        foreach (char c in expressao)
        {
            if (c is '(' or '[' or '{')
            {
                pilha.Push(c);
            }
            else if (c is ')' or ']' or '}')
            {
                if (pilha.Count == 0) return false;
                char topo = pilha.Pop();
                if (c == ')' && topo != '(') return false;
                if (c == ']' && topo != '[') return false;
                if (c == '}' && topo != '{') return false;
            }
        }

        return pilha.Count == 0;
    }

    // ---- 01.7: Pattern Matching e Switch Expressions ----
    public static decimal CalcularPercentualDesconto(Pedido pedido) => pedido switch
    {
        { ClienteVip: true, ValorTotal: >= 1000m } => 0.25m, // 25%
        { ClienteVip: true }                       => 0.15m, // 15%
        { Pais: "BR", QuantidadeItens: >= 5 }      => 0.10m, // 10%
        { ValorTotal: >= 500m }                    => 0.05m, // 5%
        _                                          => 0.00m
    };

    // ---- 01.10: Datas e Horas ----
    public static int CalcularDiasUteis(DateOnly inicio, DateOnly fim)
    {
        if (inicio > fim)
            (inicio, fim) = (fim, inicio);

        int diasUteis = 0;
        var atual = inicio;
        while (atual <= fim)
        {
            if (atual.DayOfWeek != DayOfWeek.Saturday && atual.DayOfWeek != DayOfWeek.Sunday)
            {
                diasUteis++;
            }
            atual = atual.AddDays(1);
        }
        return diasUteis;
    }
}
