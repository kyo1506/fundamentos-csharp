# Fundamentos do C# (.NET) — Curso Guiado do Básico ao Avançado

> Curso de programação em C# e .NET do zero ao avançado, organizado em **Fases**.
> Veja o roadmap completo em [`CURRICULO.md`](CURRICULO.md).

## 📋 Sobre o Projeto

Um curso progressivo e didático para aprender C# e o ecossistema .NET **na prática**.
Cada **Fase** contém **teoria** (Markdown), **exemplos práticos**, um **projeto integrador**
(console) com **testes xUnit** e **exercícios/desafios** entregues por Pull Request
para code review.

| Fase | Conteúdo | Entregável |
|------|----------|------------|
| **00** | Fundamentos da Programação (lógica com C#) | `LogicaEmAcao` (console) |
| **01** | C# Básico em profundidade (tipos/memória/coleções/strings) | console |
| **02** | Programação Orientada a Objetos + SOLID | console |
| **03** | Coleções avançadas, Delegados, Eventos, Generics, LINQ | console |
| **04** | .NET Runtime & BCL (IO, JSON, async, threads) | CLI |
| **05** | Arquitetura, Padrões, DI e Testes | lib + testes |
| **06** | Aplicações Web e Serviços | API REST |
| **07** | Tópicos Avançados & Performance | lib + benchmarks |

## 🚀 Como Usar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Editor: [VS Code](https://code.visualstudio.com/), [Rider](https://www.jetbrains.com/rider/) ou Visual Studio
- [Git](https://git-scm.com/)

### Estrutura do Repositório
```
fundamentos-csharp/
├── .github/
│   ├── pull_request_template.md   # Template de PR para code review
│   └── workflows/ci.yml           # CI: build + testes
├── Fase-00-Fundamentos-da-Programacao/
│   ├── teoria/                    # Aulas em Markdown
│   ├── aplicacao/                 # Projeto console (LogicaEmAcao)
│   ├── aplicacao.Tests/           # Testes xUnit
│   └── README.md                  # Guia da fase
├── Fase-01-.../                   # (próximas fases seguem o mesmo padrão)
├── CURRICULO.md                   # Roadmap completo
├── FundamentosCSharp.slnx         # Solução .NET
└── docs/
```

### Executar Localmente
```bash
git clone https://github.com/kyo1506/fundamentos-csharp.git
cd fundamentos-csharp

# Restaurar, compilar e testar a solução
dotnet restore FundamentosCSharp.slnx
dotnet build FundamentosCSharp.slnx
dotnet test FundamentosCSharp.slnx

# Executar o projeto da fase desejada
dotnet run --project Fase-00-Fundamentos-da-Programacao/aplicacao/LogicaEmAcao.csproj
```

## 🎯 Metodologia (TEAP)

Cada sessão de estudo segue 4 etapas:
1. **T — Teoria Sucinta:** conceito explicado de forma direta, com diagramas
2. **E — Exemplo Contextualizado:** código real e executável
3. **A — Aplicação em Projeto:** exemplo aplicado no projeto integrador da fase
4. **P — Problema para Resolver:** desafio entregue via PR para code review

## 🔁 CI/CD
O repositório usa **GitHub Actions** (`ci.yml`) para rodar **build + testes** em cada push/PR
para `main`. Como os entregáveis são revisados por PR, o CI garante que tudo compila e passa
nos testes antes do review. Não há deploy automático.

## 📝 Code Review (por Fase)
Ao final de cada fase:
1. Criar uma branch (ex.: `fase-01`)
2. Subir os entregáveis
3. Abrir Pull Request para `main`
4. Preencher o template do PR com o que aprendeu e dúvidas
5. Aguardar review do mentor

> ⚠️ A branch padrão é `main` (a antiga `master` foi removida).

## 🔗 Recursos Úteis
- [Documentação Oficial do C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)
- [Balta.io - Fundamentos do C#](https://balta.io/)
- [Exercism - C# Track](https://exercism.org/tracks/csharp)

---

*Curso em construção — Fase 00 concluída.*
