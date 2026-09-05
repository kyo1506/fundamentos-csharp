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

    // ---- Aula 00.5 / 00.6: operadores e decisões ----
    public static bool EhBissexto(int ano)
        => (ano % 4 == 0 && ano % 100 != 0) || ano % 400 == 0;

    public static int MaiorDeTres(int a, int b, int c)
    {
        if (a >= b && a >= c) return a;
        if (b >= c) return b;
        return c;
    }

    // ---- Aula 00.7: laços ----
    public static long SomarDe1Ate(int n)
    {
        long soma = 0;
        for (int i = 1; i <= n; i++) soma += i;
        return soma;
    }

    public static bool EhPrimo(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

    public static long Fatorial(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), "Não existe fatorial de negativo.");
        long resultado = 1;
        for (int i = 2; i <= n; i++) resultado *= i;
        return resultado;
    }

    // ---- Aula 00.8: arrays ----
    public static int Somatorio(int[] valores)
    {
        int soma = 0;
        foreach (var v in valores) soma += v;
        return soma;
    }

    public static int MaiorValor(int[] valores)
    {
        if (valores.Length == 0) throw new ArgumentException("Array vazio.");
        int maior = valores[0];
        foreach (var v in valores)
            if (v > maior) maior = v;
        return maior;
    }

    public static double Media(int[] valores)
        => valores.Length == 0 ? 0 : (double)Somatorio(valores) / valores.Length;

    public static string Inverter(string texto)
    {
        var caracteres = texto.ToCharArray();
        Array.Reverse(caracteres);
        return new string(caracteres);
    }

    public static bool EhPalindromo(string texto)
    {
        var limpo = texto.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray();
        int i = 0, j = limpo.Length - 1;
        while (i < j)
        {
            if (limpo[i] != limpo[j]) return false;
            i++;
            j--;
        }
        return true;
    }

    // ---- Aula 00.11: exercícios clássicos (projeto integrador) ----
    public static string FizzBuzz(int n) => (n % 3, n % 5) switch
    {
        (0, 0) => "FizzBuzz",
        (0, _) => "Fizz",
        (_, 0) => "Buzz",
        _      => n.ToString()
    };

    public static int SomarDigitos(int n)
    {
        int valor = Math.Abs(n);
        int soma = 0;
        while (valor > 0)
        {
            soma += valor % 10;
            valor /= 10;
        }
        return soma;
    }

    public static long Fibonacci(int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
        if (n <= 1) return n;
        long a = 0, b = 1;
        for (int i = 2; i <= n; i++)
        {
            long prox = a + b;
            a = b;
            b = prox;
        }
        return b;
    }

    public static (int Maior, int Menor) MaiorMenor(int[] v)
    {
        if (v.Length == 0) throw new ArgumentException("Array vazio.");
        int maior = v[0], menor = v[0];
        foreach (var x in v)
        {
            if (x > maior) maior = x;
            if (x < menor) menor = x;
        }
        return (maior, menor);
    }

    public static int[] PrimosAte(int n)
    {
        var resultado = new List<int>();
        for (int i = 2; i <= n; i++)
            if (EhPrimo(i)) resultado.Add(i);
        return resultado.ToArray();
    }

    public static int ContarVogais(string texto)
    {
        var vogais = new HashSet<char> { 'a', 'e', 'i', 'o', 'u', 'á', 'é', 'í', 'ó', 'ú', 'â', 'ê', 'ô', 'ã', 'õ' };
        int total = 0;
        foreach (var c in texto.ToLowerInvariant())
            if (vogais.Contains(c)) total++;
        return total;
    }

    // Ordenação por bolha (bubble sort) — didático. Cada passada "empurra" o maior para o fim.
    public static int[] OrdenarCrescente(int[] v)
    {
        var arr = (int[])v.Clone();
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - 1 - i; j++)
                if (arr[j] > arr[j + 1])
                    (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);  // troca (tupla)
        return arr;
    }
}
