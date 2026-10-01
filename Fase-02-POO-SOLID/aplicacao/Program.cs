using Fase02.PooSolid;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("==================================================");
Console.WriteLine(" Fase 02: POO + SOLID (Sistema Acadêmico)         ");
Console.WriteLine("==================================================");

// 1. Composition Root: montagem das dependências (DIP)
var matriculaRepo = new MatriculaRepositoryEmMemoria();
var cursoRepo = new CursoRepositoryEmMemoria();
var notificador = new NotificadorConsole();
var matriculaService = new MatriculaService(matriculaRepo, cursoRepo, notificador);
var exportador = new ExportadorMatriculaCsv();
var politicaReembolso = new ReembolsoProgressivoPolitica();

// 2. Cadastro de Cursos
var cursoCsharp = new Curso(Guid.NewGuid(), "C# Moderno e Orientação a Objetos", 40, 600.00m, NivelCurso.Intermediario);
var cursoArquitetura = new Curso(Guid.NewGuid(), "Arquitetura .NET e SOLID", 60, 950.00m, NivelCurso.Avancado);
cursoRepo.Salvar(cursoCsharp);
cursoRepo.Salvar(cursoArquitetura);

Console.WriteLine("\n[1] Cursos disponíveis:");
foreach (var c in cursoRepo.ListarTodos())
{
    Console.WriteLine($" - {c.Titulo} ({c.CargaHoraria}h, Nível {c.Nivel}): R$ {c.PrecoBase:N2}");
}

// 3. Alunos com CPF higienizado (Encapsulamento e SRP)
var cpf1 = new Cpf("123.456.789-00");
var cpf2 = new Cpf("98765432111");

var aluno1 = new Aluno("Mariana Silva", cpf1.ValorFormatado, "mariana@dev.com", new DateOnly(1998, 4, 15));
var aluno2 = new Aluno("Rodrigo Souza", cpf2.ValorFormatado, "rodrigo@dev.com", new DateOnly(2001, 8, 22));

Console.WriteLine("\n[2] Alunos cadastrados com CPF validado:");
Console.WriteLine($" - {aluno1.ObterDescricao()} (Idade: {aluno1.CalcularIdade(DateOnly.FromDateTime(DateTime.UtcNow))} anos)");
Console.WriteLine($" - {aluno2.ObterDescricao()} (Idade: {aluno2.CalcularIdade(DateOnly.FromDateTime(DateTime.UtcNow))} anos)");

// 4. Controle de Turma e Vagas (Invariantes de Domínio)
var turmaCsharp = new Turma(Guid.NewGuid(), cursoCsharp, capacidadeMaxima: 2);
Console.WriteLine($"\n[3] Turma aberta para '{cursoCsharp.Titulo}' (Capacidade: {turmaCsharp.CapacidadeMaxima})");

var matriculouA1 = turmaCsharp.MatricularAluno(aluno1);
var matriculouA2 = turmaCsharp.MatricularAluno(aluno2);
Console.WriteLine($" - Matrícula Aluno 1 na turma: {matriculouA1} (Vagas restantes: {turmaCsharp.VagasRestantes})");
Console.WriteLine($" - Matrícula Aluno 2 na turma: {matriculouA2} (Vagas restantes: {turmaCsharp.VagasRestantes})");

// 5. Matrículas com Policies de Desconto (OCP e Strategy)
Console.WriteLine("\n[4] Processando matrículas no serviço:");
var mat1 = matriculaService.Matricular(aluno1, cursoCsharp.Id, new DescontoEstudantePolicy());
var mat2 = matriculaService.Matricular(aluno2, cursoArquitetura.Id, new DescontoVipPolicy());

// 6. Cancelamento e Reembolso Progressivo
Console.WriteLine("\n[5] Simulação de Cancelamento com Reembolso:");
var diasDecorridos = 5;
var valorReembolsado = politicaReembolso.CalcularReembolso(mat1.ValorCobrado, diasDecorridos);
Console.WriteLine($" - Cancelamento com {diasDecorridos} dias decorridos: Reembolso de R$ {valorReembolsado:N2} (de R$ {mat1.ValorCobrado:N2})");

// 7. Emissão Polimórfica de Certificados (LSP e Polimorfismo)
Console.WriteLine("\n[6] Emissão de Certificados:");
var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
CertificadoBase cert1 = new CertificadoConclusao(aluno1, cursoCsharp, hoje, notaFinal: 9.8m);
CertificadoBase cert2 = new CertificadoParticipacao(aluno2, cursoArquitetura, hoje);

Console.WriteLine($" - {cert1.GerarTexto()}");
Console.WriteLine($" - {cert2.GerarTexto()}");

// 8. Exportação CSV (ISP)
Console.WriteLine("\n[7] Exportação CSV:");
Console.WriteLine(exportador.Formatar(mat1));
Console.WriteLine(exportador.Formatar(mat2));

Console.WriteLine("\nDemonstração da Fase 02 concluída com sucesso!");
