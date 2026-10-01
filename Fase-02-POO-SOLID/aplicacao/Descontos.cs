namespace Fase02.PooSolid;

public interface IDescontoPolicy
{
    string NomeRegra { get; }
    decimal CalcularValorFinal(decimal precoBase);
}

public class SemDescontoPolicy : IDescontoPolicy
{
    public string NomeRegra => "Sem Desconto";
    public decimal CalcularValorFinal(decimal precoBase) => precoBase;
}

public class DescontoPercentualPolicy : IDescontoPolicy
{
    public string NomeRegra { get; }
    public decimal Percentual { get; }

    public DescontoPercentualPolicy(string nomeRegra, decimal percentual)
    {
        if (percentual is < 0m or > 1m)
            throw new ArgumentOutOfRangeException(nameof(percentual), "O percentual deve estar entre 0.0 (0%) e 1.0 (100%).");

        NomeRegra = nomeRegra;
        Percentual = percentual;
    }

    public decimal CalcularValorFinal(decimal precoBase)
    {
        var desconto = Math.Round(precoBase * Percentual, 2, MidpointRounding.AwayFromZero);
        return Math.Max(0m, precoBase - desconto);
    }
}

public class DescontoEstudantePolicy : DescontoPercentualPolicy
{
    public DescontoEstudantePolicy() : base("Desconto Estudante (20%)", 0.20m) {}
}

public class DescontoVipPolicy : DescontoPercentualPolicy
{
    public DescontoVipPolicy() : base("Desconto VIP (30%)", 0.30m) {}
}

public class DescontoValorFixoPolicy : IDescontoPolicy
{
    public string NomeRegra => "Desconto Fixo";
    public decimal ValorDesconto { get; }

    public DescontoValorFixoPolicy(decimal valorDesconto)
    {
        if (valorDesconto < 0)
            throw new ArgumentOutOfRangeException(nameof(valorDesconto), "Valor de desconto não pode ser negativo.");

        ValorDesconto = valorDesconto;
    }

    public decimal CalcularValorFinal(decimal precoBase) =>
        Math.Max(0m, precoBase - ValorDesconto);
}
