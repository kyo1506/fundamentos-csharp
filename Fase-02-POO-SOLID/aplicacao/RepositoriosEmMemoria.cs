namespace Fase02.PooSolid;

public class MatriculaRepositoryEmMemoria : IMatriculaRepository
{
    private readonly List<Matricula> _matriculas = new();

    public Matricula? ObterPorId(Guid id) => _matriculas.FirstOrDefault(m => m.Id == id);

    public IReadOnlyList<Matricula> ListarPorAluno(string documentoAluno) =>
        _matriculas.Where(m => m.Aluno.Documento.Equals(documentoAluno, StringComparison.OrdinalIgnoreCase)).ToList();

    public void Salvar(Matricula matricula)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        var index = _matriculas.FindIndex(m => m.Id == matricula.Id);
        if (index >= 0)
            _matriculas[index] = matricula;
        else
            _matriculas.Add(matricula);
    }
}

public class CursoRepositoryEmMemoria : ICursoRepository
{
    private readonly List<Curso> _cursos = new();

    public Curso? ObterPorId(Guid id) => _cursos.FirstOrDefault(c => c.Id == id);

    public IReadOnlyList<Curso> ListarTodos() => _cursos.AsReadOnly();

    public void Salvar(Curso curso)
    {
        ArgumentNullException.ThrowIfNull(curso);
        var index = _cursos.FindIndex(c => c.Id == curso.Id);
        if (index >= 0)
            _cursos[index] = curso;
        else
            _cursos.Add(curso);
    }
}

public class NotificadorConsole : INotificador
{
    public List<(string Destinatario, string Assunto, string Mensagem)> Historico { get; } = new();

    public void Notificar(string destinatario, string assunto, string mensagem)
    {
        Historico.Add((destinatario, assunto, mensagem));
        Console.WriteLine($"[NOTIFICACAO para {destinatario}] {assunto} -> {mensagem}");
    }
}

public class ExportadorMatriculaCsv : IExportadorMatricula
{
    public string Formatar(Matricula matricula)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        return $"{matricula.Id};{matricula.Aluno.Nome};{matricula.Aluno.Documento};{matricula.Curso.Titulo};{matricula.ValorCobrado.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)};{matricula.Status}";
    }
}
