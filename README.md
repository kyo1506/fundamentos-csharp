# Fundamentos do C# (.NET) — Plano de Estudos Guiado

> Repositório de estudos para consolidar fundamentos de C# e .NET com foco em tipos por valor/referência, pilares da OO, coleções, LINQ e APIs.

## 📋 Sobre o Projeto

Este repositório faz parte de um plano de estudos de 4 semanas para aprofundar conhecimentos em:

- **Semana 1:** Stack vs Heap, Tipos por Valor vs Referência, Boxing/Unboxing
- **Semana 2:** Pilares da OO (Encapsulamento, Herança, Polimorfismo, Abstração) + SOLID
- **Semana 3:** Coleções, Generics, LINQ
- **Semana 4:** ASP.NET Core Minimal API + Deploy em Nuvem

## 🚀 Como Usar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) ou [JetBrains Rider](https://www.jetbrains.com/rider/)
- [Git](https://git-scm.com/)

### Estrutura do Repositório

```
fundamentos-csharp/
├── .github/
│   └── workflows/
│       └── deploy.yml      # CI/CD com GitHub Actions
├── Semana-01/
│   ├── teoria/             # Anotações e resumos teóricos
│   ├── praticas/           # Exemplos práticos contextualizados
│   ├── aplicacao/          # Projeto integrador da semana
│   └── desafios-semanais/  # Exercícios resolvidos
├── Semana-02/
├── Semana-03/
├── Semana-04/
└── docs/                   # Documentação adicional
```

### Executar Localmente

```bash
# Clonar o repositório
git clone https://github.com/USUARIO/fundamentos-csharp.git
cd fundamentos-csharp

# Restaurar dependências
dotnet restore

# Executar projeto da Semana 1
dotnet run --project Semana-01/aplicacao/Semana01.ControleEstoque.csproj

# Executar testes
dotnet test
```

## 📅 Cronograma de Estudos

| Semana | Tema | Status |
|--------|------|--------|
| 1 | Memória, Tipos e a Base de Tudo | 🔄 Em andamento |
| 2 | Programação Orientada a Objetos | ⏳ Pendente |
| 3 | Coleções, LINQ e Manipulação de Dados | ⏳ Pendente |
| 4 | APIs, Persistência e Deploy | ⏳ Pendente |

## 🎯 Metodologia TEAP

Cada sessão de estudo segue 4 etapas:

1. **T — Teoria Sucinta:** Conceito explicado de forma direta, com diagramas
2. **E — Exemplo Contextualizado:** Código real (automação, validação, APIs)
3. **A — Aplicação em Projeto:** Adaptação do exemplo para o projeto integrador
4. **P — Problema para Resolver:** Desafio entregue via PR para code review

## 📝 Code Review

Ao final de cada semana:

1. Criar uma branch `semana-XX`
2. Subir todos os entregáveis
3. Abrir Pull Request para `main`
4. Preencher template do PR com o que aprendeu e dúvidas
5. Aguardar review do mentor

## 🔗 Recursos Úteis

- [Documentação Oficial do C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)
- [Balta.io - Fundamentos do C#](https://balta.io/)
- [Exercism - C# Track](https://exercism.org/tracks/csharp)

---

*Plano criado em: 29 de agosto de 2026*
