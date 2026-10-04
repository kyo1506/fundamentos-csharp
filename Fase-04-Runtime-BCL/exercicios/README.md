# Exercícios e Desafios: Fase 04

Esta pasta contém exercícios para consolidar o conhecimento sobre .NET Runtime, IO de Streams, Serialização JSON, Programação Assíncrona e APIs da BCL.

---

## Desafio 1: Processador de Arquivo com IAsyncDisposable
Implemente uma classe `BufferTemporario` que herda de `IAsyncDisposable`. A classe deve gravar bytes em um `MemoryStream` ou arquivo temporário e garantir o flush e fechamento assíncrono ao ser descartada.

## Desafio 2: Serializador JSON Tipado com Source Generator
Crie um contexto de serialização customizado com `[JsonSerializable]` e serialize uma lista de registros acadêmicos sem utilizar reflexão em runtime.

## Desafio 3: Pipeline de Cópia Concorrente com SemaphoreSlim
Crie um utilitário que recebe uma lista de streams de origem e grava o conteúdo em streams de destino utilizando `Task.WhenAll`, limitando a concorrência a no máximo 3 operações simultâneas.

## Desafio 4: Cancelamento Limpo com CancellationToken
Implemente uma rotina de processamento assíncrono em lotes que verifica periodicamente um `CancellationToken` e, ao ser cancelada, restaura o estado inicial sem deixar arquivos intermediários corrompidos.

## Desafio 5: Validador de Metadados com Reflection e Atributos
Crie um atributo customizado `[Obrigatorio]` e uma função utilitária `ValidarEntidade<T>(T entidade)` que inspeciona as propriedades decoradas e lança `ValidationException` se alguma string estiver vazia ou nula.

## Desafio 6: Teste Temporal com FakeTimeProvider
Crie uma regra de expiração de acesso para matrículas e escreva testes unitários utilizando um `TimeProvider` mockado para validar a transição de status de "Ativa" para "Expirada".
