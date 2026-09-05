# Fase 00 — Fundamentos da Programação (lógica com C#)

> Primeiro contato com programação, do zero. Não é preciso experiência prévia.
> Ao final, você pensa em algoritmos e escreve funções C# simples e testáveis.

## Ordem recomendada (teoria)
Leia em ordem: cada aula depende das anteriores.

1. `teoria/00.1-o-que-e-programar.md`
2. `teoria/00.2-ambiente-dotnet.md`
3. `teoria/00.3-variaveis-e-tipos.md`
4. `teoria/00.4-entrada-saida-formatacao.md`
5. `teoria/00.5-operadores.md`
6. `teoria/00.6-decisoes-if-switch.md`
7. `teoria/00.7-lacos.md`
8. `teoria/00.8-arrays-e-strings.md`
9. `teoria/00.9-funcoes.md`
10. `teoria/00.10-depuracao-e-erros.md`
11. `teoria/00.11-logica-exercicios-classicos.md` *(projeto integrador)*

## Estrutura
```
Fase-00-Fundamentos-da-Programacao/
├── teoria/            # aulas em Markdown
├── aplicacao/         # projeto console LogicaEmAcao (menu + Logica.cs)
├── aplicacao.Tests/   # testes xUnit das funções
└── exercicios/        # espaço p/ resolver exercícios extras
```

## Como rodar e testar
```bash
# executar o menu de exercícios
dotnet run --project aplicacao/LogicaEmAcao.csproj

# rodar os testes
dotnet test aplicacao.Tests/LogicaEmAcao.Tests.csproj
```

## O que você domina ao terminar
- Ler e entender um programa C# executado de cima para baixo.
- Declarar variáveis/tipos, ler entrada e formatar saída.
- Usar operadores, `if`/`switch`, laços, arrays e `string`.
- Escrever **funções puras** (testáveis) e organizar um programa em funções.
- Depurar com teste de mesa e ler erros (stack trace).
- Resolver exercícios clássicos de lógica e cobri-los com testes.

## Regra de estilo usada aqui
Mantenha **regras** em funções puras (mesma entrada → mesma saída) e deixe
`Console`/entrada no chamador. É isso que torna o código testável.
