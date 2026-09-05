using Fase00.LogicaEmAcao;

Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║  LÓGICA EM AÇÃO — FASE 00                         ║");
Console.WriteLine("║  Fundamentos da Programação com C#               ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");

// Exemplos das primeiras aulas (o menu completo cresce a cada aula).
Console.WriteLine("\n── Testes rápidos das funções de lógica ──");

foreach (var n in new[] { 1, 2, 9, 10 })
{
    Console.WriteLine($"  {n} é par? {Logica.EhPar(n)}");
}

foreach (var media in new[] { 5.5, 6.0, 7.5, 9.2 })
{
    Console.WriteLine($"  Média {media:N1} → {Logica.ClassificarMedia(media)}");
}

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
