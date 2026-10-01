namespace Fase02.PooSolid;

public interface IMatriculaRepository
{
    Matricula? ObterPorId(Guid id);
    IReadOnlyList<Matricula> ListarPorAluno(string documentoAluno);
    void Salvar(Matricula matricula);
}

public interface ICursoRepository
{
    Curso? ObterPorId(Guid id);
    IReadOnlyList<Curso> ListarTodos();
    void Salvar(Curso curso);
}

public interface INotificador
{
    void Notificar(string destinatario, string assunto, string mensagem);
}

public interface IExportadorMatricula
{
    string Formatar(Matricula matricula);
}
