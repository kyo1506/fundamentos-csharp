using System.Text.RegularExpressions;

namespace Fase07.AltaPerformance.Core.Validacao;

public static partial class ValidadorChaveTransacao
{
    [GeneratedRegex(@"^TRX-[A-Z0-9]{4}-[0-9]{4}$", RegexOptions.CultureInvariant)]
    private static partial Regex ObterRegexChaveTransacao();

    public static bool Validar(ReadOnlySpan<char> chave)
    {
        return ObterRegexChaveTransacao().IsMatch(chave);
    }

    public static bool Validar(string? chave)
    {
        if (string.IsNullOrWhiteSpace(chave))
        {
            return false;
        }

        return ObterRegexChaveTransacao().IsMatch(chave);
    }
}
