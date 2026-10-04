using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Fase05.GestaoAcademica.Core;
using Fase06.GestaoAcademica.WebApi;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do EF Core em memória para a aplicação Web
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("GestaoAcademicaDb");
});

// 2. Registro dos Repositórios EF Core adaptados para a arquitetura do Core
builder.Services.AddScoped<IAlunoRepository, EfAlunoRepository>();
builder.Services.AddScoped<ICursoRepository, EfCursoRepository>();
builder.Services.AddScoped<IMatriculaRepository, EfMatriculaRepository>();

// 3. Serviços de Domínio e Políticas da Fase 05
builder.Services.AddTransient<IPoliticaDesconto, DescontoConvenioPolicy>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IMatriculaService, MatriculaService>();

// 4. Segurança e Autenticação JWT
string chaveSecreta = builder.Configuration["Jwt:ChaveSecreta"] ?? JwtHelper.ChaveSecretaPadrao;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta)),
            ValidateIssuer = true,
            ValidIssuer = JwtHelper.Emissor,
            ValidateAudience = true,
            ValidAudience = JwtHelper.Audiencia,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApenasAdmin", policy => policy.RequireRole("Admin"));
});

// 5. Suporte a ProblemDetails
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Seed de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Alunos.Any())
    {
        db.Alunos.AddRange(
            new Aluno(1, "Juliana Prado", "juliana@email.com", possuiConvenioEmpresarial: true),
            new Aluno(2, "Rodrigo Lima", "rodrigo@email.com", possuiConvenioEmpresarial: false)
        );
        db.Cursos.AddRange(
            new Curso(1, "C# & .NET 10 Moderno", 800m, vagasTotais: 10),
            new Curso(2, "Minimal APIs & EF Core", 600m, vagasTotais: 1)
        );
        db.SaveChanges();
    }
}

// ------------------------------------------------------------------------
// Mapeamento dos Endpoints (Minimal APIs)
// ------------------------------------------------------------------------

// Health check para orquestradores
app.MapGet("/healthz", () => Results.Ok("Healthy"));

// Autenticação: emissão de JWT
app.MapPost("/api/v1/auth/token", (LoginRequest login) =>
{
    if (login.Usuario == "admin" && login.Senha == "admin123")
    {
        string token = JwtHelper.GerarToken(login.Usuario, "Admin", chaveSecreta);
        return Results.Ok(new LoginResponse(token, "Bearer", 7200));
    }
    if (login.Usuario == "aluno" && login.Senha == "aluno123")
    {
        string token = JwtHelper.GerarToken(login.Usuario, "Aluno", chaveSecreta);
        return Results.Ok(new LoginResponse(token, "Bearer", 7200));
    }

    return Results.Unauthorized();
});

// Grupo de Cursos
var cursosGroup = app.MapGroup("/api/v1/cursos").WithTags("Cursos");

cursosGroup.MapGet("/", async (ICursoRepository repo, CancellationToken ct) =>
{
    var cursos = await repo.ListarTodosAsync(ct);
    return Results.Ok(cursos);
});

cursosGroup.MapGet("/{id:int}", async (int id, ICursoRepository repo, CancellationToken ct) =>
{
    var curso = await repo.ObterPorIdAsync(id, ct);
    return curso is not null ? Results.Ok(curso) : Results.NotFound();
});

cursosGroup.MapPost("/", async (Curso cursoDto, ICursoRepository repo, CancellationToken ct) =>
{
    await repo.AdicionarAsync(cursoDto, ct);
    return Results.Created($"/api/v1/cursos/{cursoDto.Id}", cursoDto);
}).RequireAuthorization("ApenasAdmin");

// Grupo de Matrículas
var matriculasGroup = app.MapGroup("/api/v1/matriculas").WithTags("Matriculas");

matriculasGroup.MapPost("/", async (CriarMatriculaDto dto, IMatriculaService service, CancellationToken ct) =>
{
    try
    {
        var matricula = await service.MatricularAlunoAsync(dto.AlunoId, dto.CursoId, ct);
        return Results.Created($"/api/v1/matriculas/{matricula.Id}", matricula);
    }
    catch (EntidadeNaoEncontradaException ex)
    {
        return Results.NotFound(new { erro = ex.Message });
    }
    catch (DomainException ex)
    {
        return Results.Conflict(new { erro = ex.Message });
    }
});

matriculasGroup.MapDelete("/{id:int}", async (int id, IMatriculaService service, CancellationToken ct) =>
{
    try
    {
        var cancelada = await service.CancelarMatriculaAsync(id, ct);
        return Results.Ok(cancelada);
    }
    catch (EntidadeNaoEncontradaException ex)
    {
        return Results.NotFound(new { erro = ex.Message });
    }
    catch (DomainException ex)
    {
        return Results.Conflict(new { erro = ex.Message });
    }
});

matriculasGroup.MapGet("/aluno/{alunoId:int}", async (int alunoId, IMatriculaService service, CancellationToken ct) =>
{
    try
    {
        var matriculas = await service.ListarMatriculasAlunoAsync(alunoId, ct);
        return Results.Ok(matriculas);
    }
    catch (EntidadeNaoEncontradaException ex)
    {
        return Results.NotFound(new { erro = ex.Message });
    }
});

app.Run();

public record CriarMatriculaDto(int AlunoId, int CursoId);

// Necessário para o WebApplicationFactory nos testes de integração
public partial class Program { }
