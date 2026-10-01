using Fase02.PooSolid;
using Xunit;

namespace Fase02.PooSolid.Tests;

public class PooSolidTests
{
    [Fact]
    public void Aluno_ComDadosValidos_DeveInstanciarCorretamente()
    {
        var dataNascimento = new DateOnly(2000, 5, 20);
        var aluno = new Aluno("João Pereira", "111.222.333-44", "joao@email.com", dataNascimento);

        Assert.Equal("João Pereira", aluno.Nome);
        Assert.Equal("111.222.333-44", aluno.Documento);
        Assert.Equal("joao@email.com", aluno.Email);
        Assert.Equal(dataNascimento, aluno.DataNascimento);
    }

    [Theory]
    [InlineData("", "111", "email@teste.com")]
    [InlineData("   ", "111", "email@teste.com")]
    [InlineData("Nome", "", "email@teste.com")]
    [InlineData("Nome", "111", "sem-arroba")]
    [InlineData("Nome", "111", "")]
    public void Aluno_ComDadosInvalidos_DeveLancarArgumentException(string nome, string doc, string email)
    {
        var dataNascimento = new DateOnly(2000, 1, 1);
        Assert.Throws<ArgumentException>(() => new Aluno(nome, doc, email, dataNascimento));
    }

    [Fact]
    public void Aluno_ComDataNascimentoNoFuturo_DeveLancarArgumentException()
    {
        var futuro = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        Assert.Throws<ArgumentException>(() => new Aluno("Nome", "111", "teste@email.com", futuro));
    }

    [Fact]
    public void Aluno_CalcularIdade_DeveRetornarIdadeExata()
    {
        var aluno = new Aluno("Pedro", "123", "p@email.com", new DateOnly(2000, 10, 15));
        var hojeAntesAniversario = new DateOnly(2026, 10, 14);
        var hojeNoAniversario = new DateOnly(2026, 10, 15);

        Assert.Equal(25, aluno.CalcularIdade(hojeAntesAniversario));
        Assert.Equal(26, aluno.CalcularIdade(hojeNoAniversario));
    }

    [Fact]
    public void Instrutor_ComDadosValidos_DeveInstanciarEPolimorfismoFuncionar()
    {
        Pessoa instrutor = new Instrutor("Prof. Roberto", "444.555.666-77", "roberto@escola.com", "C# e Arquitetura");

        Assert.Contains("Instrutor: Prof. Roberto", instrutor.ObterDescricao());
        Assert.Contains("C# e Arquitetura", instrutor.ObterDescricao());
    }

    [Fact]
    public void Curso_ComDadosValidos_DeveCriarCorretamente()
    {
        var id = Guid.NewGuid();
        var curso = new Curso(id, "Design Patterns", 30, 450.00m, NivelCurso.Intermediario);

        Assert.Equal(id, curso.Id);
        Assert.Equal("Design Patterns", curso.Titulo);
        Assert.Equal(30, curso.CargaHoraria);
        Assert.Equal(450.00m, curso.PrecoBase);
        Assert.Equal(NivelCurso.Intermediario, curso.Nivel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Curso_CargaHorariaInvalida_DeveLancarExcecao(int cargaHoraria)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Curso(Guid.NewGuid(), "Titulo", cargaHoraria, 100m, NivelCurso.Iniciante));
    }

    [Fact]
    public void DescontoEstudante_DeveAplicar20Porcento()
    {
        var policy = new DescontoEstudantePolicy();
        var valorFinal = policy.CalcularValorFinal(500m);

        Assert.Equal(400m, valorFinal);
    }

    [Fact]
    public void DescontoVip_DeveAplicar30Porcento()
    {
        var policy = new DescontoVipPolicy();
        var valorFinal = policy.CalcularValorFinal(1000m);

        Assert.Equal(700m, valorFinal);
    }

    [Fact]
    public void DescontoValorFixo_DeveSubtrairValor()
    {
        var policy = new DescontoValorFixoPolicy(50m);
        var valorFinal = policy.CalcularValorFinal(200m);

        Assert.Equal(150m, valorFinal);
    }

