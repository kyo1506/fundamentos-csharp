using Fase00.LogicaEmAcao;
using Xunit;

namespace Fase00.LogicaEmAcao.Tests;

public class LogicaEhParTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(-4, true)]
    [InlineData(1, false)]
    [InlineData(7, false)]
    public void EhPar_RetornaEsperado(int numero, bool esperado)
        => Assert.Equal(esperado, Logica.EhPar(numero));
}

public class LogicaClassificarMediaTests
{
    [Theory]
    [InlineData(11, "Média inválida")]
    [InlineData(-1, "Média inválida")]
    [InlineData(5.5, "Reprovado")]
    [InlineData(6.0, "Aprovado")]
    [InlineData(7.5, "Aprovado")]
    [InlineData(9.2, "Aprovado com destaque")]
    public void ClassificarMedia_RetornaRotulo(double media, string esperado)
        => Assert.Equal(esperado, Logica.ClassificarMedia(media));
}

public class LogicaDecisoesTests
{
    [Fact]
    public void MaiorDeDois_RetornaOMaior()
    {
        Assert.Equal(7, Logica.MaiorDeDois(3, 7));
        Assert.Equal(3, Logica.MaiorDeDois(3, -1));
    }

    [Theory]
    [InlineData(5, "Criança")]
    [InlineData(13, "Adolescente")]
    [InlineData(30, "Adulto")]
    [InlineData(70, "Idoso")]
    [InlineData(-2, "Inválida")]
    public void ClassificarIdade_RetornaFaixa(int idade, string esperado)
        => Assert.Equal(esperado, Logica.ClassificarIdade(idade));
}

public class LogicaBissextoTests
{
    [Theory]
    [InlineData(2000, true)]   // div por 400
    [InlineData(2024, true)]   // div por 4, não por 100
    [InlineData(2023, false)]
    [InlineData(1900, false)]  // div por 100, não por 400
    public void EhBissexto_RetornaEsperado(int ano, bool esperado)
        => Assert.Equal(esperado, Logica.EhBissexto(ano));
}

public class LogicaMaiorDeTresTests
{
    [Fact]
    public void MaiorDeTres_RetornaOMaior()
    {
        Assert.Equal(9, Logica.MaiorDeTres(3, 9, 5));
        Assert.Equal(10, Logica.MaiorDeTres(10, 1, 2));
        Assert.Equal(7, Logica.MaiorDeTres(1, 2, 7));
        Assert.Equal(5, Logica.MaiorDeTres(5, 5, 3));
    }
}

public class LogicaLacosTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 15)]
    [InlineData(100, 5050)]
    public void SomarDe1Ate_SomaCorretamente(int n, long esperado)
        => Assert.Equal(esperado, Logica.SomarDe1Ate(n));

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(13, true)]
    [InlineData(25, false)]
    public void EhPrimo_RetornaEsperado(int n, bool esperado)
        => Assert.Equal(esperado, Logica.EhPrimo(n));

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(5, 120)]
    public void Fatorial_CalculaCorretamente(int n, long esperado)
        => Assert.Equal(esperado, Logica.Fatorial(n));

    [Fact]
    public void Fatorial_Negativo_LancaExcecao()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Logica.Fatorial(-1));
}

public class LogicaArraysTests
{
    [Fact]
    public void Somatorio_SomaElementos() => Assert.Equal(24, Logica.Somatorio(new[] { 4, 7, 1, 9, 3 }));

    [Fact]
    public void MaiorValor_RetornaOMaior() => Assert.Equal(9, Logica.MaiorValor(new[] { 4, 7, 1, 9, 3 }));

    [Fact]
    public void Media_CalculaCorretamente() => Assert.Equal(4.0, Logica.Media(new[] { 3, 4, 5 }));

    [Fact]
    public void MaiorValor_ArrayVazio_LancaExcecao()
        => Assert.Throws<ArgumentException>(() => Logica.MaiorValor(Array.Empty<int>()));
}

public class LogicaStringTests
{
    [Theory]
    [InlineData("abc", "cba")]
    [InlineData("arara", "arara")]
    public void Inverter_InverteTexto(string texto, string esperado)
        => Assert.Equal(esperado, Logica.Inverter(texto));

    [Theory]
    [InlineData("arara", true)]
    [InlineData("A man a plan a canal Panama", true)]
    [InlineData("socorram-me subi no onibus em marrocos", true)]
    [InlineData("casa", false)]
    [InlineData("", true)]
    public void EhPalindromo_RetornaEsperado(string texto, bool esperado)
        => Assert.Equal(esperado, Logica.EhPalindromo(texto));
}

public class LogicaClassicosTests
{
    [Theory]
    [InlineData(15, "FizzBuzz")]
    [InlineData(9, "Fizz")]
    [InlineData(10, "Buzz")]
    [InlineData(7, "7")]
    public void FizzBuzz_RetornaEsperado(int n, string esperado)
        => Assert.Equal(esperado, Logica.FizzBuzz(n));

    [Theory]
    [InlineData(123, 6)]
    [InlineData(9999, 36)]
    [InlineData(0, 0)]
    public void SomarDigitos_RetornaEsperado(int n, int esperado)
        => Assert.Equal(esperado, Logica.SomarDigitos(n));

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(6, 8)]
    [InlineData(10, 55)]
    public void Fibonacci_RetornaEsperado(int n, long esperado)
        => Assert.Equal(esperado, Logica.Fibonacci(n));

    [Fact]
    public void MaiorMenor_RetornaAmbos()
    {
        var (maior, menor) = Logica.MaiorMenor(new[] { 3, -5, 9, 0, 7 });
        Assert.Equal(9, maior);
        Assert.Equal(-5, menor);
    }

    [Fact]
    public void PrimosAte_RetornaPrimos()
        => Assert.Equal(new[] { 2, 3, 5, 7, 11, 13 }, Logica.PrimosAte(13));

    [Theory]
    [InlineData("aeiou", 5)]
    [InlineData("XYZ", 0)]
    [InlineData("Olá, mundo!", 4)]
    [InlineData("", 0)]
    public void ContarVogais_RetornaEsperado(string texto, int esperado)
        => Assert.Equal(esperado, Logica.ContarVogais(texto));

    [Fact]
    public void OrdenarCrescente_Ordena()
        => Assert.Equal(new[] { 1, 2, 3, 4, 5 }, Logica.OrdenarCrescente(new[] { 5, 3, 4, 1, 2 }));

    [Fact]
    public void OrdenarCrescente_NaoAlteraArrayOriginal()
    {
        int[] original = { 5, 1 };
        Logica.OrdenarCrescente(original);
        Assert.Equal(new[] { 5, 1 }, original);
    }
}
