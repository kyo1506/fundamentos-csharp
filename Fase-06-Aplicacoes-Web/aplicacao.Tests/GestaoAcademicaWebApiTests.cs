using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Fase05.GestaoAcademica.Core;

namespace Fase06.GestaoAcademica.WebApi.Tests;

public class GestaoAcademicaWebApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public GestaoAcademicaWebApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_DeveRetornarStatus200ETextoHealthy()
    {
        // Act
        var response = await _client.GetAsync("/healthz");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string conteudo = await response.Content.ReadAsStringAsync();
        Assert.Equal("\"Healthy\"", conteudo);
    }

    [Fact]
    public async Task Auth_ComCredenciaisValidas_DeveRetornarTokenJwt()
    {
        // Arrange
        var login = new LoginRequest("admin", "admin123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/token", login);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(resultado);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Token));
        Assert.Equal("Bearer", resultado.Tipo);
    }

    [Fact]
    public async Task Auth_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
    {
        // Arrange
        var login = new LoginRequest("usuario-invalido", "senha-errada");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/token", login);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListarCursos_DeveRetornarCursosIniciaisSeedados()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/cursos");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var cursos = await response.Content.ReadFromJsonAsync<List<Curso>>();
        Assert.NotNull(cursos);
        Assert.NotEmpty(cursos);
    }

    [Fact]
    public async Task ObterCursoPorId_QuandoIdNaoExiste_DeveRetornarNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/cursos/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CriarCurso_SemToken_DeveRetornarUnauthorized()
    {
        // Arrange
        var novoCurso = new Curso(50, "Docker e K8s", 900m, 20);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/cursos", novoCurso);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CriarCurso_ComTokenAdmin_DeveCriarCursoRetornandoCreated()
    {
        // Arrange - Obter token admin
        var loginResp = await _client.PostAsJsonAsync("/api/v1/auth/token", new LoginRequest("admin", "admin123"));
        var loginData = await loginResp.Content.ReadFromJsonAsync<LoginResponse>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/cursos");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData!.Token);
        request.Content = JsonContent.Create(new Curso(99, "Engenharia de Software", 750m, 15));

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task MatricularAluno_ComDadosValidos_DeveRetornarCreatedComDesconto()
    {
        // Arrange: Aluno 1 tem convênio (25% de desconto no Curso 1 de R$ 800 -> R$ 600)
        var dto = new { AlunoId = 1, CursoId = 1 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/matriculas", dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var matricula = await response.Content.ReadFromJsonAsync<Matricula>();
        Assert.NotNull(matricula);
        Assert.Equal(1, matricula.AlunoId);
        Assert.Equal(1, matricula.CursoId);
        Assert.Equal(600m, matricula.ValorCobrado);
    }

    [Fact]
    public async Task MatricularAluno_QuandoVagasEsgotadas_DeveRetornarConflict()
    {
        // Arrange: Curso 2 possui apenas 1 vaga total
        // Matrícula 1 ocupa a única vaga
        await _client.PostAsJsonAsync("/api/v1/matriculas", new { AlunoId = 1, CursoId = 2 });

        // Act: Matrícula 2 deve estourar o limite de vagas
        var response = await _client.PostAsJsonAsync("/api/v1/matriculas", new { AlunoId = 2, CursoId = 2 });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
