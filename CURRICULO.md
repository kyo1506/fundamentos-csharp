# Currículo — Fundamentos de C#/.NET (do Básico ao Avançado)

> Curso completo e **didático e progressivo** de C# e do ecossistema .NET.
> Organizado em **Fases** (cada fase = um bloco de aprendizado coerente), com
> **aulas** densas, **exemplos práticos**, um **projeto integrador** e **exercícios/desafios**
> entregues por Pull Request para code review. Este documento é o índice do curso.

## Como ler este documento
- Cada **Fase** é uma pasta `Fase-NN-Nome/` com: `teoria/` (aulas em Markdown), `praticas/`,
  `aplicacao/` (projeto console da fase), `aplicacao.Tests/` e `exercicios/`.
- As fases são **sequenciais**: faça na ordem. Ao final de cada fase, entregue o desafio por PR.
- Repositório é fonte do portal de estudos (`portal-estudos-csharp`), que espelha as lições/exercícios.

> **Status da reestruturação:** as pastas legadas `Semana-01..04` ainda contêm conteúdo
> parcial (tipos/memória, OO/SOLID, coleções/LINQ, ASP.NET). Elas serão **absorvidas** nas
> fases correspondentes (ver mapa no fim). Conteúdo novo nasce já no formato de fases.

---

## Trilha resumida

```
Fase 00  Fundamentos da Programação (lógica + C# do zero)      → Console
Fase 01  C# Básico: tipos, fluxo, strings, coleções            → Console
Fase 02  Programação Orientada a Objetos + SOLID                → Console
Fase 03  Coleções avançadas, Delegados, Eventos, Generics, LINQ → Console
Fase 04  .NET Runtime & BCL: exceções, IO, JSON, threads, async → Console/CLI
Fase 05  Arquitetura, Padrões, DI e Testes (TDD + code review)  → Lib + xUnit
Fase 06  Aplicações Web: HTTP, Minimal APIs, EF Core, segurança → Web
Fase 07  Tópicos Avançados & Performance                        → Lib + Benchmark
```

---

## Fase 00 — Fundamentos da Programação (lógica com C#)
**Objetivo:** aprender a *pensar* como programador e os blocos básicos da linguagem, sem pressupor experiência. **Entregável:** `LogicaEmAcao` (menu de exercícios) + testes xUnit. **Status: concluída** (pronta para revisão).

