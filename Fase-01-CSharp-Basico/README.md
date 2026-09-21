# Fase 01 — C# Básico: a linguagem em profundidade

> Domine os tipos fundamentais, memória (stack/heap, boxing/unboxing), manipulação de texto,
> números com precisão monetária (`decimal`), coleções da BCL, tratamento profissional de exceções,
> structs, enums, records, pattern matching moderno e datas com `DateOnly`/`TimeSpan`.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/01.1-tipos-valor-referencia-memoria.md`
2. `teoria/01.2-strings-stringbuilder-formatacao.md`
3. `teoria/01.3-numeros-decimal-math.md`
4. `teoria/01.4-colecoes-fundamentais.md`
5. `teoria/01.5-excecoes-e-erros.md`
6. `teoria/01.6-struct-enum-tuplas-records.md`
7. `teoria/01.7-pattern-matching-switch.md`
8. `teoria/01.8-namespaces-using-organizacao.md`
9. `teoria/01.9-classes-metodos-extensao.md`
10. `teoria/01.10-datetime-timespan-datas.md`

## Estrutura da Fase
```
Fase-01-CSharp-Basico/
├── teoria/            # 10 lições densas e práticas em Markdown
├── aplicacao/         # Projeto console CSharpBasico (menu + Basico.cs)
├── aplicacao.Tests/   # Testes unitários com xUnit
├── praticas/          # Códigos experimentais
└── exercicios/        # Espaço para exercícios práticos
```

## Como rodar e testar
```bash
# Executar a aplicação demonstrativa
dotnet run --project aplicacao/CSharpBasico.csproj

# Executar a suíte de testes unitários
dotnet test aplicacao.Tests/CSharpBasico.Tests.csproj
```

## O que você domina ao terminar
- Diferenciar alocação em Stack vs Heap e evitar o custo oculto de boxing/unboxing.
- Manipular strings com eficiência usando `StringBuilder` e comparações ordinais.
- Evitar armadilhas de arredondamento financeiro usando `decimal` e `MidpointRounding`.
- Escolher a coleção certa (`List`, `Dictionary`, `HashSet`, `Stack`, `Queue`) para complexidade $O(1)$.
- Estruturar tratamento robusto de exceções com filtros `when` sem perder o stack trace.
- Modelar dados leves e imutáveis com `readonly struct`, tuplas e `records`.
- Escrever código limpo e expressivo com pattern matching e expressões `switch`.
- Escrever métodos de extensão seguros e manipular datas sem problemas de timezone.
