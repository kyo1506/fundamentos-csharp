namespace Semana01.ControleEstoque;

// ============================================
// TIPOS POR VALOR: struct e enum
// ============================================

/// <summary>
/// Representa um valor monetário imutável.
/// Struct é ideal aqui: pequeno, sem identidade própria, valor padrão faz sentido.
/// </summary>
public readonly struct Dinheiro
{
    public decimal Valor { get; }
    public string Moeda { get; }

    public Dinheiro(decimal valor, string moeda = "BRL")
    {
        if (valor < 0)
            throw new ArgumentException("Valor não pode ser negativo", nameof(valor));
        
        Valor = valor;
        Moeda = moeda.ToUpper() switch
        {
            "BRL" or "USD" or "EUR" => moeda.ToUpper(),
            _ => throw new ArgumentException("Moeda não suportada. Use BRL, USD ou EUR.")
        };
    }

    public override string ToString() => $"{Moeda} {Valor:N2}";

    // Operadores para facilitar cálculos
    public static Dinheiro operator +(Dinheiro a, Dinheiro b)
    {
        if (a.Moeda != b.Moeda)
            throw new InvalidOperationException("Não é possível somar moedas diferentes");
        return new Dinheiro(a.Valor + b.Valor, a.Moeda);
    }

    public static Dinheiro operator *(Dinheiro dinheiro, int quantidade)
    {
        return new Dinheiro(dinheiro.Valor * quantidade, dinheiro.Moeda);
    }
}

/// <summary>
/// Unidades de medida para produtos do mercadinho.
/// </summary>
public enum UnidadeMedida
{
    Unidade,
    Kilograma,
    Litro
}

/// <summary>
/// Produto perecível do mercadinho.
/// Struct porque é um dado pequeno, sem identidade, que será copiado frequentemente.
/// </summary>
public struct ProdutoPerecivel
{
    public string Nome { get; set; }
    public string CodigoBarras { get; set; }
    public Dinheiro Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public UnidadeMedida Unidade { get; set; }
    public DateTime DataValidade { get; set; }

    public bool EstaVencido => DateTime.Now > DataValidade;

    public override string ToString() =>
        $"{Nome} ({CodigoBarras}) - {Preco} - Estoque: {QuantidadeEstoque} {Unidade}" +
        $"{(EstaVencido ? " [VENCIDO!]" : "")}";
}

// ============================================
// TIPO POR REFERÊNCIA: class
// ============================================

/// <summary>
/// Representa uma transação no estoque (entrada ou saída).
/// Class porque tem identidade e será compartilhada no histórico.
/// </summary>
public class Transacao
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Data { get; } = DateTime.Now;
    public string Tipo { get; } // "Entrada" ou "Saída"
    public int Quantidade { get; }
    public ProdutoPerecivel Produto { get; }
    public string Observacao { get; }

    public Transacao(string tipo, int quantidade, ProdutoPerecivel produto, string observacao = "")
    {
        if (tipo != "Entrada" && tipo != "Saída")
            throw new ArgumentException("Tipo deve ser 'Entrada' ou 'Saída'");
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva");

        Tipo = tipo;
        Quantidade = quantidade;
        Produto = produto;
        Observacao = observacao;
    }

    public override string ToString() =>
        $"[{Data:dd/MM/yyyy HH:mm}] {Tipo}: {Quantidade}x {Produto.Nome}" +
        $"{(string.IsNullOrEmpty(Observacao) ? "" : $" ({Observacao})")}";
}

/// <summary>
/// Estoque do mercadinho.
/// Class porque é uma entidade complexa com identidade e estado mutável.
/// </summary>
public class Estoque
{
    public string Nome { get; }
    private readonly List<ProdutoPerecivel> _produtos = new();
    private readonly List<Transacao> _historico = new();

    public Estoque(string nome)
    {
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
    }

