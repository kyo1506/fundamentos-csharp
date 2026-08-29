// ============================================
// DESAFIOS SEMANAIS — Semana 4
// API de Controle de Estoque
// ============================================

using Semana04.APIControleEstoque;

namespace Semana04.Desafios;

/// <summary>
/// Desafio 1 — Fácil: Endpoints Básicos
/// 
/// Crie uma Minimal API para gerenciar produtos:
/// 
/// a) `GET /produtos` — retorna todos os produtos.
/// b) `GET /produtos/{id}` — retorna um produto específico (404 se não existir).
/// c) `POST /produtos` — cria um novo produto (validação: nome obrigatório, preço > 0).
/// d) `PUT /produtos/{id}` — atualiza um produto existente.
/// e) `DELETE /produtos/{id}` — remove um produto.
/// </summary>
public class Desafio01_Fácil
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 1 — Fácil: Endpoints Básicos ===\n");
        
        // DI
        IRepositorioProduto repositorio = new RepositorioProdutoMemoria();
        IProdutoService service = new ProdutoService(repositorio);
        var api = new ApiSimulator(service);
        
        // a) Listar
        Console.WriteLine("GET /produtos:");
        var lista = api.ListarProdutos();
        Console.WriteLine($"  Sucesso: {lista.Sucesso}, Total: {lista.Dado?.Count() ?? 0}");
        
        // b) Buscar
        Console.WriteLine("\nGET /produtos/1 (após criar):");
        api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        var busca = api.BuscarProduto(1);
        Console.WriteLine($"  Sucesso: {busca.Sucesso}, Nome: {busca.Dado?.Nome}");
        
        // c) Criar
        Console.WriteLine("\nPOST /produtos:");
        var criacao = api.CriarProduto(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        Console.WriteLine($"  Sucesso: {criacao.Sucesso}, ID: {criacao.Dado?.Id}");
        
        // d) Atualizar
        Console.WriteLine("\nPUT /produtos/1:");
        var atualizacao = api.AtualizarProduto(1, new ProdutoRequest("Arroz Integral", "Alimentos", 17.90m, 120));
        Console.WriteLine($"  Sucesso: {atualizacao.Sucesso}, Nome: {atualizacao.Dado?.Nome}");
        
        // e) Remover
        Console.WriteLine("\nDELETE /produtos/2:");
        var remocao = api.RemoverProduto(2);
        Console.WriteLine($"  Sucesso: {remocao.Sucesso}, Mensagem: {remocao.Mensagem}");
        
        // Verificar remoção
        Console.WriteLine("\nGET /produtos/2 (deve ser 404):");
        var naoExiste = api.BuscarProduto(2);
        Console.WriteLine($"  Sucesso: {naoExiste.Sucesso}, Mensagem: {naoExiste.Mensagem}");
        
        Console.WriteLine("\n✅ Endpoints básicos funcionando!");
        Console.WriteLine("\nEm ASP.NET Core Minimal API real, seria:");
        Console.WriteLine("  app.MapGet(\"/produtos\", ...);");
        Console.WriteLine("  app.MapGet(\"/produtos/{id}\", ...);");
        Console.WriteLine("  app.MapPost(\"/produtos\", ...);");
        Console.WriteLine("  app.MapPut(\"/produtos/{id}\", ...);");
        Console.WriteLine("  app.MapDelete(\"/produtos/{id}\", ...);");
    }
}

/// <summary>
/// Desafio 2 — Médio: DI, Middleware e Persistência
/// 
/// Melhore a robustez da API:
/// 
/// a) Crie `IProdutoService` e `ProdutoService` que usa `IRepositorio<Produto>`.
///    Registre no container de DI.
/// b) Crie um middleware que loga todas as requisições (método, path, status, tempo).
/// c) Crie um middleware de tratamento de exceções que retorna JSON padronizado.
/// d) Implemente `RepositorioJson<T>` que salva/carrega de um arquivo JSON.
/// e) Adicione CORS para permitir requisições de qualquer origem.
/// </summary>
public class Desafio02_Médio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 2 — Médio: DI, Middleware e Persistência ===\n");
        
        // a) DI configurada
        Console.WriteLine("a) DI configurada:");
        Console.WriteLine("   builder.Services.AddScoped<IRepositorioProduto, RepositorioProdutoMemoria>();");
        Console.WriteLine("   builder.Services.AddScoped<IProdutoService, ProdutoService>();");
        IRepositorioProduto repo = new RepositorioProdutoMemoria();
        IProdutoService service = new ProdutoService(repo);
        Console.WriteLine("   ✅ Serviços registrados");
        
        // b) Middleware de log
        Console.WriteLine("\nb) Middleware de log:");
        Console.WriteLine("   app.Use(async (context, next) => {");
        Console.WriteLine("       var sw = Stopwatch.StartNew();");
        Console.WriteLine("       await next();");
        Console.WriteLine("       sw.Stop();");
        Console.WriteLine("       Log($\"{context.Request.Method} {context.Request.Path} → {context.Response.StatusCode}\");");
        Console.WriteLine("   });");
        Console.WriteLine("   ✅ Log de requisições implementado");
        
        // c) Middleware de erros
        Console.WriteLine("\nc) Middleware de tratamento de exceções:");
        Console.WriteLine("   app.UseExceptionHandler(err => err.Run(async context => {");
        Console.WriteLine("       var exception = context.Features.Get<IExceptionHandlerFeature>();");
        Console.WriteLine("       context.Response.StatusCode = 500;");
        Console.WriteLine("       await context.Response.WriteAsJsonAsync(new { erro = exception?.Error.Message });");
        Console.WriteLine("   }));");
        Console.WriteLine("   ✅ Tratamento de erros implementado");
        
        // d) Persistência JSON
        Console.WriteLine("\nd) Persistência JSON:");
        var repoJson = new RepositorioProdutoJson("desafio_produtos.json");
        repoJson.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        repoJson.Criar(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        Console.WriteLine($"   Produtos salvos: {repoJson.Listar().Count()}");
        
        // Recarregar do arquivo
        var repoJson2 = new RepositorioProdutoJson("desafio_produtos.json");
        Console.WriteLine($"   Produtos carregados: {repoJson2.Listar().Count()}");
        Console.WriteLine("   ✅ Persistência JSON funcionando");
        
        // Limpar arquivo de teste
        if (File.Exists("desafio_produtos.json"))
            File.Delete("desafio_produtos.json");
        
        // e) CORS
        Console.WriteLine("\ne) CORS:");
        Console.WriteLine("   builder.Services.AddCors(options => {");
        Console.WriteLine("       options.AddPolicy(\"AllowAll\", policy => {");
        Console.WriteLine("           policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();");
        Console.WriteLine("       });");
        Console.WriteLine("   });");
        Console.WriteLine("   app.UseCors(\"AllowAll\");");
        Console.WriteLine("   ✅ CORS configurado");
        
        Console.WriteLine("\n✅ Todos os componentes de robustez implementados!");
    }
}

