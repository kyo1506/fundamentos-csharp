namespace Fase08.EventBus.Distribuido.Core.Sagas;

public sealed class MatriculaSagaContexto
{
    public Guid SagaId { get; init; } = Guid.NewGuid();
    public int AlunoId { get; init; }
    public int CursoId { get; init; }
    public decimal Valor { get; init; }
    public EstadoSaga Estado { get; set; } = EstadoSaga.Iniciada;
    public bool VagaReservada { get; set; }
    public bool MatriculaAtiva { get; set; }
    public string? MotivoFalha { get; set; }
    public List<string> HistoricoEtapas { get; } = new();

    public void RegistrarEtapa(string descricao)
    {
        HistoricoEtapas.Add($"[{DateTime.UtcNow:HH:mm:ss}] {descricao}");
    }
}
