using System.Globalization;
using Fase02.PooSolid;

namespace Fase03.CourseAnalytics;

public record ResumoNivelReceita(NivelCurso Nivel, int TotalMatriculas, decimal ReceitaTotal, decimal TicketMedio);

public record CursoPopularidade(string TituloCurso, int TotalMatriculas, decimal TotalArrecadado);

public static class AnalyticsEngine
{
    public static IReadOnlyList<ResumoNivelReceita> ReceitaPorNivel(IEnumerable<Matricula> matriculas)
    {
        ArgumentNullException.ThrowIfNull(matriculas);

        return matriculas
            .GroupBy(m => m.Curso.Nivel)
            .Select(g =>
            {
                var total = g.Sum(m => m.ValorCobrado);
                var qtd = g.Count();
                var media = qtd > 0 ? Math.Round(total / qtd, 2, MidpointRounding.AwayFromZero) : 0m;
                return new ResumoNivelReceita(g.Key, qtd, total, media);
            })
            .OrderByDescending(r => r.ReceitaTotal)
            .ToList();
    }

    public static IReadOnlyList<CursoPopularidade> TopCursos(IEnumerable<Matricula> matriculas, int top = 3)
    {
        ArgumentNullException.ThrowIfNull(matriculas);

        return matriculas
            .GroupBy(m => m.Curso.Titulo)
            .Select(g => new CursoPopularidade(
                TituloCurso: g.Key,
                TotalMatriculas: g.Count(),
                TotalArrecadado: g.Sum(m => m.ValorCobrado)
            ))
            .OrderByDescending(c => c.TotalMatriculas)
            .ThenByDescending(c => c.TotalArrecadado)
            .Take(top)
            .ToList();
    }

    public static decimal CalcularIdadeMediaAlunos(IEnumerable<Matricula> matriculas, DateOnly dataReferencia)
    {
        ArgumentNullException.ThrowIfNull(matriculas);

        var idades = matriculas
            .Select(m => m.Aluno.CalcularIdade(dataReferencia))
            .ToList();

        if (idades.Count == 0) return 0m;

        var media = idades.Average();
        return Math.Round((decimal)media, 1, MidpointRounding.AwayFromZero);
    }

    public static string FormatarListaCursos(IEnumerable<Curso> cursos)
    {
        ArgumentNullException.ThrowIfNull(cursos);

        var titulos = cursos.Select(c => c.Titulo).ToList();
        if (titulos.Count == 0) return string.Empty;

        return titulos.Aggregate((acumulado, proximo) => $"{acumulado} | {proximo}");
    }

    public static IReadOnlyList<Matricula> ObterPagina(
        IEnumerable<Matricula> matriculas,
        int pagina,
        int tamanhoPagina,
        Func<Matricula, bool>? filtro = null)
    {
        ArgumentNullException.ThrowIfNull(matriculas);
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina), "Página deve ser >= 1.");
        if (tamanhoPagina < 1) throw new ArgumentOutOfRangeException(nameof(tamanhoPagina), "Tamanho deve ser >= 1.");

        var consulta = matriculas;
        if (filtro is not null)
            consulta = consulta.Where(filtro);

        return consulta
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToList();
    }

    public static IEnumerable<IReadOnlyList<T>> ParticionarSobDemanda<T>(IEnumerable<T> fonte, int tamanhoLote)
    {
        ArgumentNullException.ThrowIfNull(fonte);
        if (tamanhoLote <= 0) throw new ArgumentOutOfRangeException(nameof(tamanhoLote));

        var lote = new List<T>(tamanhoLote);
        foreach (var item in fonte)
        {
            lote.Add(item);
            if (lote.Count == tamanhoLote)
            {
                yield return lote.AsReadOnly();
                lote = new List<T>(tamanhoLote);
            }
        }

        if (lote.Count > 0)
            yield return lote.AsReadOnly();
    }
}
