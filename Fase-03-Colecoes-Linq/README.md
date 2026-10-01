# Fase 03: Coleções Avançadas, Delegados, Eventos, Generics e LINQ

> Domine tipos genéricos e restrições, ponteiros de função com type-safety (`Func`, `Action`, lambdas e closures),
> arquitetura desacoplada com eventos e `EventHandler`, consultas e agregações ricas com LINQ fluente,
> além de iteradores sob demanda com `yield return` e coleções especializadas como `PriorityQueue`.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/03.1-generics-classes-metodos-restricoes.md`
2. `teoria/03.2-delegates-func-action-lambdas-closures.md`
3. `teoria/03.3-eventos-eventhandler-desacoplamento.md`
4. `teoria/03.4-linq-fundamentos-execucao-adiada.md`
5. `teoria/03.5-linq-avancado-agrupamentos-agregacoes-joins.md`
6. `teoria/03.6-colecoes-especializadas-yield-ienumerable.md`

## Estrutura da Fase
```
Fase-03-Colecoes-Linq/
├── teoria/            # 6 lições em Markdown cobrindo de Generics a LINQ analítico
├── aplicacao/         # Projeto console CourseAnalytics (motor analítico com LINQ, generics e eventos)
├── aplicacao.Tests/   # Suíte de testes unitários xUnit
├── praticas/          # Códigos experimentais
└── exercicios/        # Desafios práticos da fase
```

## Como rodar e testar
```bash
# Executar a aplicação demonstrativa
dotnet run --project aplicacao/CourseAnalytics.csproj

# Executar a suíte de testes unitários
dotnet test aplicacao.Tests/CourseAnalytics.Tests.csproj
```

## O que você domina ao terminar
- Projetar classes e repositórios genéricos com restrições `where`.
- Utilizar delegates de alta ordem (`Func`, `Action`, `Predicate`) e closures.
- Disparar e assinar eventos desacoplados seguindo a convenção `EventHandler<TEventArgs>`.
- Escrever consultas declarativas com LINQ fluente (`Where`, `Select`, `OrderBy`, `GroupBy`, `Aggregate`).
- Evitar armadilhas de performance como múltipla enumeração em `IEnumerable<T>`.
- Produzir sequências sob demanda sem sobrecarga de memória com `yield return`.
