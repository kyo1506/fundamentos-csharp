# Exercícios e Desafios: Fase 07

Esta pasta contém exercícios para consolidar o conhecimento sobre tópicos avançados de C# e otimização extrema de performance no .NET runtime.

---

## Desafio 1: Parsing de Log Zero-Allocation com Span
Implemente um método que recebe uma linha de log delimitada por barras verticais e extrai os campos de nível de log e código de status utilizando `ReadOnlySpan<char>` sem criar instâncias de substrings no Heap.

## Desafio 2: Pipeline Concorrente com Channel Bounded
Construa um pipeline assíncrono produtor/consumidor utilizando `Channel.CreateBounded<T>` com capacidade limitada a 10 itens e estratégia de espera para aplicar controle de backpressure.

## Desafio 3: Validação de Chave com GeneratedRegex
Implemente um validador de formato de chave de transação no formato `TRX-XXXX-9999` utilizando o atributo `[GeneratedRegex]` do C# moderno para evitar compilação em tempo de execução.

## Desafio 4: Busca Eficiente de Caracteres Especiais com SearchValues
Utilize a API `SearchValues<char>` do .NET 8+ para localizar a primeira ocorrência de caracteres de pontuação proibidos em uma cadeia de caracteres com instruções vetorizadas SIMD.

## Desafio 5: Estrutura Ref Struct de Leitura Segura
Crie um leitor binário ou de texto baseado em `ref struct` que encapsula um `ReadOnlySpan<char>` e avança incrementalmente pelos tokens sem permitir escape da memória para o Heap.

## Desafio 6: Microbenchmark com MemoryDiagnoser
Configure uma classe de teste de benchmark com `BenchmarkDotNet` decorada com `[MemoryDiagnoser]` comparando a concatenação tradicional de strings com a abordagem otimizada utilizando `ValueStringBuilder` ou `Span`.
