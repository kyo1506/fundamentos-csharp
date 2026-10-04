namespace Fase07.AltaPerformance.Core.Parsing;

public ref struct LogTokenizer
{
    private ReadOnlySpan<char> _restante;
    private readonly char _delimitador;

    public LogTokenizer(ReadOnlySpan<char> texto, char delimitador = '|')
    {
        _restante = texto;
        _delimitador = delimitador;
    }

    public bool TryGetNext(out ReadOnlySpan<char> token)
    {
        if (_restante.IsEmpty)
        {
            token = default;
            return false;
        }

        int indice = _restante.IndexOf(_delimitador);
        if (indice == -1)
        {
            token = _restante;
            _restante = ReadOnlySpan<char>.Empty;
            return true;
        }

        token = _restante.Slice(0, indice);
        _restante = _restante.Slice(indice + 1);
        return true;
    }
}
