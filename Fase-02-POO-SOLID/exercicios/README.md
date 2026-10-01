# Desafios Práticos: Fase 02 (POO + SOLID)

Conjunto de 4 desafios práticos para fixar a modelagem orientada a objetos, encapsulamento, polimorfismo e os princípios SOLID.

---

## Desafio 1: Value Object CPF (Encapsulamento e SRP)
- **Objetivo:** Criar um tipo imutável `Cpf` que higieniza e valida o documento no momento da instanciação.
- **Regras:**
  - Remove pontos, hifens e espaços (`123.456.789-00` -> `12345678900`).
  - Deve conter exatamente 11 dígitos numéricos e não pode ter todos os dígitos iguais (ex.: `11111111111`).
  - Lança `ArgumentException` se for nulo, vazio ou inválido.
  - Expõe propriedade `ValorFormatado` no formato `000.000.000-00` e método `ToString()`.

---

## Desafio 2: Política de Reembolso Progressivo (Strategy e OCP)
- **Objetivo:** Implementar o cálculo de reembolso no cancelamento de matrículas sem quebrar o princípio Aberto/Fechado.
- **Regras:**
  - Interface `IPoliticaReembolso` com método `decimal CalcularReembolso(decimal valorPago, int diasDecorridos)`.
  - `ReembolsoProgressivoPolitica`:
    - Até 7 dias decorridos (inclusivo): 100% do valor pago.
    - De 8 a 30 dias decorridos (inclusivo): 50% do valor pago.
    - Acima de 30 dias: 0% (sem reembolso).

---

## Desafio 3: Emissão de Certificados Polimórficos (LSP e Polimorfismo)
- **Objetivo:** Gerar texto de certificados respeitando o princípio de substituição de Liskov.
- **Regras:**
  - Classe abstrata `CertificadoBase(Aluno aluno, Curso curso, DateOnly dataEmissao)`.
  - `CertificadoConclusao`: exige `decimal NotaFinal` (deve ser >= 7.0m). Método `GerarTexto()` retorna:
    `"Certificamos que [Aluno] concluiu com êxito o curso [Curso] com nota [NotaFinal:F1]."`
  - `CertificadoParticipacao`: método `GerarTexto()` retorna:
    `"Certificamos que [Aluno] participou das atividades do curso [Curso]."`

---

## Desafio 4: Gestão de Turma e Vagas (Invariantes de Domínio)
- **Objetivo:** Controlar a capacidade máxima de uma turma garantindo que o limite nunca seja ultrapassado.
- **Regras:**
  - Classe `Turma(Guid id, Curso curso, int capacidadeMaxima)`.
  - Método `bool MatricularAluno(Aluno aluno)`:
    - Retorna `false` se a turma já tiver atingido a capacidade máxima.
    - Retorna `false` se o aluno já estiver matriculado nesta turma.
    - Retorna `true` e registra o aluno se houver vaga.
  - Propriedade `VagasRestantes => CapacidadeMaxima - AlunosMatriculados.Count`.
