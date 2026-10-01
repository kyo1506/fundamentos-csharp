using Fase02.PooSolid;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==================================================");
Console.WriteLine(" Fase 02: POO + SOLID (Sistema de Matrículas)    ");
Console.WriteLine("==================================================");

// 1. Composition Root: montagem das dependências (DIP)
var matriculaRepo = new MatriculaRepositoryEmMemoria();
var cursoRepo = new CursoRepositoryEmMemoria();
var notificador = new NotificadorConsole();
var matriculaService = new MatriculaService(matriculaRepo, cursoRepo, notificador);
var exportador = new ExportadorMatriculaCsv();

// 2. Cadastro inicial de cursos
var cursoCsharp = new Curso(Guid.NewGuid(), "C# Moderno e Orientação a Objetos", 40, 600.00m, NivelCurso.Intermediario);
var cursoArquitetura = new Curso(Guid.NewGuid(), "Arquitetura .NET e SOLID", 60, 950.00m, NivelCurso.Avancado);
cursoRepo.Salvar(cursoCsharp);
cursoRepo.Salvar(cursoArquitetura);

Console.WriteLine($"\nCursos disponíveis:");
foreach (var c in cursoRepo.ListarTodos())
{
    Console.WriteLine($" - {c.Titulo} ({c.CargaHoraria}h, Nível {c.Nivel}): R$ {c.PrecoBase:N2}");
}

// 3. Alunos e Polimorfismo / Herança
var aluno1 = new Aluno("Mariana Silva", "123.456.789-00", "mariana@dev.com", new DateOnly(1998, 4, 15));
var aluno2 = new Aluno("Rodrigo Souza", "987.654.321-11", "rodrigo@dev.com", new DateOnly(2001, 8, 22));

Console.WriteLine($"\nAlunos cadastrados:");
Console.WriteLine($" - {aluno1.ObterDescricao()} (Idade: {aluno1.CalcularIdade(DateOnly.FromDateTime(DateTime.UtcNow))} anos)");
Console.WriteLine($" - {aluno2.ObterDescricao()} (Idade: {aluno2.CalcularIdade(DateOnly.FromDateTime(DateTime.UtcNow))} anos)");

// 4. Executando matrículas com diferentes policies de desconto (OCP)
Console.WriteLine($"\nProcessando matrículas:");
var mat1 = matriculaService.Matricular(aluno1, cursoCsharp.Id, new DescontoEstudantePolicy());
var mat2 = matriculaService.Matricular(aluno2, cursoArquitetura.Id, new DescontoVipPolicy());

// 5. Exportação e relatório (ISP)
Console.WriteLine($"\nExportação CSV das Matrículas:");
Console.WriteLine(exportador.Formatar(mat1));
Console.WriteLine(exportador.Formatar(mat2));

Console.WriteLine("\nDemonstração concluída com sucesso!");
