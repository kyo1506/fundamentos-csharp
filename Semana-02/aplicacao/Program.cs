using Semana02.ValidadorFormulario;

Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
Console.WriteLine("║  VALIDAÇÃO DE FORMULÁRIOS — GESTÃO DE INSCRIÇÕES       ║");
Console.WriteLine("║  Demonstra: OO + SOLID (SRP, OCP, LSP, ISP, DIP)        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════╝");

// ============================================
// Configuração (DIP: injeção de dependências)
// ============================================
var repositorio = new RepositorioMemoria<Participante>();
var emailService = new EmailNotificacaoService();
var relatorioService = new RelatorioParticipanteService();

var servico = new ServicoInscricao(repositorio, emailService, relatorioService);

// ============================================
// PARTE 1: Inscrições válidas
// ============================================
Console.WriteLine("\n━━━ PARTE 1: Inscrições Válidas ━━━\n");

var participantes = new[]
{
    new Participante("Maria Silva", "529.982.247-25", "maria@email.com", 25, TipoParticipante.Estudante),
    new Participante("João Santos", "111.444.777-35", "joao@email.com", 35, TipoParticipante.Profissional),
    new Participante("Ana Costa", "222.333.444-55", "ana@email.com", 65, TipoParticipante.Idoso),
    new Participante("Carlos Lima", "333.444.555-66", "carlos@email.com", 40, TipoParticipante.Vip)
};

foreach (var p in participantes)
{
    Console.WriteLine($"Inscrevendo: {p.Nome}...");
    servico.Inscrever(p);
    Console.WriteLine();
}

// ============================================
// PARTE 2: Validação com erros
// ============================================
Console.WriteLine("\n━━━ PARTE 2: Validação com Erros ━━━\n");

try
    {
        var invalido = new Participante("", "111.111.111-11", "invalido", -5, TipoParticipante.Profissional);
        servico.Inscrever(invalido);
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Erro capturado: {ex.Message}");
    }

// ============================================
// PARTE 3: Cálculo de desconto (OCP)
// ============================================
Console.WriteLine("\n━━━ PARTE 3: Cálculo de Desconto (OCP) ━━━\n");

var calculadora = new CalculadoraPrecoFinal();
calculadora.AdicionarDesconto(new DescontoEstudante());
calculadora.AdicionarDesconto(new DescontoIdoso());
calculadora.AdicionarDesconto(new DescontoVip());

decimal precoBase = 100.00m;

foreach (var p in participantes)
{
    decimal precoFinal = calculadora.Calcular(p, precoBase);
    Console.WriteLine($"{p.Nome} ({p.Tipo}): {precoBase:C} → {precoFinal:C}");
}

// ============================================
// PARTE 4: Relatório
// ============================================
Console.WriteLine("\n━━━ PARTE 4: Relatório ━━━\n");
Console.WriteLine(servico.GerarRelatorio());

// ============================================
// PARTE 5: Notificação por SMS (DIP)
// ============================================
Console.WriteLine("\n━━━ PARTE 5: Troca de Notificação (DIP) ━━━\n");

var smsService = new SmsNotificacaoService();
var servicoSms = new ServicoInscricao(repositorio, smsService, relatorioService);

var novoParticipante = new Participante("Pedro Alves", "444.555.666-77", "pedro@email.com", 28, TipoParticipante.Profissional);
servicoSms.Inscrever(novoParticipante);

Console.WriteLine("\nPressione qualquer tecla para sair...");
Console.ReadKey();
