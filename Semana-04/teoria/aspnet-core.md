# ASP.NET Core Minimal APIs — Criando APIs RESTful

> **Semana 4 — Teoria** | Tempo estimado: 20 minutos

---

## O que é uma API REST?

**API (Application Programming Interface)** é um contrato que permite sistemas se comunicarem. **REST (Representational State Transfer)** é um estilo arquitetural que usa HTTP de forma semântica.

```
┌─────────────────────────────────────────────────────────────────┐
│                    API REST                                     │
├─────────────────────────────────────────────────────────────────┤
│  Cliente (navegador/app)  ←→  Servidor (API)                   │
│                                                                  │
│  GET    /produtos      →  Listar produtos                       │
│  GET    /produtos/1    →  Buscar produto específico             │
│  POST   /produtos      →  Criar novo produto                    │
│  PUT    /produtos/1    →  Atualizar produto                     │
│  DELETE /produtos/1    →  Remover produto                       │
└─────────────────────────────────────────────────────────────────┘
```

---

## HTTP Methods (Verbos)

| Método | Descrição | Exemplo |
|--------|-----------|---------|
| `GET` | Ler dados (idempotente) | `GET /produtos` |
| `POST` | Criar recurso | `POST /produtos` |
| `PUT` | Atualizar recurso (substituir) | `PUT /produtos/1` |
| `PATCH` | Atualizar parcialmente | `PATCH /produtos/1` |
| `DELETE` | Remover recurso | `DELETE /produtos/1` |

### Status Codes comuns

| Código | Significado |
|--------|-------------|
| `200 OK` | Sucesso (GET, PUT) |
| `201 Created` | Criado com sucesso (POST) |
| `204 No Content` | Sucesso sem corpo (DELETE) |
| `400 Bad Request` | Dados inválidos |
| `404 Not Found` | Recurso não encontrado |
| `500 Internal Server Error` | Erro no servidor |

---

## ASP.NET Core Minimal APIs

**Minimal APIs** são a forma mais simples de criar APIs no .NET — sem controllers, sem classes extras. Tudo em um arquivo.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/produtos", () => "Lista de produtos");
app.MapGet("/produtos/{id}", (int id) => $"Produto {id}");
app.MapPost("/produtos", () => "Produto criado");

app.Run();
```

---

## Roteamento (Routing)

```csharp
// Rota fixa
app.MapGet("/hello", () => "Hello World!");

// Rota com parâmetro
app.MapGet("/produtos/{id}", (int id) => $"Produto {id}");

// Rota com múltiplos parâmetros
app.MapGet("/produtos/{id}/comentarios/{comentarioId}", 
    (int id, int comentarioId) => $"Comentário {comentarioId} do produto {id}");

// Restante da URL
app.MapGet("/arquivos/{*caminho}", (string caminho) => $"Arquivo: {caminho}");
```

---

## Injeção de Dependência (DI)

ASP.NET Core tem DI nativo. Você registra serviços no container e eles são injetados automaticamente.

```csharp
// Registro
builder.Services.AddSingleton<IRepositorio, RepositorioMemoria>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Uso em Minimal API
app.MapGet("/produtos", (IRepositorio repo) => 
{
    return repo.Listar();
});
```

### Ciclos de vida

| Ciclo | Descrição | Uso típico |
|-------|-----------|------------|
| `AddTransient` | Nova instância a cada uso | Serviços leves, stateless |
| `AddScoped` | Uma instância por requisição | Repositórios, DbContext |
| `AddSingleton` | Uma instância única | Cache, configuração |

---

## Middleware

Middleware são componentes que formam o pipeline de requisição. Cada um pode processar a requisição e passá-la adiante.

```csharp
// Pipeline padrão
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Middleware customizado
app.Use(async (context, next) =>
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    Console.WriteLine($"[{context.Request.Method}] {context.Request.Path}");
    
    await next(); // Próximo middleware
    
    sw.Stop();
    Console.WriteLine($"  → {context.Response.StatusCode} ({sw.ElapsedMilliseconds}ms)");
});
```

---

## Validação de Modelo

```csharp
// Modelo com validação
public record ProdutoRequest(
    [Required] string Nome,
    [Range(0.01, double.MaxValue)] decimal Preco,
    [Range(0, int.MaxValue)] int Quantidade
);

// Em Minimal API
app.MapPost("/produtos", (ProdutoRequest request) =>
{
    // Se inválido, retorna 400 automaticamente
    return Results.Created($"/produtos/1", new { Id = 1, request.Nome });
});
```

---

## CORS (Cross-Origin Resource Sharing)

Permite que a API seja acessada de origens diferentes (ex: frontend em outro domínio).

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
```

---

## Swagger / OpenAPI

Documentação automática da API.

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

app.UseSwagger();
app.UseSwaggerUI();
```

Acesse `/swagger` para ver a documentação interativa.

---

## Resumo Visual

```
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Minimal API                     │
├─────────────────────────────────────────────────────────────────┤
│  1. Criar app: WebApplication.CreateBuilder()                   │
│  2. Registrar serviços: builder.Services.Add...()              │
│  3. Mapear endpoints: app.MapGet/MapPost/MapPut/MapDelete()    │
│  4. Middleware: app.Use...()                                    │
│  5. Executar: app.Run()                                         │
├─────────────────────────────────────────────────────────────────┤
│  DI: Injeção de Dependência nativa                              │
│  CORS: Controle de origem cruzada                               │
│  Swagger: Documentação automática                               │
│  Middleware: Pipeline de requisição                             │
└─────────────────────────────────────────────────────────────────┘
```

---

*Anterior: [LINQ](./linq.md) | Próximo: [Deploy em Nuvem](./deploy-cloud.md)*
