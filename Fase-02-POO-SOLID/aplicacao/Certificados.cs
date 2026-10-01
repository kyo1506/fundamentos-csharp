using System.Globalization;

namespace Fase02.PooSolid;

public abstract class CertificadoBase(Aluno aluno, Curso curso, DateOnly dataEmissao)
{
    public Aluno Aluno { get; } = aluno ?? throw new ArgumentNullException(nameof(aluno));
    public Curso Curso { get; } = curso ?? throw new ArgumentNullException(nameof(curso));
    public DateOnly DataEmissao { get; } = dataEmissao;

    public abstract string GerarTexto();
}

public class CertificadoConclusao : CertificadoBase
{
    public decimal NotaFinal { get; }

    public CertificadoConclusao(Aluno aluno, Curso curso, DateOnly dataEmissao, decimal notaFinal)
        : base(aluno, curso, dataEmissao)
    {
        if (notaFinal is < 7.0m or > 10.0m)
            throw new ArgumentOutOfRangeException(nameof(notaFinal), "Nota final para conclusão deve ser entre 7.0 e 10.0.");

        NotaFinal = notaFinal;
    }

    public override string GerarTexto() =>
        $"Certificamos que {Aluno.Nome} concluiu com êxito o curso {Curso.Titulo} com nota {NotaFinal.ToString("F1", CultureInfo.InvariantCulture)}.";
}

public class CertificadoParticipacao : CertificadoBase
{
    public CertificadoParticipacao(Aluno aluno, Curso curso, DateOnly dataEmissao)
        : base(aluno, curso, dataEmissao) {}

    public override string GerarTexto() =>
        $"Certificamos que {Aluno.Nome} participou das atividades do curso {Curso.Titulo}.";
}
