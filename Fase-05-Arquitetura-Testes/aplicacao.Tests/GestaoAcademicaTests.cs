using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fase05.GestaoAcademica.Core.Tests;

public class GestaoAcademicaTests
{
    // ------------------------------------------------------------------------
    // 1. Testes de Domínio & Validações Iniciais
    // ------------------------------------------------------------------------

    [Theory]
    [InlineData("", "aluno@teste.com")]
    [InlineData("   ", "aluno@teste.com")]
    [InlineData("Carlos", "")]
    [InlineData("Carlos", "email-sem-arroba")]
    public void Aluno_ComDadosInvalidos_DeveLancarDomainException(string nome, string email)
    {
        Assert.Throws<DomainException>(() => new Aluno(1, nome, email));
    }

    [Theory]
    [InlineData("", 100, 10)]
    [InlineData("Curso C#", -10, 10)]
    [InlineData("Curso C#", 100, 0)]
    [InlineData("Curso C#", 100, -5)]
    public void Curso_ComDadosInvalidos_DeveLancarDomainException(string titulo, decimal preco, int vagas)
    {
        Assert.Throws<DomainException>(() => new Curso(1, titulo, preco, vagas));
    }

    [Fact]
    public void Curso_OcuparVaga_QuandoEsgotado_DeveLancarVagasEsgotadasException()
    {
        // Arrange
        var curso = new Curso(1, "DDD na Prática", 500m, vagasTotais: 1);
        curso.OcuparVaga();

        // Act & Assert
        Assert.Throws<VagasEsgotadasException>(() => curso.OcuparVaga());
    }

    [Fact]
    public void Curso_LiberarVaga_DeveDecrementarVagasOcupadas()
    {
        // Arrange
        var curso = new Curso(1, "DDD na Prática", 500m, vagasTotais: 2);
        curso.OcuparVaga();
        Assert.Equal(1, curso.VagasOcupadas);

        // Act
        curso.LiberarVaga();

        // Assert
        Assert.Equal(0, curso.VagasOcupadas);
    }

    // ------------------------------------------------------------------------
    // 2. Testes Parametrizados de Políticas de Desconto (Strategy)
    // ------------------------------------------------------------------------

    [Theory]
    [InlineData(1000, true, 250)]
    [InlineData(1000, false, 0)]
    [InlineData(500, true, 125)]
    public void DescontoConvenioPolicy_DeveCalcularDescontoBaseadoNoConvenio(decimal precoBase, bool temConvenio, decimal descontoEsperado)
    {
        var aluno = new Aluno(1, "Aluno Teste", "aluno@teste.com", temConvenio);
        var policy = new DescontoConvenioPolicy();

        decimal desconto = policy.CalcularDesconto(precoBase, aluno);

        Assert.Equal(descontoEsperado, desconto);
    }

    [Theory]
    [InlineData(1000, 100)]
    [InlineData(250, 25)]
    public void DescontoPromocionalPolicy_DeveCalcularDezPorCento(decimal precoBase, decimal descontoEsperado)
    {
        var aluno = new Aluno(1, "Aluno Teste", "aluno@teste.com");
        var policy = new DescontoPromocionalPolicy();

        decimal desconto = policy.CalcularDesconto(precoBase, aluno);

        Assert.Equal(descontoEsperado, desconto);
    }

    // ------------------------------------------------------------------------
    // 3. Testes de Caso de Uso (IMatriculaService com Fakes)
    // ------------------------------------------------------------------------

    private static (MatriculaService service, IAlunoRepository alunoRepo, ICursoRepository cursoRepo, IMatriculaRepository matRepo)
        CriarServicoTeste(IPoliticaDesconto? policy = null)
    {
        var alunoRepo = new InMemoryAlunoRepository();
        var cursoRepo = new InMemoryCursoRepository();
        var matRepo = new InMemoryMatriculaRepository();
        var p = policy ?? new SemDescontoPolicy();
        var service = new MatriculaService(alunoRepo, cursoRepo, matRepo, p);
        return (service, alunoRepo, cursoRepo, matRepo);
    }

    [Fact]
    public async Task MatricularAlunoAsync_QuandoDadosValidos_DeveCriarMatriculaEOcuparVaga()
    {
        // Arrange
        var (service, alunoRepo, cursoRepo, matRepo) = CriarServicoTeste(new DescontoPromocionalPolicy());
        var aluno = new Aluno(10, "Fernanda", "fernanda@teste.com");
        var curso = new Curso(20, "Clean Code", 400m, vagasTotais: 5);

        await alunoRepo.AdicionarAsync(aluno);
        await cursoRepo.AdicionarAsync(curso);

        // Act
        var matricula = await service.MatricularAlunoAsync(aluno.Id, curso.Id);

        // Assert
        Assert.NotNull(matricula);
        Assert.Equal(aluno.Id, matricula.AlunoId);
        Assert.Equal(curso.Id, matricula.CursoId);
        Assert.Equal(360m, matricula.ValorCobrado); // 400 - 10% = 360
        Assert.Equal(StatusMatricula.Ativa, matricula.Status);

        var cursoAtualizado = await cursoRepo.ObterPorIdAsync(curso.Id);
        Assert.Equal(1, cursoAtualizado!.VagasOcupadas);

        var salvo = await matRepo.ObterPorIdAsync(matricula.Id);
        Assert.NotNull(salvo);
    }

