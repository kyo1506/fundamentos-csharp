using System.Text.Json.Serialization;

namespace Fase04.RelatoriosAsyncCli;

/// <summary>
/// Atributo customizado para metadados de colunas em relatórios exportados.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class ColunaExportacaoAttribute : Attribute
{
    public string NomeCabecalho { get; }
    public int Ordem { get; }

    public ColunaExportacaoAttribute(string nomeCabecalho, int ordem = 0)
    {
        NomeCabecalho = nomeCabecalho;
        Ordem = ordem;
    }
}

/// <summary>
/// Item representativo de uma matrícula/avaliação acadêmica para exportação.
/// </summary>
public record ItemRelatorio(
    [property: ColunaExportacao("ID", 1)]
    int Id,

    [property: ColunaExportacao("Curso", 2)]
    string TituloCurso,

    [property: ColunaExportacao("Estudante", 3)]
    string NomeAluno,

    [property: ColunaExportacao("Nota Final", 4)]
    decimal Nota,

    [property: ColunaExportacao("Conclusão", 5)]
    DateOnly DataConclusao,

    [property: ColunaExportacao("Status", 6)]
    string Status
);

/// <summary>
/// Resumo estatístico do processamento assíncrono de relatórios.
/// </summary>
public record ResumoRelatorio(
    int TotalProcessado,
    decimal MediaGeral,
    int TotalAprovados,
    DateTimeOffset GeradoEmUtc,
    long DuracaoMs
);

/// <summary>
/// Contexto gerador de código (Source Generator) do System.Text.Json.
/// Garante serialização ultra-rápida, zero reflexão em runtime e compatibilidade com Native AOT.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ItemRelatorio))]
[JsonSerializable(typeof(List<ItemRelatorio>))]
[JsonSerializable(typeof(ResumoRelatorio))]
[JsonSerializable(typeof(List<ResumoRelatorio>))]
public partial class RelatorioJsonContext : JsonSerializerContext
{
}
