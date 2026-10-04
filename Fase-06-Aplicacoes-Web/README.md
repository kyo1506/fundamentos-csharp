# Fase 06: Aplicações Web e Serviços (.NET Minimal APIs e EF Core)

> Construa APIs web e serviços HTTP modernos no .NET: fundamentos do protocolo REST,
> criação de endpoints de alta performance com Minimal APIs e TypedResults, pipeline de
> middlewares com tratamento global de exceções e ProblemDetails, persistência relacional com
> Entity Framework Core, segurança com autenticação JWT e autorização baseada em roles,
> empacotamento em contêineres Docker e monitoramento com Health Checks.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/06.1-fundamentos-http-rest-httpclient.md`
2. `teoria/06.2-minimal-apis-roteamento-validacao-openapi.md`
3. `teoria/06.3-pipeline-middlewares-problemdetails.md`
4. `teoria/06.4-acesso-a-dados-ef-core-dbcontext-migrations.md`
5. `teoria/06.5-seguranca-autenticacao-jwt-autorizacao.md`
6. `teoria/06.6-publicacao-docker-deploy-observabilidade.md`

## Estrutura da Fase
```
Fase-06-Aplicacoes-Web/
├── teoria/            # 6 lições em Markdown cobrindo HTTP, Minimal APIs, EF Core, JWT e Docker
├── aplicacao/         # Web API GestaoAcademica.WebApi com Minimal APIs e EF Core
├── aplicacao.Tests/   # Suíte de testes de integração com WebApplicationFactory
├── praticas/          # Códigos experimentais
└── exercicios/        # Desafios práticos da fase
```

## Como rodar e testar
```bash
# Executar a Web API
dotnet run --project aplicacao/GestaoAcademica.WebApi.csproj

# Executar os testes de integração HTTP
dotnet test aplicacao.Tests/GestaoAcademica.WebApi.Tests.csproj
```

## O que você domina ao terminar
- Projetar rotas RESTful semânticas utilizando verbos HTTP e códigos de status adequados.
- Construir Minimal APIs enxutas e fortemente tipadas com `TypedResults` e `MapGroup`.
- Tratar falhas de forma padronizada via RFC 7807 (`ProblemDetails`) e middlewares.
- Modelar persistência relacional com `DbContext` e consultas assíncronas no EF Core.
- Autenticar e autorizar requisições utilizando tokens JWT (JSON Web Tokens).
- Escrever testes de integração de ponta a ponta utilizando `WebApplicationFactory`.
