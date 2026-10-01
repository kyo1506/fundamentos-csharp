namespace Fase02.PooSolid;

public enum NivelCurso
{
    Iniciante,
    Intermediario,
    Avancado
}

public enum StatusMatricula
{
    Ativa,
    Concluida,
    Cancelada
}

public abstract class Pessoa
{
    private string _email = string.Empty;

    public string Nome { get; }
    public string Documento { get; }

    public string Email
    {
        get => _email;
        init
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                throw new ArgumentException("E-mail inválido.", nameof(value));
            _email = value.Trim().ToLowerInvariant();
        }
    }

    protected Pessoa(string nome, string documento, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

        if (string.IsNullOrWhiteSpace(documento))
            throw new ArgumentException("Documento não pode ser vazio.", nameof(documento));

        Nome = nome.Trim();
        Documento = documento.Trim();
        Email = email;
    }

    public virtual string ObterDescricao() => $"{Nome} (Doc: {Documento}, Email: {Email})";
}

public class Aluno : Pessoa
{
    public DateOnly DataNascimento { get; }

    public Aluno(string nome, string documento, string email, DateOnly dataNascimento)
        : base(nome, documento, email)
    {
        if (dataNascimento > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Data de nascimento não pode estar no futuro.", nameof(dataNascimento));

        DataNascimento = dataNascimento;
    }

    public int CalcularIdade(DateOnly referencia)
    {
        var idade = referencia.Year - DataNascimento.Year;
        if (referencia < DataNascimento.AddYears(idade))
            idade--;
        return idade;
    }

    public override string ObterDescricao() => $"Aluno: {Nome} | Nascido em: {DataNascimento:dd/MM/yyyy}";
}

public class Instrutor : Pessoa
{
    public string Especialidade { get; }

    public Instrutor(string nome, string documento, string email, string especialidade)
        : base(nome, documento, email)
    {
        if (string.IsNullOrWhiteSpace(especialidade))
            throw new ArgumentException("Especialidade não pode ser vazia.", nameof(especialidade));

        Especialidade = especialidade.Trim();
    }

    public override string ObterDescricao() => $"Instrutor: {Nome} | Especialidade: {Especialidade}";
}

public class Curso
{
    public Guid Id { get; }
    public string Titulo { get; }
    public int CargaHoraria { get; }
    public decimal PrecoBase { get; }
    public NivelCurso Nivel { get; }

    public Curso(Guid id, string titulo, int cargaHoraria, decimal precoBase, NivelCurso nivel)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id inválido.", nameof(id));

        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Título não pode ser vazio.", nameof(titulo));

        if (cargaHoraria <= 0)
            throw new ArgumentOutOfRangeException(nameof(cargaHoraria), "Carga horária deve ser positiva.");

        if (precoBase < 0)
            throw new ArgumentOutOfRangeException(nameof(precoBase), "Preço base não pode ser negativo.");

        Id = id;
        Titulo = titulo.Trim();
        CargaHoraria = cargaHoraria;
        PrecoBase = precoBase;
        Nivel = nivel;
    }
}

public class Matricula
{
    public Guid Id { get; }
    public Aluno Aluno { get; }
    public Curso Curso { get; }
    public DateTime DataMatricula { get; }
    public decimal ValorCobrado { get; }
    public StatusMatricula Status { get; private set; }
    public string? MotivoCancelamento { get; private set; }

    public Matricula(Guid id, Aluno aluno, Curso curso, decimal valorCobrado, DateTime? dataMatricula = null)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id inválido.", nameof(id));

        ArgumentNullException.ThrowIfNull(aluno);
        ArgumentNullException.ThrowIfNull(curso);

        if (valorCobrado < 0)
            throw new ArgumentOutOfRangeException(nameof(valorCobrado), "Valor cobrado não pode ser negativo.");

        Id = id;
        Aluno = aluno;
        Curso = curso;
        ValorCobrado = valorCobrado;
        DataMatricula = dataMatricula ?? DateTime.UtcNow;
        Status = StatusMatricula.Ativa;
    }

    public void Concluir()
    {
        if (Status != StatusMatricula.Ativa)
            throw new InvalidOperationException($"Não é possível concluir matrícula com status {Status}.");

        Status = StatusMatricula.Concluida;
    }

    public void Cancelar(string motivo)
    {
        if (Status != StatusMatricula.Ativa)
            throw new InvalidOperationException($"Não é possível cancelar matrícula com status {Status}.");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new ArgumentException("Motivo de cancelamento é obrigatório.", nameof(motivo));

        Status = StatusMatricula.Cancelada;
        MotivoCancelamento = motivo.Trim();
    }
}
