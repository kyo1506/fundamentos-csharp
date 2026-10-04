namespace Fase07.AltaPerformance.Core.Modelos;

public sealed record RegistroEvento(
    Guid Id,
    DateTimeOffset Timestamp,
    NivelLog Nivel,
    string Servico,
    string Mensagem,
    int CodigoHttp,
    double DuracaoMs);
