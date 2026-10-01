namespace Fase02.PooSolid;

public interface IPoliticaReembolso
{
    string NomePolitica { get; }
    decimal CalcularReembolso(decimal valorPago, int diasDecorridos);
}

public class ReembolsoProgressivoPolitica : IPoliticaReembolso
{
    public string NomePolitica => "Reembolso Progressivo (7d: 100%, 30d: 50%)";

    public decimal CalcularReembolso(decimal valorPago, int diasDecorridos)
    {
        if (valorPago <= 0) return 0m;
        if (diasDecorridos < 0)
            throw new ArgumentOutOfRangeException(nameof(diasDecorridos), "Dias decorridos não podem ser negativos.");

        return diasDecorridos switch
        {
            <= 7 => valorPago,
            <= 30 => Math.Round(valorPago * 0.50m, 2, MidpointRounding.AwayFromZero),
            _ => 0m
        };
    }
}
