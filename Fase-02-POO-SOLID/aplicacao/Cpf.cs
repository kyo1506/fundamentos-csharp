using System.Text.RegularExpressions;

namespace Fase02.PooSolid;

public readonly struct Cpf
{
    public string ValorLimpo { get; }

    public string ValorFormatado =>
        $"{ValorLimpo[..3]}.{ValorLimpo.Substring(3, 3)}.{ValorLimpo.Substring(6, 3)}-{ValorLimpo.Substring(9, 2)}";

    public Cpf(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("CPF não pode ser nulo ou vazio.", nameof(valor));

        var limpo = Regex.Replace(valor, @"[^\d]", "");
        if (limpo.Length != 11)
            throw new ArgumentException("CPF deve conter exatamente 11 dígitos numéricos.", nameof(valor));

        // Não pode ter todos os dígitos iguais (ex.: 111.111.111-11)
        if (new string(limpo[0], 11) == limpo)
            throw new ArgumentException("CPF inválido (todos os dígitos são iguais).", nameof(valor));

        ValorLimpo = limpo;
    }

    public override string ToString() => ValorFormatado;
}
