# Fase 04: .NET Runtime & BCL (IO, JSON, Threads e Async)

> Domine o funcionamento interno do Common Language Runtime (CLR) e as bibliotecas essenciais do .NET:
> gerenciamento de memória geracional e descarte com `IDisposable`/`IAsyncDisposable`, manipulação
> de streams e buffers de alto desempenho, serialização otimizada com `System.Text.Json` e Source Generators,
> padrões modernos de assincronismo com `Task`, `ValueTask`, `CancellationToken`, tipos temporais testáveis
> com `TimeProvider`, introspecção dinâmica com Reflection e ecossistema de configuração e logging.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/04.1-garbage-collector-idisposable-finalizadores.md`
2. `teoria/04.2-manipulacao-arquivos-streams-buffers.md`
3. `teoria/04.3-serializacao-system-text-json-source-generators.md`
4. `teoria/04.4-programacao-assincrona-task-async-await-cancellation.md`
5. `teoria/04.5-globalizacao-tipos-temporais-timeprovider.md`
6. `teoria/04.6-reflection-metadados-atributos-customizados.md`
7. `teoria/04.7-configuracao-logging-microsoft-extensions.md`

## Estrutura da Fase
```
Fase-04-Runtime-BCL/
├── teoria/            # 7 lições em Markdown cobrindo o runtime e APIs essenciais da BCL
├── aplicacao/         # Projeto console RelatoriosAsyncCli (processamento assíncrono de relatórios)
├── aplicacao.Tests/   # Suíte de testes unitários xUnit com streams, JSON e cancelamento
├── praticas/          # Códigos experimentais
└── exercicios/        # Desafios práticos da fase
```

## Como rodar e testar
```bash
# Executar a aplicação demonstrativa
dotnet run --project aplicacao/RelatoriosAsyncCli.csproj

# Executar a suíte de testes unitários
dotnet test aplicacao.Tests/RelatoriosAsyncCli.Tests.csproj
```

## O que você domina ao terminar
- Diferenciar os ciclos de vida de memória nas gerações Gen 0, Gen 1, Gen 2, LOH e POH do Garbage Collector.
- Implementar o padrão `IDisposable` e `IAsyncDisposable` para liberação de recursos não gerenciados.
- Ler e gravar fluxos de dados de forma eficiente com `Stream`, `StreamReader`, `StreamWriter` e reutilização de buffers.
- Utilizar `System.Text.Json` com Source Generators para serialização veloz e zero-reflection.
- Coordenar rotinas assíncronas com `Task.WhenAll`, limitar paralelismo com `SemaphoreSlim` e propagar cancelamento com `CancellationToken`.
- Garantir testes unitários determinísticos de tempo usando a abstração `TimeProvider`.
- Criar e inspecionar atributos customizados com Reflection.
- Estruturar logs e configurações com as abstrações do `Microsoft.Extensions`.