- **00.1** O que é programar — algoritmo, linguagem, compilação (C# → IL → executável)
- **00.2** Preparando o ambiente — instalar .NET, `dotnet new/run/build`, editor
- **00.3** Estrutura de um programa C# e **variáveis/tipos** básicos (`int, decimal, bool, string`, `var`, constantes)
- **00.4** Entrada e saída — `Console`, interpolação `$""`, `TryParse`, formatação/cultura
- **00.5** Operadores — aritmética, comparação, lógica, atribuição, precedência
- **00.6** Decisões — `if/else`, `switch` clássico e **switch de expressão**
- **00.7** Laços — `for`, `while`, `do-while`, `foreach`; `break`/`continue`
- **00.8** Vetores (arrays) e `string` (métodos, imutabilidade, comparação)
- **00.9** Funções/métodos — parâmetros, retorno, escopo, função pura, organização
- **00.10** Depuração e erros comuns — stack trace, debugger, teste de mesa
- **00.11** Lógica de programação — exercícios clássicos (FizzBuzz, dígitos, Fibonacci, primos, vogais, ordenação) no projeto integrador

**Projeto integrador (Fase 00):** `LogicaEmAcao` — menu com os exercícios de lógica; funções puras em `Logica.cs` cobertas por testes.

---

## Fase 01 — C# Básico: a linguagem em profundidade
**Objetivo:** dominar tipos, memória e a sintaxe moderna do C#. **Entregável:** console + testes.

- 01.1 Tipos por valor vs referência, stack/heap, boxing/unboxing *(absorve Semana-01)*
- 01.2 `string` em profundidade, `StringBuilder`, cultura e formatação
- 01.3 Números: `decimal` para dinheiro, arredondamento, `Math`
- 01.4 Coleções fundamentais: `List<T>`, `Dictionary<K,V>`, `HashSet<T>`, `Stack`/`Queue`
- 01.5 Exceções e tratamento de erros (`try/catch/finally`, exceções customizadas)
- 01.6 `struct`, `enum`, tuplas, `record`
- 01.7 Pattern matching e expressões `switch`
- 01.8 Namespaces, `using`, organização de arquivos e `static`
- 01.9 Classes e métodos de extensão
- 01.10 `DateTime`/`TimeSpan` e formatação de datas

---

## Fase 02 — Programação Orientada a Objetos + SOLID
**Objetivo:** modelar domínios com OO e aplicar SOLID na prática. *(absorve Semana-02)*

- 02.1 Classes e objetos; encapsulamento; propriedades com validação
- 02.2 Herança, `virtual`/`override`, classes abstratas
- 02.3 Polimorfismo e interfaces
- 02.4 Composição vs herança
- 02.5 SRP, OCP, LSP, ISP, DIP — cada princípio com exemplo + anti-exemplo
- 02.6 Interfaces segregadas e injeção por construtor (visão inicial de DI)

**Projeto integrador (Fase 02):** sistema com domínio modelado por OO + validações + testes (ex.: matrícula/participantes).

---

## Fase 03 — Coleções avançadas, Delegados, Eventos, Generics e LINQ
**Objetivo:** escrever código declarativo e reutilizável. *(absorve Semana-03)*

- 03.1 Generics (classes, métodos, restrições)
- 03.2 Delegates, `Func`/`Action`, lambdas, closures
- 03.3 Eventos e `event`
- 03.4 LINQ fluente e query syntax; operadores `Where/Select/OrderBy/GroupBy/Join/Aggregate/Any/All/First...`
- 03.5 LINQ aplicado (agregações, projeções, agrupamentos)
- 03.6 Coleções do mundo real: filas/prioridade, `IEnumerable`/`IEnumerator`

**Projeto integrador (Fase 03):** análise de dados com LINQ + testes.

---

## Fase 04 — .NET Runtime & BCL: exceções, IO, JSON, threads e async
**Objetivo:** entender como o .NET executa e usar recursos do runtime de verdade.

- 04.1 Garbage Collector, `IDisposable`, `using`, finalizers
- 04.2 IO: `File`, `Stream`, `Directory`; ler/escrever texto, CSV
- 04.3 Serialização `System.Text.Json`
- 04.4 Threads e `Task`; `async/await`; `Task.WhenAll`/`WhenAny`; canais e fluxos assíncronos
- 04.5 `DateTime`, cultura e globalização
- 04.6 Reflection e `Attribute`
- 04.7 Configuração e `Microsoft.Extensions.*` (visão)

**Projeto integrador (Fase 04):** CLI que lê arquivo, processa com async e gera relatório (JSON/CSV).

---

## Fase 05 — Arquitetura, Padrões, DI e Testes
**Objetivo:** escrever software testável e bem-estruturado.

- 05.1 SOLID aplicado em arquitetura em camadas
- 05.2 Design patterns: Strategy, Factory, Repository, Observer, etc.
- 05.3 Injeção de Dependência (Microsoft.Extensions.DependencyInjection)
- 05.4 Testes: xUnit (Facts/Theory), TDD, mocks/fakes
- 05.5 Qualidade: code review, git/PR, CI

**Projeto integrador (Fase 05):** biblioteca + suíte de testes TDD com cobertura.

---

## Fase 06 — Aplicações Web e Serviços
**Objetivo:** construir APIs reais. *(absorve Semana-04)*

- 06.1 HTTP/REST na prática (`HttpClient`)
- 06.2 Minimal APIs e Controllers; validação; Swagger/OpenAPI
- 06.3 Middleware e ciclo de vida da requisição
- 06.4 Acesso a dados com EF Core (SQLite/Postgres) e migrações
- 06.5 Autenticação/autorização básica (JWT)
- 06.6 Publicação e deploy (containers, Railway/Render/Azure)

---

## Fase 07 — Tópicos Avançados & Performance
**Objetivo:** alicerces para nível sênior.

- 07.1 Performance: alocação, `Span<T>`/`Memory<T>`, `ref struct`
- 07.2 Async avançado: `ValueTask`, `Channels`, pipelines
- 07.3 Generics avançados, `System.Text` e `Regex` performático
- 07.4 Source generators e metaprogramação
- 07.5 Estudo de benchmarks (`BenchmarkDotNet`) e profiling

---

## Mapa de absorção do conteúdo legado

| Pasta legada | Conteúdo | Destino no novo currículo |
|--------------|----------|----------------------------|
| `Semana-01/aplicacao` (ControleEstoque) | tipos/memória/boxing | Fase 01 |
| `Semana-01/teoria` | stack-heap, valor/referência | Fase 01 |
| `Semana-02/aplicacao` (ValidadorFormulario) | OO + SOLID | Fase 02 |
| `Semana-03/aplicacao` (AnaliseDados) | coleções + LINQ | Fase 03 |
| `Semana-04/aplicacao` (APIControleEstoque) | ASP.NET | Fase 06 |

> O conteúdo antigo será **movido/enriquecido** para as fases correspondentes em uma
> etapa de reconciliação posterior (mantendo testes verdes). As fases `00` e `01..` novas
> já são escritas no novo formato.

---

## Critérios de qualidade por aula
Cada aula deve ter, quando aplicável:
1. **Conceito** explicado com analogia e precisão técnica.
2. **Exemplo real** curto e executável.
3. **Anti-exemplo** ou pegadinha comum (por quê não fazer assim).
4. **Exercícios** progressivos (fácil → médio → desafiador).
5. Referência ao projeto integrador da fase.
