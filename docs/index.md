# Fundamentos do C# — Documentação

> Repositório de estudos para consolidar fundamentos de C# e .NET

## 📋 Visão Geral

Este repositório faz parte de um plano de estudos de 4 semanas focado em:

| Semana | Tema | Contexto |
|--------|------|----------|
| 1 | Stack/Heap, Tipos por Valor vs Referência | Automação de planilhas financeiras |
| 2 | Pilares da OO + SOLID | Validação de formulários de inscrição |
| 3 | Coleções, Generics, LINQ | Análise de vendas de loja online |
| 4 | ASP.NET Core Minimal API + Deploy | API RESTful de controle de estoque |

## 🎯 Metodologia TEAP

Cada sessão de estudo segue 4 etapas:

1. **T — Teoria Sucinta:** Conceito explicado de forma direta, com diagramas
2. **E — Exemplo Contextualizado:** Código real (automação, validação, APIs)
3. **A — Aplicação em Projeto:** Adaptação do exemplo para o projeto integrador
4. **P — Problema para Resolver:** Desafio entregue via PR para code review

## 📁 Estrutura

```
fundamentos-csharp/
├── Semana-01/
│   ├── teoria/             # Anotações e resumos teóricos
│   ├── praticas/           # Exemplos práticos contextualizados
│   ├── aplicacao/          # Projeto integrador da semana
│   └── desafios-semanais/  # Exercícios resolvidos
├── Semana-02/
├── Semana-03/
├── Semana-04/
└── docs/                   # Esta documentação
```

## 🚀 Como Executar

```bash
# Clonar
git clone https://github.com/USUARIO/fundamentos-csharp.git
cd fundamentos-csharp

# Restaurar e buildar
dotnet restore
dotnet build

# Executar projeto da Semana 1
dotnet run --project Semana-01/aplicacao/Semana01.ControleEstoque.csproj

# Executar testes
dotnet test
```

## 📝 Code Review

Ao final de cada semana, abrir um Pull Request com:

- [ ] Anotações teóricas
- [ ] Exemplos práticos
- [ ] Projeto integrador
- [ ] Desafios resolvidos
- [ ] Dúvidas e aprendizados

---

*Documentação criada em: 29 de agosto de 2026*
