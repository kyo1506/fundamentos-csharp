using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Fase05.GestaoAcademica.Core;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("====================================================================");
Console.WriteLine("  Fase 05: Arquitetura, Padrões, DI e Testes (GestaoAcademica.Core)");
Console.WriteLine("====================================================================");
Console.WriteLine();

// 1. Configuração do container de Injeção de Dependência
var services = new ServiceCollection();
services.AddGestaoAcademicaCore();

using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
});

Console.WriteLine("1. Container IoC configurado com sucesso e validação de escopos ativada.");

// 2. Criação de escopo para execução das operações
using (IServiceScope scope = provider.CreateScope())
{
    var alunoRepo = scope.ServiceProvider.GetRequiredService<IAlunoRepository>();
    var cursoRepo = scope.ServiceProvider.GetRequiredService<ICursoRepository>();
    var baseService = scope.ServiceProvider.GetRequiredService<IMatriculaService>();

    // Decoramos o serviço para demonstrar o padrão Decorator
    var matriculaService = new LoggingMatriculaServiceDecorator(baseService);

    // Seed de dados iniciais
    var aluno1 = new Aluno(1, "Juliana Prado", "juliana@email.com", possuiConvenioEmpresarial: true);
    var aluno2 = new Aluno(2, "Rodrigo Lima", "rodrigo@email.com", possuiConvenioEmpresarial: false);
    var curso = new Curso(101, "Arquitetura .NET & Microsserviços", precoBase: 1000m, vagasTotais: 2);

    await alunoRepo.AdicionarAsync(aluno1);
    await alunoRepo.AdicionarAsync(aluno2);
    await cursoRepo.AdicionarAsync(curso);

    Console.WriteLine();
    Console.WriteLine("2. Realizando matrículas com estratégia de desconto:");
    var mat1 = await matriculaService.MatricularAlunoAsync(aluno1.Id, curso.Id);
    Console.WriteLine($"   - Aluno: {aluno1.Nome} (Com convênio) | Cobrado: R$ {mat1.ValorCobrado:F2} (25% off)");

    var mat2 = await matriculaService.MatricularAlunoAsync(aluno2.Id, curso.Id);
    Console.WriteLine($"   - Aluno: {aluno2.Nome} (Sem convênio)  | Cobrado: R$ {mat2.ValorCobrado:F2} (Preço cheio)");

    Console.WriteLine();
    Console.WriteLine("3. Testando validação de negócio: vagas esgotadas:");
    var alunoExtra = new Aluno(3, "Camila Silva", "camila@email.com");
    await alunoRepo.AdicionarAsync(alunoExtra);

    try
    {
        await matriculaService.MatricularAlunoAsync(alunoExtra.Id, curso.Id);
    }
    catch (VagasEsgotadasException ex)
    {
        Console.WriteLine($"   [Regra de Negócio]: {ex.Message}");
    }

    Console.WriteLine();
    Console.WriteLine($"4. Telemetria do Decorator: {matriculaService.TotalMatriculasRealizadas} matrículas realizadas.");
}

Console.WriteLine();
Console.WriteLine("Demonstração da Fase 05 concluída com sucesso!");
