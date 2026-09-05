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
