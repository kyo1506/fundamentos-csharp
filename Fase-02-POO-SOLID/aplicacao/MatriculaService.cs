namespace Fase02.PooSolid;

public class MatriculaService(
    IMatriculaRepository matriculaRepo,
    ICursoRepository cursoRepo,
    INotificador notificador)
{
    private readonly IMatriculaRepository _matriculaRepo = matriculaRepo ?? throw new ArgumentNullException(nameof(matriculaRepo));
    private readonly ICursoRepository _cursoRepo = cursoRepo ?? throw new ArgumentNullException(nameof(cursoRepo));
    private readonly INotificador _notificador = notificador ?? throw new ArgumentNullException(nameof(notificador));

    public Matricula Matricular(Aluno aluno, Guid cursoId, IDescontoPolicy? descontoPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);

        var curso = _cursoRepo.ObterPorId(cursoId)
            ?? throw new InvalidOperationException($"Curso com Id '{cursoId}' não encontrado.");

        var matriculasAluno = _matriculaRepo.ListarPorAluno(aluno.Documento);
        var jaMatriculado = matriculasAluno.Any(m => m.Curso.Id == cursoId && m.Status == StatusMatricula.Ativa);
        if (jaMatriculado)
            throw new InvalidOperationException($"O aluno '{aluno.Nome}' já possui matrícula ativa no curso '{curso.Titulo}'.");

        var policy = descontoPolicy ?? new SemDescontoPolicy();
        var valorFinal = policy.CalcularValorFinal(curso.PrecoBase);

        var matricula = new Matricula(Guid.NewGuid(), aluno, curso, valorFinal);
        _matriculaRepo.Salvar(matricula);

        _notificador.Notificar(
            aluno.Email,
            $"Matrícula Confirmada: {curso.Titulo}",
            $"Olá {aluno.Nome}, sua matrícula no curso '{curso.Titulo}' foi confirmada por R$ {valorFinal:N2} (Regra: {policy.NomeRegra}).");

        return matricula;
    }

    public void CancelarMatricula(Guid matriculaId, string motivo)
    {
        var matricula = _matriculaRepo.ObterPorId(matriculaId)
            ?? throw new InvalidOperationException($"Matrícula com Id '{matriculaId}' não encontrada.");

        matricula.Cancelar(motivo);
        _matriculaRepo.Salvar(matricula);

        _notificador.Notificar(
            matricula.Aluno.Email,
            $"Cancelamento de Matrícula: {matricula.Curso.Titulo}",
            $"Sua matrícula foi cancelada. Motivo: {motivo}");
    }
}
