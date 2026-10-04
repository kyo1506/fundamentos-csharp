# Exercícios e Desafios: Fase 05

Esta pasta contém exercícios para consolidar o conhecimento sobre Clean Architecture, Design Patterns, Injeção de Dependência e TDD com xUnit.

---

## Desafio 1: Injeção de Dependência Manual e com ServiceCollection
Crie uma interface `IServicoTaxa` e duas implementações: `TaxaPadrao` e `TaxaIsenta`. Configure uma coleção `ServiceCollection` e resolva a implementação com escopo `Transient`.

## Desafio 2: Padrão Strategy para Regras de Cancelamento
Implemente a interface `IPoliticaCancelamento` com duas estratégias: `CancelamentoSemMulta` (reembolso integral de 100%) e `CancelamentoComMulta` (retenção de taxa de 20%).

## Desafio 3: Padrão Decorator para Medição de Latência
Implemente um decorador `TimingMatriculaServiceDecorator` sobre `IMatriculaService` que mede o tempo de execução do método `MatricularAsync` utilizando `Stopwatch` ou `TimeProvider` sem alterar a classe concreta.

## Desafio 4: Repositório Fake em Memória
Construa uma classe `InMemoryCursoRepository` que implementa `ICursoRepository` utilizando um `Dictionary<int, Curso>` para fins de testes unitários.

## Desafio 5: TDD com Theory e InlineData
Escreva testes unitários parametrizados com `[Theory]` e `[InlineData]` para validar cálculos de descontos progressivos por quantidade de matérias contratadas.

## Desafio 6: Validação de Dependência Cativa
Configure um `ServiceProviderOptions` com `ValidateScopes = true` e escreva um teste que verifica que um serviço `Scoped` não pode ser resolvido a partir do provedor raiz sem escopo explícito.
