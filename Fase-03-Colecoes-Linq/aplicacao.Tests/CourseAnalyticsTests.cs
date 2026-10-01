using Fase02.PooSolid;
using Fase03.CourseAnalytics;
using Xunit;

namespace Fase03.CourseAnalytics.Tests;

public class CourseAnalyticsTests
{
    private static (List<Curso> Cursos, List<Matricula> Matriculas) CriarDadosTeste()
    {
        var c1 = new Curso(Guid.NewGuid(), "C# Básico", 20, 200m, NivelCurso.Iniciante);
        var c2 = new Curso(Guid.NewGuid(), "POO Avançado", 30, 400m, NivelCurso.Intermediario);
        var c3 = new Curso(Guid.NewGuid(), "Arquitetura", 40, 800m, NivelCurso.Avancado);

        var a1 = new Aluno("Ana", "111", "ana@email.com", new DateOnly(2000, 1, 1));
        var a2 = new Aluno("Bia", "222", "bia@email.com", new DateOnly(1990, 1, 1));
        var a3 = new Aluno("Caio", "333", "caio@email.com", new DateOnly(1980, 1, 1));

        var m1 = new Matricula(Guid.NewGuid(), a1, c1, 200m);
        var m2 = new Matricula(Guid.NewGuid(), a2, c2, 400m);
        var m3 = new Matricula(Guid.NewGuid(), a3, c2, 400m);
        var m4 = new Matricula(Guid.NewGuid(), a1, c3, 800m);

        return (new List<Curso> { c1, c2, c3 }, new List<Matricula> { m1, m2, m3, m4 });
    }

    [Fact]
    public void RepositorioGenerico_AdicionarEFiltrar_DeveFuncionarComTipoReferencia()
    {
        var repo = new RepositorioGenerico<Curso, Guid>();
        var c1 = new Curso(Guid.NewGuid(), "C# Fundamentos", 20, 200m, NivelCurso.Iniciante);
        var c2 = new Curso(Guid.NewGuid(), "C# Avançado", 40, 500m, NivelCurso.Avancado);

        repo.Adicionar(c1);
        repo.Adicionar(c2);

        Assert.Equal(2, repo.Contar());
        var encontrado = repo.ObterPorChave(c => c.Id, c1.Id);
        Assert.NotNull(encontrado);
        Assert.Equal("C# Fundamentos", encontrado!.Titulo);

        var avancados = repo.Filtrar(c => c.Nivel == NivelCurso.Avancado).ToList();
        Assert.Single(avancados);
        Assert.Equal(c2.Id, avancados[0].Id);
    }

    [Fact]
    public void AnalyticsEventHub_PublicarMatricula_DeveNotificarSubscribers()
    {
        var hub = new AnalyticsEventHub();
        int chamadas = 0;
        string? ultimoAluno = null;

        hub.AoRegistrarMatricula += (sender, e) =>
        {
            chamadas++;
            ultimoAluno = e.Matricula.Aluno.Nome;
        };

        var curso = new Curso(Guid.NewGuid(), "C#", 10, 100m, NivelCurso.Iniciante);
        var aluno = new Aluno("Marcos", "123", "m@email.com", new DateOnly(2000, 1, 1));
        var matricula = new Matricula(Guid.NewGuid(), aluno, curso, 100m);

        hub.PublicarMatricula(matricula);

        Assert.Equal(1, chamadas);
        Assert.Equal("Marcos", ultimoAluno);
    }

    [Fact]
    public void AnalyticsEventHub_AlertaLotacao_DeveDispararEvento()
    {
        var hub = new AnalyticsEventHub();
        bool disparado = false;

        hub.AoLotarTurma += (sender, e) =>
        {
            if (e.NomeCurso == "Docker" && e.Capacidade == 30)
                disparado = true;
        };

        hub.PublicarAlertaLotacao("Docker", 30);
        Assert.True(disparado);
    }

