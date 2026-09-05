namespace Fase00.LogicaEmAcao;

/// <summary>
/// Funções puras de lógica da Fase 00. Função "pura" = mesmo argumento → mesmo
/// resultado, sem efeitos colaterais (não imprime, não lê teclado). Isso permite
/// testar cada regra isoladamente com xUnit.
/// </summary>
public static class Logica
{
    // ---- Aula 00.4 / 00.6: operadores ----
    public static bool EhPar(int numero) => numero % 2 == 0;

    public static string ClassificarMedia(double media) => media switch
    {
        < 0 or > 10 => "Média inválida",
        < 6 => "Reprovado",
        < 8 => "Aprovado",
        _ => "Aprovado com destaque"
    };

    // ---- Aula 00.6: decisões ----
    public static int MaiorDeDois(int a, int b) => a >= b ? a : b;

    public static string ClassificarIdade(int idade) => idade switch
    {
        < 0 => "Inválida",
        <= 12 => "Criança",
        <= 17 => "Adolescente",
        <= 59 => "Adulto",
        _ => "Idoso"
    };
}
