# Fase 02: Programação Orientada a Objetos + SOLID

> Domine classes e objetos, encapsulamento rigoroso de regras, herança vs composição,
> polimorfismo dinâmico com interfaces, os cinco princípios SOLID na prática e injeção de dependência por construtor.

## Ordem recomendada (teoria)
Leia as lições em ordem:

1. `teoria/02.1-classes-objetos-encapsulamento.md`
2. `teoria/02.2-heranca-override-classes-abstratas.md`
3. `teoria/02.3-polimorfismo-interfaces.md`
4. `teoria/02.4-composicao-vs-heranca.md`
5. `teoria/02.5-principios-solid-pratica.md`
6. `teoria/02.6-interfaces-segregadas-injecao-dependencia.md`

## Estrutura da Fase
```
Fase-02-POO-SOLID/
├── teoria/            # 6 lições em Markdown com teoria e código prático
├── aplicacao/         # Projeto console PooSolid (domínio acadêmico modelado com POO + SOLID)
├── aplicacao.Tests/   # Suíte de testes unitários com xUnit
├── praticas/          # Códigos experimentais
└── exercicios/        # Espaço para desafios da fase
```

## Como rodar e testar
```bash
# Executar a aplicação demonstrativa
dotnet run --project aplicacao/PooSolid.csproj

# Executar a suíte de testes unitários
dotnet test aplicacao.Tests/PooSolid.Tests.csproj
```

## O que você domina ao terminar
- Proteger o estado de objetos utilizando encapsulamento e propriedades com validação.
- Projetar herança correta respeitando `virtual`, `override` e classes abstratas.
- Desacoplar contratos de implementação através de interfaces limpas e segregadas (ISP).
- Aplicar o princípio de substituição de Liskov (LSP) evitando surpresas de runtime.
- Extensibilidade sem modificação (OCP) através do padrão Strategy.
- Implementar inversão de dependência (DIP) com Primary Constructors e alta testabilidade com Fakes.
