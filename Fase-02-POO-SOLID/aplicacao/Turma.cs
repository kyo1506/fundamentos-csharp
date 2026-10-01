namespace Fase02.PooSolid;

public class Turma
{
    private readonly List<Aluno> _alunos = new();

    public Guid Id { get; }
    public Curso Curso { get; }
    public int CapacidadeMaxima { get; }
    public IReadOnlyList<Aluno> AlunosMatriculados => _alunos.AsReadOnly();
    public int VagasRestantes => CapacidadeMaxima - _alunos.Count;

    public Turma(Guid id, Curso curso, int capacidadeMaxima)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id inválido.", nameof(id));
        Curso = curso ?? throw new ArgumentNullException(nameof(curso));
        if (capacidadeMaxima <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacidadeMaxima), "Capacidade máxima deve ser positiva.");

        Id = id;
        CapacidadeMaxima = capacidadeMaxima;
    }

    public bool MatricularAluno(Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(aluno);

        if (_alunos.Count >= CapacidadeMaxima)
            return false;

        if (_alunos.Any(a => a.Documento.Equals(aluno.Documento, StringComparison.OrdinalIgnoreCase)))
            return false;

        _alunos.Add(aluno);
        return true;
    }
}
