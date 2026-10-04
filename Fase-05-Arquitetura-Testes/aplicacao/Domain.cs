namespace Fase05.GestaoAcademica.Core;

public enum StatusMatricula
{
    Pendente,
    Ativa,
    Cancelada,
    Concluida
}

public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem) { }
}

public class EntidadeNaoEncontradaException : DomainException
{
    public EntidadeNaoEncontradaException(string entidade, object id)
        : base($"{entidade} com identificador '{id}' não foi encontrada.") { }
}

public class VagasEsgotadasException : DomainException
{
    public VagasEsgotadasException(string cursoTitulo)
        : base($"As vagas para o curso '{cursoTitulo}' estão totalmente esgotadas.") { }
}

public class AlunoJaMatriculadoException : DomainException
{
    public AlunoJaMatriculadoException(int alunoId, int cursoId)
        : base($"O aluno ID {alunoId} já possui matrícula ativa ou pendente no curso ID {cursoId}.") { }
}

public record Aluno
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public string Email { get; init; }
    public bool PossuiConvenioEmpresarial { get; init; }

    public Aluno(int id, string nome, string email, bool possuiConvenioEmpresarial = false)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome do aluno não pode ser vazio.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("E-mail do aluno é inválido.");

        Id = id;
        Nome = nome;
        Email = email;
        PossuiConvenioEmpresarial = possuiConvenioEmpresarial;
    }
}

public record Curso
{
    public int Id { get; init; }
    public string Titulo { get; init; }
    public decimal PrecoBase { get; init; }
    public int VagasTotais { get; init; }
    public int VagasOcupadas { get; private set; }

    public Curso(int id, string titulo, decimal precoBase, int vagasTotais, int vagasOcupadas = 0)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("Título do curso é obrigatório.");
        if (precoBase < 0)
            throw new DomainException("Preço do curso não pode ser negativo.");
        if (vagasTotais <= 0)
            throw new DomainException("Total de vagas deve ser maior que zero.");

        Id = id;
        Titulo = titulo;
        PrecoBase = precoBase;
        VagasTotais = vagasTotais;
        VagasOcupadas = vagasOcupadas;
    }

    public bool PossuiVagasDisponiveis => VagasOcupadas < VagasTotais;

    public void OcuparVaga()
    {
        if (!PossuiVagasDisponiveis)
            throw new VagasEsgotadasException(Titulo);

        VagasOcupadas++;
    }

    public void LiberarVaga()
    {
        if (VagasOcupadas > 0)
            VagasOcupadas--;
    }
}

public record Matricula
{
    public int Id { get; init; }
    public int AlunoId { get; init; }
    public int CursoId { get; init; }
    public decimal ValorCobrado { get; init; }
    public StatusMatricula Status { get; private set; }
    public DateTimeOffset CriadaEm { get; init; }

    public Matricula(int id, int alunoId, int cursoId, decimal valorCobrado, DateTimeOffset criadaEm, StatusMatricula status = StatusMatricula.Ativa)
    {
        Id = id;
        AlunoId = alunoId;
        CursoId = cursoId;
        ValorCobrado = valorCobrado;
        CriadaEm = criadaEm;
        Status = status;
    }

    public void Cancelar()
    {
        if (Status == StatusMatricula.Cancelada)
            throw new DomainException("A matrícula já se encontra cancelada.");

        Status = StatusMatricula.Cancelada;
    }
}