    [Fact]
    public void Matricula_ConcluirECancelar_DeveAtualizarStatus()
    {
        var aluno = new Aluno("Ana", "123", "ana@email.com", new DateOnly(2000, 1, 1));
        var curso = new Curso(Guid.NewGuid(), "C#", 10, 100m, NivelCurso.Iniciante);
        var matricula = new Matricula(Guid.NewGuid(), aluno, curso, 100m);

        Assert.Equal(StatusMatricula.Ativa, matricula.Status);

        matricula.Concluir();
        Assert.Equal(StatusMatricula.Concluida, matricula.Status);

        // Não pode cancelar após concluída
        Assert.Throws<InvalidOperationException>(() => matricula.Cancelar("Desisti"));
    }

    [Fact]
    public void MatriculaService_MatricularComSucesso_DeveSalvarENotificar()
    {
        var matriculaRepo = new MatriculaRepositoryEmMemoria();
        var cursoRepo = new CursoRepositoryEmMemoria();
        var notificador = new NotificadorConsole();
        var service = new MatriculaService(matriculaRepo, cursoRepo, notificador);

        var curso = new Curso(Guid.NewGuid(), "POO Avançada", 20, 500m, NivelCurso.Intermediario);
        cursoRepo.Salvar(curso);

        var aluno = new Aluno("Carla", "999", "carla@email.com", new DateOnly(1995, 3, 10));
        var matricula = service.Matricular(aluno, curso.Id, new DescontoEstudantePolicy());

        Assert.NotNull(matricula);
        Assert.Equal(400m, matricula.ValorCobrado);
        Assert.Equal(StatusMatricula.Ativa, matricula.Status);

        var salvas = matriculaRepo.ListarPorAluno("999");
        Assert.Single(salvas);

        Assert.Single(notificador.Historico);
        Assert.Equal("carla@email.com", notificador.Historico[0].Destinatario);
    }

    [Fact]
    public void MatriculaService_AlunoJaMatriculado_DeveLancarExcecao()
    {
        var matriculaRepo = new MatriculaRepositoryEmMemoria();
        var cursoRepo = new CursoRepositoryEmMemoria();
        var notificador = new NotificadorConsole();
        var service = new MatriculaService(matriculaRepo, cursoRepo, notificador);

        var curso = new Curso(Guid.NewGuid(), "POO Avançada", 20, 500m, NivelCurso.Intermediario);
        cursoRepo.Salvar(curso);

        var aluno = new Aluno("Carla", "999", "carla@email.com", new DateOnly(1995, 3, 10));
        service.Matricular(aluno, curso.Id);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.Matricular(aluno, curso.Id));

        Assert.Contains("já possui matrícula ativa", ex.Message);
    }

    [Fact]
    public void MatriculaService_CancelarMatricula_DeveAtualizarRepoENotificar()
    {
        var matriculaRepo = new MatriculaRepositoryEmMemoria();
        var cursoRepo = new CursoRepositoryEmMemoria();
        var notificador = new NotificadorConsole();
        var service = new MatriculaService(matriculaRepo, cursoRepo, notificador);

        var curso = new Curso(Guid.NewGuid(), "POO Avançada", 20, 500m, NivelCurso.Intermediario);
        cursoRepo.Salvar(curso);
        var aluno = new Aluno("Carla", "999", "carla@email.com", new DateOnly(1995, 3, 10));
        var matricula = service.Matricular(aluno, curso.Id);

        service.CancelarMatricula(matricula.Id, "Mudança de cidade");

        var atualizada = matriculaRepo.ObterPorId(matricula.Id);
        Assert.NotNull(atualizada);
        Assert.Equal(StatusMatricula.Cancelada, atualizada.Status);
        Assert.Equal("Mudança de cidade", atualizada.MotivoCancelamento);
        Assert.Equal(2, notificador.Historico.Count); // 1 matricula + 1 cancelamento
    }

    [Fact]
    public void ExportadorMatriculaCsv_DeveFormatarCorretamente()
    {
        var exportador = new ExportadorMatriculaCsv();
        var aluno = new Aluno("Marcos", "111222", "m@email.com", new DateOnly(1990, 1, 1));
        var curso = new Curso(Guid.NewGuid(), "C# Fundamentos", 10, 150m, NivelCurso.Iniciante);
        var matricula = new Matricula(Guid.NewGuid(), aluno, curso, 150m);

        var csv = exportador.Formatar(matricula);
        Assert.Contains("Marcos", csv);
        Assert.Contains("111222", csv);
        Assert.Contains("C# Fundamentos", csv);
        Assert.Contains("150.00", csv);
        Assert.Contains("Ativa", csv);
    }
}