    [Fact]
    public void AnalyticsEngine_ReceitaPorNivel_DeveCalcularAgrupamentoEMedias()
    {
        var (_, matriculas) = CriarDadosTeste();

        var relatorio = AnalyticsEngine.ReceitaPorNivel(matriculas);

        Assert.Equal(3, relatorio.Count);
        // O nível com maior receita deve vir primeiro:
        // Intermediário: 2 matrículas de 400 = 800
        // Avançado: 1 matrícula de 800 = 800
        // Iniciante: 1 matrícula de 200 = 200
        var iniciante = relatorio.First(r => r.Nivel == NivelCurso.Iniciante);
        Assert.Equal(1, iniciante.TotalMatriculas);
        Assert.Equal(200m, iniciante.ReceitaTotal);
        Assert.Equal(200m, iniciante.TicketMedio);

        var intermediario = relatorio.First(r => r.Nivel == NivelCurso.Intermediario);
        Assert.Equal(2, intermediario.TotalMatriculas);
        Assert.Equal(800m, intermediario.ReceitaTotal);
        Assert.Equal(400m, intermediario.TicketMedio);
    }

    [Fact]
    public void AnalyticsEngine_TopCursos_DeveOrdenarPorTotalMatriculas()
    {
        var (_, matriculas) = CriarDadosTeste();

        var top = AnalyticsEngine.TopCursos(matriculas, top: 2);

        Assert.Equal(2, top.Count);
        Assert.Equal("POO Avançado", top[0].TituloCurso); // 2 matrículas
        Assert.Equal(2, top[0].TotalMatriculas);
    }

    [Fact]
    public void AnalyticsEngine_CalcularIdadeMediaAlunos_DeveCalcularMediaCorreta()
    {
        var (_, matriculas) = CriarDadosTeste();
        var dataRef = new DateOnly(2026, 1, 1);

        // a1 (nasc 2000): 26 anos (aparece 2 vezes: m1 e m4)
        // a2 (nasc 1990): 36 anos
        // a3 (nasc 1980): 46 anos
        // Total idades: 26 + 36 + 46 + 26 = 134 / 4 = 33.5
        var media = AnalyticsEngine.CalcularIdadeMediaAlunos(matriculas, dataRef);
        Assert.Equal(33.5m, media);
    }

    [Fact]
    public void AnalyticsEngine_FormatarListaCursos_DeveUtilizarAggregate()
    {
        var (cursos, _) = CriarDadosTeste();

        var texto = AnalyticsEngine.FormatarListaCursos(cursos);

        Assert.Equal("C# Básico | POO Avançado | Arquitetura", texto);
    }

    [Fact]
    public void AnalyticsEngine_ObterPagina_DevePaginarEFiltrar()
    {
        var (_, matriculas) = CriarDadosTeste();

        // 4 matrículas no total: página 1 com 2 itens
        var pagina1 = AnalyticsEngine.ObterPagina(matriculas, pagina: 1, tamanhoPagina: 2);
        Assert.Equal(2, pagina1.Count);

        // Página 2 com 2 itens
        var pagina2 = AnalyticsEngine.ObterPagina(matriculas, pagina: 2, tamanhoPagina: 2);
        Assert.Equal(2, pagina2.Count);

        // Página 3 vazia
        var pagina3 = AnalyticsEngine.ObterPagina(matriculas, pagina: 3, tamanhoPagina: 2);
        Assert.Empty(pagina3);

        // Com filtro: apenas matrículas de valor >= 400
        var filtradas = AnalyticsEngine.ObterPagina(matriculas, pagina: 1, tamanhoPagina: 5, m => m.ValorCobrado >= 400m);
        Assert.Equal(3, filtradas.Count);
    }

    [Fact]
    public void AnalyticsEngine_ParticionarSobDemanda_DeveGerarLotesCorretosComYield()
    {
        var itens = new List<int> { 1, 2, 3, 4, 5 };

        var lotes = AnalyticsEngine.ParticionarSobDemanda(itens, tamanhoLote: 2).ToList();

        Assert.Equal(3, lotes.Count);
        Assert.Equal(new[] { 1, 2 }, lotes[0]);
        Assert.Equal(new[] { 3, 4 }, lotes[1]);
        Assert.Equal(new[] { 5 }, lotes[2]);
    }
}
