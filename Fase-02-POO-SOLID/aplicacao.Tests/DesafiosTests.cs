using Fase02.PooSolid;
using Xunit;

namespace Fase02.PooSolid.Tests;

public class DesafiosTests
{
    [Theory]
    [InlineData("123.456.789-00", "12345678900", "123.456.789-00")]
    [InlineData("98765432111", "98765432111", "987.654.321-11")]
    public void Cpf_ComValorValido_DeveHigienizarEFormatar(string entrada, string esperadoLimpo, string esperadoFormatado)
    {
        var cpf = new Cpf(entrada);
        Assert.Equal(esperadoLimpo, cpf.ValorLimpo);
        Assert.Equal(esperadoFormatado, cpf.ValorFormatado);
        Assert.Equal(esperadoFormatado, cpf.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1234567890123")]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    public void Cpf_ComValorInvalido_DeveLancarArgumentException(string entrada)
    {
        Assert.Throws<ArgumentException>(() => new Cpf(entrada));
    }

    [Theory]
    [InlineData(1000, 3, 1000)]   // Até 7 dias: 100%
    [InlineData(1000, 7, 1000)]   // Exatamente 7 dias: 100%
    [InlineData(1000, 8, 500)]    // 8 dias: 50%
    [InlineData(1000, 30, 500)]   // 30 dias: 50%
    [InlineData(1000, 31, 0)]     // Acima de 30 dias: 0%
    [InlineData(1000, 60, 0)]
    public void ReembolsoProgressivo_DeveCalcularValorCorreto(decimal valorPago, int dias, decimal esperado)
    {
        var politica = new ReembolsoProgressivoPolitica();
        var resultado = politica.CalcularReembolso(valorPago, dias);
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void Certificados_Polimorfismo_DeveGerarTextosCorretos()
    {
        var aluno = new Aluno("Bruno Costa", "123", "bruno@email.com", new DateOnly(1995, 5, 10));
        var curso = new Curso(Guid.NewGuid(), "Arquitetura Limpa", 40, 500m, NivelCurso.Avancado);
        var hoje = new DateOnly(2026, 10, 1);

        CertificadoBase conclusao = new CertificadoConclusao(aluno, curso, hoje, 9.5m);
        CertificadoBase participacao = new CertificadoParticipacao(aluno, curso, hoje);

        Assert.Contains("concluiu com êxito o curso Arquitetura Limpa com nota 9.5", conclusao.GerarTexto());
        Assert.Contains("participou das atividades do curso Arquitetura Limpa", participacao.GerarTexto());
    }

    [Fact]
    public void CertificadoConclusao_NotaAbaixoDeSete_DeveLancarExcecao()
    {
        var aluno = new Aluno("Bruno", "123", "b@email.com", new DateOnly(1995, 1, 1));
        var curso = new Curso(Guid.NewGuid(), "C#", 10, 100m, NivelCurso.Iniciante);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CertificadoConclusao(aluno, curso, new DateOnly(2026, 1, 1), 6.9m));
    }

    [Fact]
    public void Turma_ControleDeVagas_DeveRespeitarCapacidadeMaxima()
    {
        var curso = new Curso(Guid.NewGuid(), "C# Básico", 20, 200m, NivelCurso.Iniciante);
        var turma = new Turma(Guid.NewGuid(), curso, 2);

        var a1 = new Aluno("Aluno 1", "111", "a1@email.com", new DateOnly(2000, 1, 1));
        var a2 = new Aluno("Aluno 2", "222", "a2@email.com", new DateOnly(2000, 1, 1));
        var a3 = new Aluno("Aluno 3", "333", "a3@email.com", new DateOnly(2000, 1, 1));

        Assert.Equal(2, turma.VagasRestantes);

        Assert.True(turma.MatricularAluno(a1));
        Assert.Equal(1, turma.VagasRestantes);

        Assert.False(turma.MatricularAluno(a1)); // Aluno duplicado
        Assert.Equal(1, turma.VagasRestantes);

        Assert.True(turma.MatricularAluno(a2));
        Assert.Equal(0, turma.VagasRestantes);

        Assert.False(turma.MatricularAluno(a3)); // Turma lotada
        Assert.Equal(0, turma.VagasRestantes);
    }
}
