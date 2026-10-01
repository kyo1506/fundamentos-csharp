using System.Globalization;
using Fase02.PooSolid;
using Fase03.CourseAnalytics;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==================================================");
Console.WriteLine(" Fase 03: Coleções, Eventos e LINQ Analítico     ");
Console.WriteLine("==================================================");

// 1. Repositório Genérico com Restrições
var cursoRepo = new RepositorioGenerico<Curso, Guid>();
var matriculaRepo = new RepositorioGenerico<Matricula, Guid>();

var c1 = new Curso(Guid.NewGuid(), "C# Fundamentos e Lógica", 30, 300m, NivelCurso.Iniciante);
var c2 = new Curso(Guid.NewGuid(), "POO e Boas Práticas SOLID", 40, 500m, NivelCurso.Intermediario);
var c3 = new Curso(Guid.NewGuid(), "LINQ, Coleções e Performance", 50, 750m, NivelCurso.Avancado);
var c4 = new Curso(Guid.NewGuid(), "Arquitetura .NET Moderna", 60, 950m, NivelCurso.Avancado);

cursoRepo.Adicionar(c1);
cursoRepo.Adicionar(c2);
cursoRepo.Adicionar(c3);
cursoRepo.Adicionar(c4);

Console.WriteLine($"\n[1] Catálogo de Cursos formatado com Aggregate:");
Console.WriteLine(AnalyticsEngine.FormatarListaCursos(cursoRepo.ObterTodos()));

// 2. Hub de Eventos Desacoplado (Publisher/Subscriber)
var eventHub = new AnalyticsEventHub();
eventHub.AoRegistrarMatricula += (sender, e) =>
{
    Console.WriteLine($" [EVENTO] Nova matrícula: Aluno '{e.Matricula.Aluno.Nome}' em '{e.Matricula.Curso.Titulo}' por R$ {e.Matricula.ValorCobrado:N2}");
};
eventHub.AoLotarTurma += (sender, e) =>
{
    Console.WriteLine($" [ALERTA] Turma do curso '{e.NomeCurso}' atingiu capacidade máxima de {e.Capacidade} vagas!");
};

// 3. Cadastrando Alunos e Matrículas
var a1 = new Aluno("Larissa Santos", "111.111.111-11", "larissa@email.com", new DateOnly(1996, 3, 10));
var a2 = new Aluno("Mateus Lima", "222.222.222-22", "mateus@email.com", new DateOnly(2002, 7, 20));
var a3 = new Aluno("Camila Rocha", "333.333.333-33", "camila@email.com", new DateOnly(1990, 11, 5));
var a4 = new Aluno("Thiago Alves", "444.444.444-44", "thiago@email.com", new DateOnly(1999, 1, 15));

Console.WriteLine("\n[2] Registrando Matrículas e disparando eventos:");
var m1 = new Matricula(Guid.NewGuid(), a1, c2, 500m);
var m2 = new Matricula(Guid.NewGuid(), a2, c3, 600m);
var m3 = new Matricula(Guid.NewGuid(), a3, c3, 750m);
var m4 = new Matricula(Guid.NewGuid(), a4, c4, 950m);
var m5 = new Matricula(Guid.NewGuid(), a1, c3, 750m);

foreach (var m in new[] { m1, m2, m3, m4, m5 })
{
    matriculaRepo.Adicionar(m);
    eventHub.PublicarMatricula(m);
}

// 4. Análise com LINQ: Agrupamento por Nível (GroupBy + Sum/Average)
Console.WriteLine("\n[3] Relatório de Receita por Nível (GroupBy + Aggregate):");
var resumoNiveis = AnalyticsEngine.ReceitaPorNivel(matriculaRepo.ObterTodos());
foreach (var r in resumoNiveis)
{
    Console.WriteLine($" - Nível {r.Nivel,-13}: {r.TotalMatriculas} matrículas | Receita: R$ {r.ReceitaTotal,8:N2} | Ticket Médio: R$ {r.TicketMedio,6:N2}");
}

// 5. Análise com LINQ: Top Cursos Mais Procurados
Console.WriteLine("\n[4] Top 2 Cursos Mais Procurados (OrderByDescending + Take):");
var top = AnalyticsEngine.TopCursos(matriculaRepo.ObterTodos(), top: 2);
foreach (var t in top)
{
    Console.WriteLine($" - {t.TituloCurso}: {t.TotalMatriculas} alunos (Total: R$ {t.TotalArrecadado:N2})");
}

// 6. Média de Idade dos Alunos
var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
var mediaIdade = AnalyticsEngine.CalcularIdadeMediaAlunos(matriculaRepo.ObterTodos(), hoje);
Console.WriteLine($"\n[5] Média de idade dos alunos matriculados: {mediaIdade} anos");

// 7. Streaming com yield return (Particionamento sob demanda)
Console.WriteLine("\n[6] Lotes de processamento gerados sob demanda com yield return:");
int loteIndex = 1;
foreach (var lote in AnalyticsEngine.ParticionarSobDemanda(matriculaRepo.ObterTodos(), tamanhoLote: 2))
{
    Console.WriteLine($" - Lote {loteIndex++} ({lote.Count} itens): {string.Join(", ", lote.Select(x => x.Aluno.Nome))}");
}

Console.WriteLine("\nDemonstração da Fase 03 concluída com sucesso!");
