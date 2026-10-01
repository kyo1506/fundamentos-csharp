namespace Fase03.CourseAnalytics;

public interface IRepositorioGenerico<T, in TId> where T : class
{
    void Adicionar(T item);
    T? ObterPorChave(Func<T, TId> seletorChave, TId chave);
    IEnumerable<T> Filtrar(Func<T, bool> predicado);
    int Contar(Func<T, bool>? predicado = null);
    IReadOnlyList<T> ObterTodos();
}

public class RepositorioGenerico<T, TId> : IRepositorioGenerico<T, TId> where T : class
{
    private readonly List<T> _itens = new();

    public void Adicionar(T item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _itens.Add(item);
    }

    public T? ObterPorChave(Func<T, TId> seletorChave, TId chave)
    {
        ArgumentNullException.ThrowIfNull(seletorChave);
        return _itens.FirstOrDefault(i => EqualityComparer<TId>.Default.Equals(seletorChave(i), chave));
    }

    public IEnumerable<T> Filtrar(Func<T, bool> predicado)
    {
        ArgumentNullException.ThrowIfNull(predicado);
        return _itens.Where(predicado);
    }

    public int Contar(Func<T, bool>? predicado = null) =>
        predicado is null ? _itens.Count : _itens.Count(predicado);

    public IReadOnlyList<T> ObterTodos() => _itens.AsReadOnly();
}