    public IReadOnlyList<ProdutoPerecivel> Produtos => _produtos.AsReadOnly();
    public IReadOnlyList<Transacao> Historico => _historico.AsReadOnly();

    public void AdicionarProduto(ProdutoPerecivel produto)
    {
        var existente = _produtos.FindIndex(p => p.CodigoBarras == produto.CodigoBarras);
        if (existente >= 0)
        {
            var atual = _produtos[existente];
            atual.QuantidadeEstoque += produto.QuantidadeEstoque;
            _produtos[existente] = atual;
        }
        else
        {
            _produtos.Add(produto);
        }
    }

    public ProdutoPerecivel? BuscarPorCodigo(string codigoBarras)
    {
        for (int i = 0; i < _produtos.Count; i++)
        {
            if (_produtos[i].CodigoBarras == codigoBarras)
                return _produtos[i];
        }
        return null;
    }

    public bool RegistrarEntrada(string codigoBarras, int quantidade, string observacao = "")
    {
        var produto = BuscarPorCodigo(codigoBarras);
        if (produto == null) return false;

        var atualizada = produto.Value;
        atualizada.QuantidadeEstoque += quantidade;
        
        var index = _produtos.FindIndex(p => p.CodigoBarras == codigoBarras);
        _produtos[index] = atualizada;
        
        _historico.Add(new Transacao("Entrada", quantidade, atualizada, observacao));
        return true;
    }

    public bool RegistrarSaida(string codigoBarras, int quantidade, string observacao = "")
    {
        var produto = BuscarPorCodigo(codigoBarras);
        if (produto == null || produto.Value.QuantidadeEstoque < quantidade)
            return false;

        var atualizada = produto.Value;
        atualizada.QuantidadeEstoque -= quantidade;
        
        var index = _produtos.FindIndex(p => p.CodigoBarras == codigoBarras);
        _produtos[index] = atualizada;
        
        _historico.Add(new Transacao("Saída", quantidade, atualizada, observacao));
        return true;
    }

    /// <summary>
    /// Ajusta preços de múltiplos produtos usando ref (modifica diretamente).
    /// </summary>
    public void AjustarPrecos(ref ProdutoPerecivel[] produtos, decimal percentual)
    {
        for (int i = 0; i < produtos.Length; i++)
        {
            var precoAtual = produtos[i].Preco;
            var novoPreco = new Dinheiro(precoAtual.Valor * (1 + percentual / 100), precoAtual.Moeda);
            produtos[i].Preco = novoPreco;
        }
    }

    /// <summary>
    /// Retorna um relatório com valores antes e depois do ajuste usando out.
    /// </summary>
    public bool TentarAjustarPrecosComRelatorio(
        ref ProdutoPerecivel[] produtos, 
        decimal percentual,
        out string relatorio)
    {
        if (produtos == null || produtos.Length == 0)
        {
            relatorio = "Nenhum produto para ajustar.";
            return false;
        }

        var antes = produtos.Select(p => p.Preco.Valor).ToArray();
        AjustarPrecos(ref produtos, percentual);
        var depois = produtos.Select(p => p.Preco.Valor).ToArray();

        relatorio = "Relatório de Ajuste de Preços:\n";
        for (int i = 0; i < produtos.Length; i++)
        {
            relatorio += $"  {produtos[i].Nome}: {antes[i]:C} → {depois[i]:C}\n";
        }
        return true;
    }

    public void ExibirRelatorio()
    {
        Console.WriteLine($"\n=== Estoque: {Nome} ===");
        Console.WriteLine($"Total de produtos: {_produtos.Count}");
        Console.WriteLine($"Total de transações: {_historico.Count}");
        Console.WriteLine("\nProdutos:");
        foreach (var p in _produtos)
        {
            Console.WriteLine($"  {p}");
        }
        Console.WriteLine("\nHistórico:");
        foreach (var t in _historico)
        {
            Console.WriteLine($"  {t}");
        }
    }
}
