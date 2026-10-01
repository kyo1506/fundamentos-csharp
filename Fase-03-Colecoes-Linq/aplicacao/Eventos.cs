using Fase02.PooSolid;

namespace Fase03.CourseAnalytics;

public class MatriculaRegistradaEventArgs(Matricula matricula) : EventArgs
{
    public Matricula Matricula { get; } = matricula ?? throw new ArgumentNullException(nameof(matricula));
    public DateTime RegistradaEm { get; } = DateTime.UtcNow;
}

public class AlertaTurmaLotadaEventArgs(string nomeCurso, int capacidade) : EventArgs
{
    public string NomeCurso { get; } = nomeCurso;
    public int Capacidade { get; } = capacidade;
}

public class AnalyticsEventHub
{
    public event EventHandler<MatriculaRegistradaEventArgs>? AoRegistrarMatricula;
    public event EventHandler<AlertaTurmaLotadaEventArgs>? AoLotarTurma;

    public void PublicarMatricula(Matricula matricula)
    {
        AoRegistrarMatricula?.Invoke(this, new MatriculaRegistradaEventArgs(matricula));
    }

    public void PublicarAlertaLotacao(string curso, int capacidade)
    {
        AoLotarTurma?.Invoke(this, new AlertaTurmaLotadaEventArgs(curso, capacidade));
    }
}
