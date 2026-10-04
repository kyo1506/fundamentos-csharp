using System.Collections.Concurrent;

namespace Fase05.GestaoAcademica.Core;

public interface IRepository<T, TId> where T : class
{
    Task<T?> ObterPorIdAsync(TId id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListarTodosAsync(CancellationToken ct = default);
    Task AdicionarAsync(T entidade, CancellationToken ct = default);
    Task AtualizarAsync(T entidade, CancellationToken ct = default);
}

public interface IAlunoRepository : IRepository<Aluno, int> { }

public interface ICursoRepository : IRepository<Curso, int> { }

public interface IMatriculaRepository : IRepository<Matricula, int>
{
    Task<IReadOnlyList<Matricula>> ObterPorAlunoIdAsync(int alunoId, CancellationToken ct = default);
    Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId, CancellationToken ct = default);
}

public class InMemoryAlunoRepository : IAlunoRepository
{
    private readonly ConcurrentDictionary<int, Aluno> _itens = new();

    public Task<Aluno?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _itens.TryGetValue(id, out var aluno);
        return Task.FromResult(aluno);
    }

    public Task<IReadOnlyList<Aluno>> ListarTodosAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<Aluno>>(_itens.Values.ToList());
    }

    public Task AdicionarAsync(Aluno entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Aluno entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }
}

public class InMemoryCursoRepository : ICursoRepository
{
    private readonly ConcurrentDictionary<int, Curso> _itens = new();

    public Task<Curso?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _itens.TryGetValue(id, out var curso);
        return Task.FromResult(curso);
    }

    public Task<IReadOnlyList<Curso>> ListarTodosAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<Curso>>(_itens.Values.ToList());
    }

    public Task AdicionarAsync(Curso entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Curso entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }
}

public class InMemoryMatriculaRepository : IMatriculaRepository
{
    private readonly ConcurrentDictionary<int, Matricula> _itens = new();

    public Task<Matricula?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        _itens.TryGetValue(id, out var matricula);
        return Task.FromResult(matricula);
    }

    public Task<IReadOnlyList<Matricula>> ListarTodosAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<Matricula>>(_itens.Values.ToList());
    }

    public Task AdicionarAsync(Matricula entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Matricula entidade, CancellationToken ct = default)
    {
        _itens[entidade.Id] = entidade;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Matricula>> ObterPorAlunoIdAsync(int alunoId, CancellationToken ct = default)
    {
        var lista = _itens.Values.Where(m => m.AlunoId == alunoId).ToList();
        return Task.FromResult<IReadOnlyList<Matricula>>(lista);
    }

    public Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId, CancellationToken ct = default)
    {
        bool existe = _itens.Values.Any(m =>
            m.AlunoId == alunoId &&
            m.CursoId == cursoId &&
            m.Status != StatusMatricula.Cancelada);

        return Task.FromResult(existe);
    }
}
