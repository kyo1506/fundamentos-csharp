# Fase 05: Arquitetura, Padrões de Projeto, Injeção de Dependência e Testes (TDD)

> Domine os alicerces da engenharia de software no ecossistema .NET: princípios SOLID aplicados à
> Clean Architecture, padrões de projeto consolidados (Strategy, Factory, Repository, Decorator, Observer),
> configuração e ciclos de vida de Injeção de Dependência (`Microsoft.Extensions.DependencyInjection`),
> desenvolvimento orientado a testes com xUnit (`[Fact]`, `[Theory]`), dublês de teste (Fakes e Mocks)
> e práticas modernas de engenharia com métricas de cobertura de código.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/05.1-solid-aplicado-arquitetura-camadas.md`
2. `teoria/05.2-design-patterns-strategy-factory-repository-decorator.md`
3. `teoria/05.3-injecao-dependencia-microsoft-extensions.md`
4. `teoria/05.4-testes-unitarios-xunit-tdd-facts-theories.md`
5. `teoria/05.5-dubles-de-teste-mocks-fakes-e-cobertura.md`

## Estrutura da Fase
```
Fase-05-Arquitetura-Testes/
├── teoria/            # 5 lições em Markdown sobre Clean Architecture, Patterns, DI e TDD
├── aplicacao/         # Núcleo GestaoAcademica.Core com DI, serviços de domínio e padrões
├── aplicacao.Tests/   # Suíte completa de testes xUnit (Facts, Theories, Fakes e DI)
├── praticas/          # Códigos experimentais
└── exercicios/        # Desafios práticos da fase
```

## Como rodar e testar
```bash
# Executar a demonstração de resolução do container de DI
dotnet run --project aplicacao/GestaoAcademica.Core.csproj

# Executar a suíte de testes unitários xUnit com TDD
dotnet test aplicacao.Tests/GestaoAcademica.Tests.csproj
```

## O que você domina ao terminar
- Separar regras de negócio da infraestrutura seguindo os princípios da Clean Architecture.
- Aplicar padrões de projeto (Strategy para políticas, Repository para abstração de dados, Decorator para extensões).
- Configurar contêineres de Injeção de Dependência com os ciclos `Transient`, `Scoped` e `Singleton` evitando dependências cativas.
- Escrever testes automatizados seguindo o ciclo Red-Green-Refactor do TDD.
- Utilizar `[Theory]` com `[InlineData]` para testes de decisão parametrizados.
- Construir e utilizar Fakes em memória para testar casos de uso complexos sem acoplamento externo.
