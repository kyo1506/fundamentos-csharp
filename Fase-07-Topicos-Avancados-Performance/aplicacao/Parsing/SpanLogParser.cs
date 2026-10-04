using System.Globalization;
using Fase07.AltaPerformance.Core.Modelos;

namespace Fase07.AltaPerformance.Core.Parsing;

public static class SpanLogParser
{
    public static bool TryParse(ReadOnlySpan<char> linha, out RegistroEvento? evento)
    {
        evento = null;
        if (linha.IsEmpty)
        {
            return false;
        }

        var tokenizer = new LogTokenizer(linha, '|');

        // 1. Id (Guid)
        if (!tokenizer.TryGetNext(out var idSpan) || !Guid.TryParse(idSpan, out var id))
        {
            return false;
        }

        // 2. Timestamp (DateTimeOffset)
        if (!tokenizer.TryGetNext(out var timestampSpan) || !DateTimeOffset.TryParse(timestampSpan, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp))
        {
            return false;
        }

        // 3. Nivel (NivelLog)
        if (!tokenizer.TryGetNext(out var nivelSpan) || !Enum.TryParse<NivelLog>(nivelSpan, ignoreCase: true, out var nivel))
        {
            return false;
        }

        // 4. Servico (string)
        if (!tokenizer.TryGetNext(out var servicoSpan) || servicoSpan.IsEmpty)
        {
            return false;
        }
        string servico = servicoSpan.ToString();

        // 5. Mensagem (string)
        if (!tokenizer.TryGetNext(out var mensagemSpan) || mensagemSpan.IsEmpty)
        {
            return false;
        }
        string mensagem = mensagemSpan.ToString();

        // 6. CodigoHttp (int)
        if (!tokenizer.TryGetNext(out var codigoSpan) || !int.TryParse(codigoSpan, NumberStyles.Integer, CultureInfo.InvariantCulture, out var codigoHttp))
        {
            return false;
        }

        // 7. DuracaoMs (double)
        if (!tokenizer.TryGetNext(out var duracaoSpan) || !double.TryParse(duracaoSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var duracaoMs))
        {
            return false;
        }

        evento = new RegistroEvento(id, timestamp, nivel, servico, mensagem, codigoHttp, duracaoMs);
        return true;
    }
}