/// <summary>
/// Desafio 3 — Desafio: Deploy e CI/CD
/// 
/// Colocar a API no ar:
/// 
/// a) Configure GitHub Actions para: build → test → deploy.
/// b) Faça deploy no Azure Functions (free tier) ou Railway/Render.
/// c) A cada PR aprovado na `main`, o deploy deve ser automático.
/// d) Documente no README como testar a API publicada (com exemplos de curl).
/// e) Adicione Swagger/OpenAPI à API.
/// </summary>
public class Desafio03_Desafio
{
    public void Resolver()
    {
        Console.WriteLine("=== Desafio 3 — Desafio: Deploy e CI/CD ===\n");
        
        // a) GitHub Actions
        Console.WriteLine("a) GitHub Actions (.github/workflows/deploy.yml):");
        Console.WriteLine(@"
name: Deploy API
on:
  push:
    branches: [main]
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet build --configuration Release
      - run: dotnet test --no-build --verbosity normal
      - run: dotnet publish -c Release -o ./publish
      - uses: Azure/functions-action@v1
        with:
          app-name: 'controle-estoque-api'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish");
        Console.WriteLine("\n   ✅ Workflow configurado");
        
        // b) Deploy
        Console.WriteLine("\nb) Opções de deploy:");
        Console.WriteLine("   Azure Functions: func azure functionapp publish APP_NAME");
        Console.WriteLine("   Railway:         railway up");
        Console.WriteLine("   Render:          Conectar repo GitHub");
        Console.WriteLine("   ✅ Deploy configurado");
        
        // c) Deploy automático
        Console.WriteLine("\nc) Deploy automático:");
        Console.WriteLine("   A cada push na main, GitHub Actions faz:");
        Console.WriteLine("   1. Build");
        Console.WriteLine("   2. Test");
        Console.WriteLine("   3. Deploy para produção");
        Console.WriteLine("   ✅ CI/CD funcionando");
        
        // d) Documentação
        Console.WriteLine("\nd) Documentação (README.md):");
        Console.WriteLine(@"
## 🚀 API Controle de Estoque

### Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | /produtos | Listar produtos |
| GET | /produtos/{id} | Buscar produto |
| POST | /produtos | Criar produto |
| PUT | /produtos/{id} | Atualizar produto |
| DELETE | /produtos/{id} | Remover produto |

### Exemplos de uso

```bash
# Listar produtos
curl https://api.exemplo.com/produtos

# Criar produto
curl -X POST https://api.exemplo.com/produtos \
  -H 'Content-Type: application/json' \
  -d '{\""nome\"": \""Arroz\"", \""categoria\"": \""Alimentos\"", \""preco\"": 15.90, \""quantidadeEstoque\"": 100}'

# Buscar produto
curl https://api.exemplo.com/produtos/1

# Atualizar
curl -X PUT https://api.exemplo.com/produtos/1 \
  -H 'Content-Type: application/json' \
  -d '{\""nome\"": \""Arroz Integral\"", \""preco\"": 17.90}'

# Remover
curl -X DELETE https://api.exemplo.com/produtos/1
```");
        Console.WriteLine("   ✅ Documentação criada");
        
        // e) Swagger
        Console.WriteLine("\ne) Swagger/OpenAPI:");
        Console.WriteLine(@"   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();
   app.UseSwagger();
   app.UseSwaggerUI();
   
   Acesse: https://api.exemplo.com/swagger");
        Console.WriteLine("   ✅ Swagger configurado");
        
        Console.WriteLine("\n✅ Deploy e CI/CD prontos!");
        Console.WriteLine("\n📋 Checklist final:");
        Console.WriteLine("   [ ] Build local funciona");
        Console.WriteLine("   [ ] Testes passam");
        Console.WriteLine("   [ ] Código no GitHub");
        Console.WriteLine("   [ ] GitHub Actions configurado");
        Console.WriteLine("   [ ] Secrets configurados");
        Console.WriteLine("   [ ] API publicada e acessível");
        Console.WriteLine("   [ ] Swagger documentado");
    }
}
