using Fase00.LogicaEmAcao;

// ============================================
// LÓGICA EM AÇÃO — Fase 00 (menu integrador)
// Cada opção chama uma função pura de Logica.cs.
// ============================================
Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║  LÓGICA EM AÇÃO — FASE 00                         ║");
Console.WriteLine("║  Fundamentos da Programação com C#               ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");

do
{
    Console.WriteLine("\n── Menu de exercícios ──");
    Console.WriteLine("  1  FizzBuzz");
    Console.WriteLine("  2  Soma de dígitos");
    Console.WriteLine("  3  Fibonacci");
    Console.WriteLine("  4  Maior e menor de um array");
    Console.WriteLine("  5  Jogo: adivinhe o número");
    Console.WriteLine("  6  Primos até N");
    Console.WriteLine("  7  Contar vogais");
    Console.WriteLine("  8  Ordenar (bolha)");
    Console.WriteLine("  0  Sair");
    Console.Write("Escolha: ");

    var opcao = Console.ReadLine()?.Trim();

    switch (opcao)
    {
        case "1":
            for (int i = 1; i <= 20; i++) Console.WriteLine($"  {i,2} → {Logica.FizzBuzz(i)}");
            break;
        case "2":
            Console.Write("Número: ");
            if (int.TryParse(Console.ReadLine(), out int nd) && nd >= 0)
                Console.WriteLine($"  Soma dos dígitos: {Logica.SomarDigitos(nd)}");
            else Console.WriteLine("  Entrada inválida.");
            break;
        case "3":
            Console.Write("Posição (n): ");
            if (int.TryParse(Console.ReadLine(), out int nf))
                Console.WriteLine($"  Fibonacci({nf}) = {Logica.Fibonacci(nf)}");
            else Console.WriteLine("  Entrada inválida.");
            break;
        case "4":
            var v = LerArray();
            if (v.Length > 0)
            {
                var (maior, menor) = Logica.MaiorMenor(v);
                Console.WriteLine($"  Maior: {maior} · Menor: {menor}");
            }
            break;
        case "5":
            JogarAdivinhacao();
            break;
        case "6":
            Console.Write("Até qual número? ");
            if (int.TryParse(Console.ReadLine(), out int np))
                Console.WriteLine($"  Primos: {string.Join(", ", Logica.PrimosAte(np))}");
            else Console.WriteLine("  Entrada inválida.");
            break;
        case "7":
            Console.Write("Texto: ");
            Console.WriteLine($"  Vogais: {Logica.ContarVogais(Console.ReadLine() ?? "")}");
            break;
        case "8":
            var v8 = LerArray();
            if (v8.Length > 0)
                Console.WriteLine($"  Ordenado: {string.Join(", ", Logica.OrdenarCrescente(v8))}");
            break;
        case "0":
            Console.WriteLine("Até mais!");
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }

    if (opcao != "0")
    {
        Console.Write("\nPressione ENTER para voltar ao menu...");
        Console.ReadLine();
    }
} while (true);

// ===== helpers =====
static int[] LerArray()
{
    Console.Write("Números separados por espaço: ");
    var itens = (Console.ReadLine() ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
    return itens.Select(s => int.TryParse(s, out int n) ? n : (int?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToArray();
}

static void JogarAdivinhacao()
{
    var aleatorio = new Random();
    int segredo = aleatorio.Next(1, 101);
    int tentativas = 0;
    Console.WriteLine("  Sorteie! Adivinhe o número entre 1 e 100.");

    while (true)
    {
        Console.Write("  Seu palpite: ");
        if (!int.TryParse(Console.ReadLine(), out int palpite)) continue;

        tentativas++;
        if (palpite < segredo) Console.WriteLine("  → É MAIOR.");
        else if (palpite > segredo) Console.WriteLine("  → É MENOR.");
        else
        {
            Console.WriteLine($"  🎉 Acertou! O número era {segredo} em {tentativas} tentativa(s).");
            return;
        }
    }
}
