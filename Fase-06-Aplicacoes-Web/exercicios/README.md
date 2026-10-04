# Exercícios e Desafios: Fase 06

Esta pasta contém exercícios para consolidar o conhecimento sobre desenvolvimento Web com ASP.NET Core Minimal APIs, EF Core e segurança JWT.

---

## Desafio 1: Endpoint de Consulta com TypedResults
Crie um endpoint `GET /api/v1/cursos/{id}` que retorna `TypedResults.Ok(curso)` se o curso for encontrado e `TypedResults.NotFound()` caso contrário.

## Desafio 2: Validação de Payload com Endpoint Filter
Implemente um `IEndpointFilter` para o endpoint `POST /api/v1/cursos` que valida se o título possui no mínimo 3 caracteres e se o preço é positivo.

## Desafio 3: Middleware de Medição de Tempo de Resposta
Construa um middleware customizado que calcula o tempo decorrido para processar a requisição e anexa o cabeçalho `X-Response-Time-Ms` na resposta.

## Desafio 4: Consulta Paginada com Entity Framework Core
Implemente uma consulta com LINQ no EF Core utilizando `.Skip()` e `.Take()` com `.AsNoTracking()` para paginação de matrículas cadastradas.

## Desafio 5: Geração de Token JWT com Claims
Crie um serviço de autenticação que valida credenciais de usuário e gera um token JWT contendo as claims de `Email` e `Role`.

## Desafio 6: Teste de Integração com WebApplicationFactory
Escreva um teste de integração utilizando `WebApplicationFactory<Program>` que faz uma requisição HTTP real `GET /healthz` e valida o retorno `200 OK`.
