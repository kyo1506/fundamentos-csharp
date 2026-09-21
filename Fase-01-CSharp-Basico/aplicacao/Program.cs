using System.Globalization;
using Fase01.CSharpBasico;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

Console.WriteLine("==========================================================");
Console.WriteLine("  Fase 01 — C# Básico: a linguagem em profundidade");
Console.WriteLine("==========================================================");
Console.WriteLine();

// 1. Dinheiro e struct
var d1 = new Dinheiro(150.50m, "BRL");
var d2 = new Dinheiro(49.50m, "BRL");
var soma = d1 + d2;
Console.WriteLine($"[01.1 & 01.6] Struct Dinheiro: {d1} + {d2} = {soma}");

// 2. Inverter string com StringBuilder
string original = "Engenharia de Software no .NET 10";
string invertido = Basico.InverterTexto(original);
Console.WriteLine($"[01.2] StringBuilder Inverter: \"{original}\" -> \"{invertido}\"");

// 3. Arredondamento e parcelas
decimal conta = 100.00m;
var parcelas = Basico.DividirParcelas(conta, 3);
Console.WriteLine($"[01.3] Divisão exata de R$ 100 em 3 parcelas: {string.Join(", ", parcelas.Select(p => p.ToString("C")))}");

// 4. Coleções: Contagem de palavras e Validação de delimitadores
string frase = "C# é moderno. O .NET é rápido e C# é produtivo!";
var freq = Basico.ContarFrequenciaPalavras(frase);
Console.WriteLine($"[01.4] Frequência de 'C#': {freq.GetValueOrDefault("C#", 0)}x, 'é': {freq.GetValueOrDefault("é", 0)}x");
bool delimitadoresOk = Basico.ValidarDelimitadores("{[()]}");
Console.WriteLine($"[01.4] Delimitadores '{{[()]}}' balanceados: {delimitadoresOk}");

// 5. Pattern Matching e Record
var pedido = new Pedido(101, 1250.00m, "BR", ClienteVip: true, QuantidadeItens: 6);
decimal desc = Basico.CalcularPercentualDesconto(pedido);
Console.WriteLine($"[01.6 & 01.7] Pedido VIP {pedido.ValorTotal:C}: Desconto = {desc:P0}");

// 6. Métodos de extensão
string textoLongo = "Aprender C# em profundidade transforma sua carreira de programador.";
Console.WriteLine($"[01.9] Método de extensão Truncar: {textoLongo.Truncar(25)}");

// 7. DateOnly e dias úteis
var hoje = DateOnly.FromDateTime(DateTime.Today);
var daquiADezDias = hoje.AddDays(10);
int diasUteis = Basico.CalcularDiasUteis(hoje, daquiADezDias);
Console.WriteLine($"[01.10] Dias úteis entre {hoje} e {daquiADezDias}: {diasUteis} dias");

Console.WriteLine();
Console.WriteLine("Todos os exemplos foram executados com sucesso!");
