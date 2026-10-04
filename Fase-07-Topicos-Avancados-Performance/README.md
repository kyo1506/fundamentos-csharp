# Fase 07: Tópicos Avançados, Alta Performance e Benchmarking

> Domine técnicas avançadas de engenharia de software e otimização extrema no .NET:
> manipulação de memória contígua sem alocações no heap com Span e Memory,
> pipelines assíncronos de alta vazão com System.Threading.Channels,
> processamento ultrarrápido de texto com Source Generators de Regex e SearchValues,
> metaprogramação moderna com geradores de código em tempo de compilação,
> e medição científica de CPU e alocações de memória com BenchmarkDotNet e Profiling.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/07.1-engenharia-performance-span-memory-ref-struct.md`
2. `teoria/07.2-pipelines-assincronos-channels.md`
3. `teoria/07.3-processamento-texto-regex-source-generator-searchvalues.md`
4. `teoria/07.4-metaprogramacao-source-generators-modernos.md`
5. `teoria/07.5-benchmarking-cientifico-benchmarkdotnet-profiling.md`

## Estrutura da Fase
```
Fase-07-Topicos-Avancados-Performance/
├── teoria/            # 5 lições em Markdown sobre Span, Channels, Regex, Source Generators e BenchmarkDotNet
├── aplicacao/         # Motor de processamento AltaPerformance.Core com Channels e Spans
├── aplicacao.Tests/   # Suíte de testes unitários xUnit com testes de concorrência e throughput
├── praticas/          # Códigos experimentais e benchmarks
└── exercicios/        # Desafios práticos e enunciados da fase
```

## Como rodar e testar
```bash
# Executar a biblioteca e pipeline
dotnet build aplicacao/AltaPerformance.Core.csproj -v q -clp:ErrorsOnly

# Executar a suíte de testes xUnit
dotnet test aplicacao.Tests/AltaPerformance.Tests.csproj -v q
```

## O que você domina ao terminar
- Manipular fatias de memória com `Span<T>` e `ReadOnlySpan<T>` sem alocações no Heap.
- Implementar padrões concorrentes Produtor/Consumidor com `System.Threading.Channels` e controle de backpressure.
- Otimizar buscas e validações em textos com `[GeneratedRegex]` e `SearchValues<T>`.
- Compreender o pipeline de compilação com Source Generators e benefícios de Native AOT.
- Projetar benchmarks científicos com `BenchmarkDotNet` diagnosticando tempo de CPU e consumo de memória com `[MemoryDiagnoser]`.
