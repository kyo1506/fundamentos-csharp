using Microsoft.Extensions.DependencyInjection;

namespace Fase05.GestaoAcademica.Core;

public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Registra os serviços, repositórios e políticas da Gestão Acadêmica no contêiner IoC.
    /// Demonstra a composição limpa e desacoplada por camada.
    /// </summary>
    public static IServiceCollection AddGestaoAcademicaCore(
        this IServiceCollection services,
        IPoliticaDesconto? politicaDesconto = null,
        TimeProvider? timeProvider = null)
    {
        // 1. Repositórios em memória (Scoped para representar o ciclo de trabalho)
        services.AddScoped<IAlunoRepository, InMemoryAlunoRepository>();
        services.AddScoped<ICursoRepository, InMemoryCursoRepository>();
        services.AddScoped<IMatriculaRepository, InMemoryMatriculaRepository>();

        // 2. Políticas de desconto (Transient - sem estado)
        if (politicaDesconto != null)
        {
            services.AddSingleton(politicaDesconto);
        }
        else
        {
            services.AddTransient<IPoliticaDesconto, DescontoConvenioPolicy>();
        }

        // 3. Provedor temporal (Singleton)
        services.AddSingleton(timeProvider ?? TimeProvider.System);

        // 4. Serviço de Aplicação / Domínio
        services.AddScoped<IMatriculaService, MatriculaService>();

        return services;
    }
}
