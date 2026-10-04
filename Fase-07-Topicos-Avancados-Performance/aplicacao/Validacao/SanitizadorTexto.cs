using System.Buffers;

namespace Fase07.AltaPerformance.Core.Validacao;

public static class SanitizadorTexto
{
    private static readonly SearchValues<char> CaracteresInseguros =
        SearchValues.Create("<>&\"'\0\r\n\t");

    public static bool ContemCaracteresInseguros(ReadOnlySpan<char> texto)
    {
        return texto.ContainsAny(CaracteresInseguros);
    }

    public static int IndicePrimeiroCaractereInseguro(ReadOnlySpan<char> texto)
    {
        return texto.IndexOfAny(CaracteresInseguros);
    }
}
