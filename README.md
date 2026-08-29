# Fundamentos do C# (.NET) — Plano de Estudos Guiado

> Repositório de estudos para consolidar fundamentos de C# e .NET com foco em tipos por valor/referência, pilares da OO, coleções, LINQ e APIs.

## 📋 Sobre o Projeto

Este repositório faz parte de um plano de estudos de 4 semanas. Cada semana tem
**teoria**, **prática**, **projeto integrador** (app) e **desafios** entregues via
Pull Request para code review.

| Semana | Tema | Entregável (aplicação) |
|--------|------|------------------------|
| **1** | Memória, Tipos e a Base de Tudo (Stack vs Heap, Value vs Reference, Boxing/Unboxing) | `Semana01.ControleEstoque` (console) |
| **2** | Programação Orientada a Objetos (pilares + SOLID) | `Semana02.ValidadorFormulario` (console) |
| **3** | Coleções, Generics e LINQ | `Semana03.AnaliseDados` (console) |
| **4** | ASP.NET Core Minimal API + Deploy | `Semana04.APIControleEstoque` (console + testes) |

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
│   └── workflows/ci.yml           # CI: build + testes (GitHub Actions)
├── Semana-01/
│   ├── teoria/                    # Anotações e resumos teóricos
│   ├── praticas/                  # Exemplos práticos contextualizados
│   ├── aplicacao/                 # Projeto integrador da semana
│   ├── aplicacao.Tests/           # Testes unitários (xUnit)
│   └── desafios-semanais/         # Exercícios resolvidos
├── Semana-02/                     # (mesma estrutura)
├── Semana-03/
├── Semana-04/
├── FundamentosCSharp.slnx         # Solução .NET
└── docs/
```

### Executar Localmente

```bash
git clone https://github.com/kyo1506/fundamentos-csharp.git
cd fundamentos-csharp

# Restaurar e compilar a solução
dotnet restore FundamentosCSharp.slnx
dotnet build FundamentosCSharp.slnx

# Executar os testes de todas as semanas
dotnet test FundamentosCSharp.slnx

# Executar o projeto da semana desejada
dotnet run --project Semana-01/aplicacao/Semana01.ControleEstoque.csproj
```

## 🎯 Metodologia TEAP

Cada sessão de estudo segue 4 etapas:

1. **T — Teoria Sucinta:** conceito explicado de forma direta, com diagramas
2. **E — Exemplo Contextualizado:** código real (automação, validação, APIs)
3. **A — Aplicação em Projeto:** adaptação do exemplo para o projeto integrador
4. **P — Problema para Resolver:** desafio entregue via PR para code review

## 🔁 CI/CD

O repositório usa **GitHub Actions** (`ci.yml`) para rodar **build + testes** em cada
push/PR para `main`. Como os entregáveis são revisados por PR, o CI garante que o código
compila e passa nos testes antes do review — o revisor foca na qualidade, não na compilação.
Não há deploy automático (repositório de estudos).

## 📝 Code Review

Ao final de cada semana:

1. Criar uma branch `semana-XX`
2. Subir todos os entregáveis
3. Abrir Pull Request para `main`
4. Preencher o template do PR com o que aprendeu e dúvidas
5. Aguardar review do mentor

> ⚠️ A branch padrão é `main` (a antiga `master` foi removida).

## 🔗 Recursos Úteis

- [Documentação Oficial do C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)
- [Balta.io - Fundamentos do C#](https://balta.io/)
- [Exercism - C# Track](https://exercism.org/tracks/csharp)

---

*Plano criado em: 29 de agosto de 2026*
