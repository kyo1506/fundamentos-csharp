namespace Fase05.GestaoAcademica.Core;

public interface IMatriculaService
{
    Task<Matricula> MatricularAlunoAsync(int alunoId, int cursoId, CancellationToken ct = default);
    Task<Matricula> CancelarMatriculaAsync(int matriculaId, CancellationToken ct = default);
    Task<IReadOnlyList<Matricula>> ListarMatriculasAlunoAsync(int alunoId, CancellationToken ct = default);
}

public class MatriculaService : IMatriculaService
{
    private readonly IAlunoRepository _alunoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IPoliticaDesconto _politicaDesconto;
    private readonly TimeProvider _timeProvider;
    private static int _proximoIdMatricula = 1;

    public MatriculaService(
        IAlunoRepository alunoRepository,
        ICursoRepository cursoRepository,
        IMatriculaRepository matriculaRepository,
        IPoliticaDesconto politicaDesconto,
        TimeProvider? timeProvider = null)
    {
        _alunoRepository = alunoRepository;
        _cursoRepository = cursoRepository;
        _matriculaRepository = matriculaRepository;
        _politicaDesconto = politicaDesconto;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<Matricula> MatricularAlunoAsync(int alunoId, int cursoId, CancellationToken ct = default)
    {
        var aluno = await _alunoRepository.ObterPorIdAsync(alunoId, ct)
            ?? throw new EntidadeNaoEncontradaException("Aluno", alunoId);

        var curso = await _cursoRepository.ObterPorIdAsync(cursoId, ct)
            ?? throw new EntidadeNaoEncontradaException("Curso", cursoId);

        bool jaMatriculado = await _matriculaRepository.ExisteMatriculaAtivaAsync(alunoId, cursoId, ct);
        if (jaMatriculado)
            throw new AlunoJaMatriculadoException(alunoId, cursoId);

        // Dispara VagasEsgotadasException se não houver vagas
        curso.OcuparVaga();

        decimal desconto = _politicaDesconto.CalcularDesconto(curso.PrecoBase, aluno);
        decimal valorCobrado = Math.Max(0m, curso.PrecoBase - desconto);

        int novoId = Interlocked.Increment(ref _proximoIdMatricula);
        var matricula = new Matricula(
            id: novoId,
            alunoId: alunoId,
            cursoId: cursoId,
            valorCobrado: valorCobrado,
            criadaEm: _timeProvider.GetUtcNow(),
            status: StatusMatricula.Ativa
        );

        await _cursoRepository.AtualizarAsync(curso, ct);
        await _matriculaRepository.AdicionarAsync(matricula, ct);

        return matricula;
    }

    public async Task<Matricula> CancelarMatriculaAsync(int matriculaId, CancellationToken ct = default)
    {
        var matricula = await _matriculaRepository.ObterPorIdAsync(matriculaId, ct)
            ?? throw new EntidadeNaoEncontradaException("Matrícula", matriculaId);

        var curso = await _cursoRepository.ObterPorIdAsync(matricula.CursoId, ct)
            ?? throw new EntidadeNaoEncontradaException("Curso", matricula.CursoId);

        matricula.Cancelar();
        curso.LiberarVaga();

        await _matriculaRepository.AtualizarAsync(matricula, ct);
        await _cursoRepository.AtualizarAsync(curso, ct);

        return matricula;
    }

    public async Task<IReadOnlyList<Matricula>> ListarMatriculasAlunoAsync(int alunoId, CancellationToken ct = default)
    {
        _ = await _alunoRepository.ObterPorIdAsync(alunoId, ct)
            ?? throw new EntidadeNaoEncontradaException("Aluno", alunoId);

        return await _matriculaRepository.ObterPorAlunoIdAsync(alunoId, ct);
    }
}

/// <summary>
/// Exemplo prático do padrão Decorator: anexa contagem de operações e logs
/// sem alterar o código do serviço de domínio original.
/// </summary>
public class LoggingMatriculaServiceDecorator : IMatriculaService
{
    private readonly IMatriculaService _inner;
    public int TotalMatriculasRealizadas { get; private set; }
    public int TotalCancelamentosRealizados { get; private set; }

    public LoggingMatriculaServiceDecorator(IMatriculaService inner)
    {
        _inner = inner;
    }

    public async Task<Matricula> MatricularAlunoAsync(int alunoId, int cursoId, CancellationToken ct = default)
    {
        var matricula = await _inner.MatricularAlunoAsync(alunoId, cursoId, ct);
        TotalMatriculasRealizadas++;
        return matricula;
    }

    public async Task<Matricula> CancelarMatriculaAsync(int matriculaId, CancellationToken ct = default)
    {
        var matricula = await _inner.CancelarMatriculaAsync(matriculaId, ct);
        TotalCancelamentosRealizados++;
        return matricula;
    }

    public Task<IReadOnlyList<Matricula>> ListarMatriculasAlunoAsync(int alunoId, CancellationToken ct = default)
    {
        return _inner.ListarMatriculasAlunoAsync(alunoId, ct);
    }
}