    [Fact]
    public async Task MatricularAlunoAsync_QuandoAlunoNaoExiste_DeveLancarExcecao()
    {
        // Arrange
        var (service, _, cursoRepo, _) = CriarServicoTeste();
        var curso = new Curso(1, "Curso", 100m, 5);
        await cursoRepo.AdicionarAsync(curso);

        // Act & Assert
        await Assert.ThrowsAsync<EntidadeNaoEncontradaException>(() =>
            service.MatricularAlunoAsync(alunoId: 999, cursoId: curso.Id));
    }

    [Fact]
    public async Task MatricularAlunoAsync_QuandoJaMatriculado_DeveLancarExcecao()
    {
        // Arrange
        var (service, alunoRepo, cursoRepo, _) = CriarServicoTeste();
        var aluno = new Aluno(1, "Aluno", "aluno@email.com");
        var curso = new Curso(1, "Curso", 100m, 5);
        await alunoRepo.AdicionarAsync(aluno);
        await cursoRepo.AdicionarAsync(curso);

        await service.MatricularAlunoAsync(aluno.Id, curso.Id);

        // Act & Assert
        await Assert.ThrowsAsync<AlunoJaMatriculadoException>(() =>
            service.MatricularAlunoAsync(aluno.Id, curso.Id));
    }

    [Fact]
    public async Task CancelarMatriculaAsync_DeveAtualizarStatusELiberarVaga()
    {
        // Arrange
        var (service, alunoRepo, cursoRepo, _) = CriarServicoTeste();
        var aluno = new Aluno(1, "Aluno", "aluno@email.com");
        var curso = new Curso(1, "Curso", 100m, 5);
        await alunoRepo.AdicionarAsync(aluno);
        await cursoRepo.AdicionarAsync(curso);

        var matricula = await service.MatricularAlunoAsync(aluno.Id, curso.Id);
        var cursoAposMatricula = await cursoRepo.ObterPorIdAsync(curso.Id);
        Assert.Equal(1, cursoAposMatricula!.VagasOcupadas);

        // Act
        var cancelada = await service.CancelarMatriculaAsync(matricula.Id);

        // Assert
        Assert.Equal(StatusMatricula.Cancelada, cancelada.Status);
        var cursoAposCancelamento = await cursoRepo.ObterPorIdAsync(curso.Id);
        Assert.Equal(0, cursoAposCancelamento!.VagasOcupadas);
    }

    // ------------------------------------------------------------------------
    // 4. Testes do Padrão Decorator
    // ------------------------------------------------------------------------

    [Fact]
    public async Task LoggingMatriculaServiceDecorator_DeveIncrementarMetricasCorretamente()
    {
        // Arrange
        var (service, alunoRepo, cursoRepo, _) = CriarServicoTeste();
        var decorator = new LoggingMatriculaServiceDecorator(service);

        var aluno = new Aluno(1, "Aluno", "aluno@email.com");
        var curso = new Curso(1, "Curso", 100m, 5);
        await alunoRepo.AdicionarAsync(aluno);
        await cursoRepo.AdicionarAsync(curso);

        // Act
        var mat = await decorator.MatricularAlunoAsync(aluno.Id, curso.Id);
        await decorator.CancelarMatriculaAsync(mat.Id);

        // Assert
        Assert.Equal(1, decorator.TotalMatriculasRealizadas);
        Assert.Equal(1, decorator.TotalCancelamentosRealizados);
    }

    // ------------------------------------------------------------------------
    // 5. Testes do Container de Injeção de Dependência (IoC)
    // ------------------------------------------------------------------------

    [Fact]
    public void DependencyInjection_DeveResolverTodosOsServicosEScopesSemErros()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddGestaoAcademicaCore();

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        // Act & Assert
        using (var scope = provider.CreateScope())
        {
            var alunoRepo = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();
            var cursoRepo = scope.ServiceProvider.GetRequiredService<ICursoRepository>();
            var matRepo = scope.ServiceProvider.GetRequiredService<IMatriculaRepository>();
            var service = scope.ServiceProvider.GetRequiredService<IMatriculaService>();
            var policy = scope.ServiceProvider.GetRequiredService<IPoliticaDesconto>();

            Assert.NotNull(alunoRepo);
            Assert.NotNull(cursoRepo);
            Assert.NotNull(matRepo);
            Assert.NotNull(service);
            Assert.NotNull(policy);
        }
    }
}
