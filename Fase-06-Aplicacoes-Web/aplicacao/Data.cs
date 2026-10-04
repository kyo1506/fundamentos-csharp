using Microsoft.EntityFrameworkCore;
using Fase05.GestaoAcademica.Core;

namespace Fase06.GestaoAcademica.WebApi;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Aluno>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Nome).IsRequired().HasMaxLength(120);
            b.Property(a => a.Email).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<Curso>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Titulo).IsRequired().HasMaxLength(150);
            b.Property(c => c.PrecoBase).HasPrecision(10, 2);
        });

        modelBuilder.Entity<Matricula>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.ValorCobrado).HasPrecision(10, 2);
        });
    }
}

public class EfAlunoRepository : IAlunoRepository
{
    private readonly AppDbContext _db;
    public EfAlunoRepository(AppDbContext db) => _db = db;

    public async Task<Aluno?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await _db.Alunos.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Aluno>> ListarTodosAsync(CancellationToken ct = default) =>
        await _db.Alunos.AsNoTracking().ToListAsync(ct);

    public async Task AdicionarAsync(Aluno entidade, CancellationToken ct = default)
    {
        await _db.Alunos.AddAsync(entidade, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Aluno entidade, CancellationToken ct = default)
    {
        _db.Alunos.Update(entidade);
        await _db.SaveChangesAsync(ct);
    }
}

public class EfCursoRepository : ICursoRepository
{
    private readonly AppDbContext _db;
    public EfCursoRepository(AppDbContext db) => _db = db;

    public async Task<Curso?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await _db.Cursos.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Curso>> ListarTodosAsync(CancellationToken ct = default) =>
        await _db.Cursos.AsNoTracking().ToListAsync(ct);

    public async Task AdicionarAsync(Curso entidade, CancellationToken ct = default)
    {
        await _db.Cursos.AddAsync(entidade, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Curso entidade, CancellationToken ct = default)
    {
        _db.Cursos.Update(entidade);
        await _db.SaveChangesAsync(ct);
    }
}

public class EfMatriculaRepository : IMatriculaRepository
{
    private readonly AppDbContext _db;
    public EfMatriculaRepository(AppDbContext db) => _db = db;

    public async Task<Matricula?> ObterPorIdAsync(int id, CancellationToken ct = default) =>
        await _db.Matriculas.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IReadOnlyList<Matricula>> ListarTodosAsync(CancellationToken ct = default) =>
        await _db.Matriculas.AsNoTracking().ToListAsync(ct);

    public async Task AdicionarAsync(Matricula entidade, CancellationToken ct = default)
    {
        int id = entidade.Id;
        while (await _db.Matriculas.AnyAsync(m => m.Id == id, ct))
        {
            id++;
        }
        var matriculaParaSalvar = entidade.Id == id ? entidade : entidade with { Id = id };
        await _db.Matriculas.AddAsync(matriculaParaSalvar, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AtualizarAsync(Matricula entidade, CancellationToken ct = default)
    {
        _db.Matriculas.Update(entidade);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Matricula>> ObterPorAlunoIdAsync(int alunoId, CancellationToken ct = default) =>
        await _db.Matriculas.AsNoTracking().Where(m => m.AlunoId == alunoId).ToListAsync(ct);

    public async Task<bool> ExisteMatriculaAtivaAsync(int alunoId, int cursoId, CancellationToken ct = default) =>
        await _db.Matriculas.AnyAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId && m.Status != StatusMatricula.Cancelada, ct);
}
