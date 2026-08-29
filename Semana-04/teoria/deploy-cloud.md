# Deploy em Nuvem — Colocando a API no Ar

> **Semana 4 — Teoria** | Tempo estimado: 20 minutos

---

## O que é Deploy?

**Deploy** é o processo de publicar sua aplicação em um servidor acessível pela internet. Em vez de rodar apenas localmente (`localhost`), sua API fica disponível para qualquer pessoa acessar.

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPLOY                                       │
├─────────────────────────────────────────────────────────────────┤
│  Desenvolvimento          →     Produção                        │
│  localhost:5000                   api.dominio.com               │
│  Sua máquina                      Servidor na nuvem             │
│  Só você acessa                   Qualquer um acessa            │
└─────────────────────────────────────────────────────────────────┘
```

---

## Opções de Deploy Gratuito (Free Tier)

| Serviço | Tipo | Limite Gratuito | Ideal para |
|---------|------|-----------------|------------|
| **Azure Functions** | Serverless | 1M req/mês | APIs pequenas |
| **Railway** | PaaS | 500h/mês | APIs .NET |
| **Render** | PaaS | 750h/mês | APIs e sites |
| **GitHub Pages** | Static | Ilimitado | Documentação, front-end |
| **Supabase** | DBaaS | 500MB | Banco de dados |
| **Vercel** | Serverless | 100GB bandwidth | Front-end, APIs |

---

## Azure Functions (Recomendado para C#)

**Azure Functions** é a opção mais natural para C# — integração direta com o ecossistema .NET.

### Vantagens:
- Free tier generoso (1 milhão de requisições/mês)
- Escala automática
- Integração com Visual Studio / CLI
- Suporte nativo a HTTP triggers

### Estrutura de uma Function:

```csharp
public class ProdutoFunctions
{
    private readonly IRepositorio<Produto> _repositorio;

    public ProdutoFunctions(IRepositorio<Produto> repositorio)
    {
        _repositorio = repositorio;
    }

    [Function("ListarProdutos")]
    public async Task<HttpResponseData> Listar(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "produtos")] 
        HttpRequestData req)
    {
        var produtos = _repositorio.Listar();
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(produtos);
        return response;
    }

    [Function("CriarProduto")]
    public async Task<HttpResponseData> Criar(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "produtos")] 
        HttpRequestData req)
    {
        var produto = await req.ReadFromJsonAsync<Produto>();
        _repositorio.Adicionar(produto);
        
        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteAsJsonAsync(produto);
        return response;
    }
}
```

---

## CI/CD com GitHub Actions

**CI (Continuous Integration)** = toda vez que código é subido, build e testes rodam automaticamente.

**CD (Continuous Deployment)** = se build e testes passam, deploy é feito automaticamente.

### Fluxo:

```
┌─────────────────────────────────────────────────────────────────┐
│  Push/PR no GitHub                                              │
│      ↓                                                          │
│  GitHub Actions: Build + Test                                   │
│      ↓                                                          │
│  Se passou → Deploy automático                                  │
│      ↓                                                          │
│  API atualizada em produção                                     │
└─────────────────────────────────────────────────────────────────┘
```

### Workflow para Azure Functions:

```yaml
name: Deploy Azure Function

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Build
        run: dotnet build --configuration Release
      
      - name: Test
        run: dotnet test --no-build
      
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      
      - name: Deploy to Azure
        uses: Azure/functions-action@v1
        with:
          app-name: 'minha-api-csharp'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: ./publish
```

---

## Railway / Render (Alternativas mais simples)

### Railway:

1. Conectar repositório GitHub
2. Railway detecta que é .NET
3. Build automático a cada push
4. URL pública gerada automaticamente

### Render:

1. Criar "Web Service"
2. Conectar repositório GitHub
3. Comando de build: `dotnet build`
4. Comando de start: `dotnet run --project MinhaApi.csproj`

---

## Variáveis de Ambiente

Nunca coloque secrets (senhas, connection strings) no código. Use variáveis de ambiente:

```csharp
// Em desenvolvimento: appsettings.json
// Em produção: variáveis de ambiente

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
// ou
var connectionString = builder.Configuration.GetConnectionString("Default");
```

No GitHub Actions, use **Secrets**:
- Settings → Secrets → New repository secret
- Acesse com `${{ secrets.NOME_DO_SECRET }}`

---

## Monitoramento e Logs

Após o deploy, como saber se está funcionando?

| Ferramenta | Uso |
|------------|-----|
| **Application Insights** (Azure) | Monitoramento de performance |
| **Railway Logs** | Ver logs em tempo real |
| **Render Logs** | Ver logs em tempo real |
| **Swagger** | Testar endpoints manualmente |

---

## Checklist de Deploy

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPLOY CHECKLIST                             │
├─────────────────────────────────────────────────────────────────┤
│  [ ] Build local funciona: dotnet build                         │
│  [ ] Testes passam: dotnet test                                 │
│  [ ] Código no GitHub                                           │
│  [ ] GitHub Actions configurado                                 │
│  [ ] Secrets configurados (se necessário)                       │
│  [ ] Variáveis de ambiente configuradas                         │
│  [ ] CORS configurado para a origem correta                     │
│  [ ] Swagger acessível                                          │
│  [ ] Endpoints testados com Postman/curl                        │
│  [ ] Health check implementado                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## Resumo Visual

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPLOY OPTIONS                               │
├─────────────────────────────────────────────────────────────────┤
│  Azure Functions  │  Serverless, free tier, ideal para C#       │
│  Railway          │  PaaS simples, deploy automático            │
│  Render           │  PaaS simples, deploy automático            │
│  GitHub Pages     │  Apenas front-end estático                  │
├─────────────────────────────────────────────────────────────────┤
│  CI/CD: GitHub Actions → Build → Test → Deploy                 │
│  Secrets: Nunca no código, sempre em variáveis de ambiente      │
│  Logs: Acompanhe em produção para detectar problemas            │
└─────────────────────────────────────────────────────────────────┘
```

---

*Anterior: [ASP.NET Core](./aspnet-core.md) | Próximo: [Exemplo Prático — API Minimal](./praticas/ExemploMinimalAPI.cs)*
