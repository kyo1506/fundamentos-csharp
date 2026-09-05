# Fundamentos do C# — Documentação

> Repositório de estudos (curso) de C# e .NET do básico ao avançado.
> O roadmap completo está em [`../CURRICULO.md`](../CURRICULO.md).

## 📋 Visão Geral

O curso é organizado em **Fases** progressivas. Cada fase tem teoria, exemplos práticos,
um projeto integrador com testes e exercícios/desafios para code review via Pull Request.

| Fase | Tema | Entregável |
|------|------|------------|
| 00 | Fundamentos da Programação (lógica com C#) | `LogicaEmAcao` (console) |
| 01 | C# em profundidade (tipos/memória/coleções/strings) | console |
| 02 | POO + SOLID | console |
| 03 | Coleções avançadas, Delegados, Eventos, Generics, LINQ | console |
| 04 | .NET Runtime & BCL (IO, JSON, async, threads) | CLI |
| 05 | Arquitetura, Padrões, DI e Testes | lib + testes |
| 06 | Aplicações Web e Serviços | API REST |
| 07 | Tópicos Avançados & Performance | lib + benchmarks |

## 🎯 Metodologia TEAP

1. **T — Teoria Sucinta:** conceito explicado de forma direta, com diagramas
2. **E — Exemplo Contextualizado:** código real e executável
3. **A — Aplicação em Projeto:** exemplo aplicado no projeto integrador da fase
4. **P — Problema para Resolver:** desafio entregue via PR para code review

## 📁 Estrutura

```
fundamentos-csharp/
├── CURRICULO.md
├── Fase-00-Fundamentos-da-Programacao/
│   ├── teoria/             # Aulas em Markdown
│   ├── aplicacao/          # Projeto integrador (console)
│   ├── aplicacao.Tests/    # Testes xUnit
│   └── README.md           # Guia da fase
├── Fase-01-.../            # (próximas fases)
├── FundamentosCSharp.slnx
└── docs/                   # Esta documentação
```

## 🚀 Como Executar

```bash
# Clonar
git clone https://github.com/kyo1506/fundamentos-csharp.git
cd fundamentos-csharp

# Restaurar, compilar e testar
dotnet restore FundamentosCSharp.slnx
dotnet build FundamentosCSharp.slnx
dotnet test FundamentosCSharp.slnx

# Executar o projeto da Fase 00
dotnet run --project Fase-00-Fundamentos-da-Programacao/aplicacao/LogicaEmAcao.csproj
```

## 📝 Code Review (por Fase)

Ao final de cada fase, abrir um Pull Request para `main` com:

- [ ] Anotações teóricas
- [ ] Exemplos práticos
- [ ] Projeto integrador
- [ ] Exercícios/desafios resolvidos
- [ ] Dúvidas e aprendizados

---

*Curso em construção — Fase 00 concluída.*
