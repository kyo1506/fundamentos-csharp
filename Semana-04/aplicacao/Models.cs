// ============================================
// API CONTROLE DE ESTOQUE — Semana 4
// Minimal API com ASP.NET Core
// ============================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace Semana04.APIControleEstoque;

// ============================================
// MODELOS
// ============================================

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int QuantidadeEstoque { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}

public record ProdutoRequest(
    string Nome,
    string Categoria,
    decimal Preco,
    int QuantidadeEstoque
);

public record ApiResponse<T>(
    bool Sucesso,
    string Mensagem,
    T? Dado
)
{
    public static ApiResponse<T> Ok(T? dado, string mensagem = "Sucesso") =>
        new(true, mensagem, dado);
    
    public static ApiResponse<T> Erro(string mensagem) =>
        new(false, mensagem, default);
}

// ============================================
// REPOSITÓRIO (Abstração + Persistência JSON)
// ============================================

public interface IRepositorioProduto
{
    IEnumerable<Produto> Listar();
    Produto? Buscar(int id);
    Produto Criar(ProdutoRequest request);
    Produto? Atualizar(int id, ProdutoRequest request);
    bool Remover(int id);
}

public class RepositorioProdutoJson : IRepositorioProduto
{
    private readonly string _caminhoArquivo;
    private List<Produto> _produtos = new();
    private int _proximoId = 1;

    public RepositorioProdutoJson(string caminhoArquivo = "produtos.json")
    {
        _caminhoArquivo = caminhoArquivo;
        Carregar();
    }

    private void Carregar()
    {
        if (File.Exists(_caminhoArquivo))
        {
            try
            {
                string json = File.ReadAllText(_caminhoArquivo);
                _produtos = JsonSerializer.Deserialize<List<Produto>>(json) ?? new();
                if (_produtos.Any())
                    _proximoId = _produtos.Max(p => p.Id) + 1;
            }
            catch (JsonException)
            {
                // Arquivo corrompido — começar com lista vazia
                _produtos = new();
            }
        }
    }

    private void Salvar()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(_produtos, options);
        File.WriteAllText(_caminhoArquivo, json);
    }

    public IEnumerable<Produto> Listar() => _produtos.AsReadOnly();

    public Produto? Buscar(int id) => _produtos.FirstOrDefault(p => p.Id == id);

    public Produto Criar(ProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = _proximoId++,
            Nome = request.Nome,
            Categoria = request.Categoria,
            Preco = request.Preco,
            QuantidadeEstoque = request.QuantidadeEstoque
        };
        
        _produtos.Add(produto);
        Salvar();
        return produto;
    }

    public Produto? Atualizar(int id, ProdutoRequest request)
    {
        var produto = Buscar(id);
        if (produto == null) return null;

        produto.Nome = request.Nome;
        produto.Categoria = request.Categoria;
        produto.Preco = request.Preco;
        produto.QuantidadeEstoque = request.QuantidadeEstoque;
        
        Salvar();
        return produto;
    }

    public bool Remover(int id)
    {
        var produto = Buscar(id);
        if (produto == null) return false;
        
        _produtos.Remove(produto);
        Salvar();
        return true;
    }
}

public class RepositorioProdutoMemoria : IRepositorioProduto
{
    private readonly List<Produto> _produtos = new();
    private int _proximoId = 1;

    public IEnumerable<Produto> Listar() => _produtos.AsReadOnly();
    public Produto? Buscar(int id) => _produtos.FirstOrDefault(p => p.Id == id);

    public Produto Criar(ProdutoRequest request)
    {
        var produto = new Produto
        {
            Id = _proximoId++,
            Nome = request.Nome,
            Categoria = request.Categoria,
            Preco = request.Preco,
            QuantidadeEstoque = request.QuantidadeEstoque
        };
        _produtos.Add(produto);
        return produto;
    }

    public Produto? Atualizar(int id, ProdutoRequest request)
    {
        var existente = Buscar(id);
        if (existente == null) return null;

        existente.Nome = request.Nome;
        existente.Categoria = request.Categoria;
        existente.Preco = request.Preco;
        existente.QuantidadeEstoque = request.QuantidadeEstoque;
        
        return existente;
    }

    public bool Remover(int id) => _produtos.RemoveAll(p => p.Id == id) > 0;
}

// ============================================
// SERVIÇO DE PRODUTOS
// ============================================

public interface IProdutoService
{
    IEnumerable<Produto> Listar();
    Produto? Buscar(int id);
    (bool Sucesso, Produto? Produto, string Erro) Criar(ProdutoRequest request);
    (bool Sucesso, Produto? Produto, string Erro) Atualizar(int id, ProdutoRequest request);
    bool Remover(int id);
}

public class ProdutoService : IProdutoService
{
    private readonly IRepositorioProduto _repositorio;

    public ProdutoService(IRepositorioProduto repositorio)
    {
        _repositorio = repositorio;
    }

    public IEnumerable<Produto> Listar() => _repositorio.Listar();

    public Produto? Buscar(int id) => _repositorio.Buscar(id);

    public (bool Sucesso, Produto? Produto, string Erro) Criar(ProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return (false, null, "Nome é obrigatório");
        
        if (request.Preco <= 0)
            return (false, null, "Preço deve ser maior que zero");
        
        if (request.QuantidadeEstoque < 0)
            return (false, null, "Quantidade não pode ser negativa");

        var produto = _repositorio.Criar(request);
        return (true, produto, string.Empty);
    }

    public (bool Sucesso, Produto? Produto, string Erro) Atualizar(int id, ProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return (false, null, "Nome é obrigatório");
        
        if (request.Preco <= 0)
            return (false, null, "Preço deve ser maior que zero");

        var produto = _repositorio.Atualizar(id, request);
        return produto != null 
            ? (true, produto, string.Empty) 
            : (false, null, "Produto não encontrado");
    }

    public bool Remover(int id) => _repositorio.Remover(id);
}

// ============================================
// MIDDLEWARE DE LOG
// ============================================

public static class LogMiddleware
{
    public static void LogRequest(string method, string path, TimeSpan duration, int statusCode)
    {
        string cor = statusCode switch
        {
            >= 200 and < 300 => "\x1b[32m", // Verde
            >= 400 and < 500 => "\x1b[33m", // Amarelo
            >= 500 => "\x1b[31m",             // Vermelho
            _ => "\x1b[0m"
        };
        
        Console.WriteLine($"{cor}[{DateTime.Now:HH:mm:ss}] {method} {path} → {statusCode} ({duration.TotalMilliseconds:F0}ms)\x1b[0m");
    }
}

// ============================================
// "SIMULADOR" DE API (Console App)
// Em produção, usar ASP.NET Core Minimal API real
// ============================================

public class ApiSimulator
{
    private readonly IProdutoService _service;

    public ApiSimulator(IProdutoService service)
    {
        _service = service;
    }

    // GET /produtos
    public ApiResponse<IEnumerable<Produto>> ListarProdutos()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var produtos = _service.Listar();
        sw.Stop();
        LogMiddleware.LogRequest("GET", "/produtos", sw.Elapsed, 200);
        return ApiResponse<IEnumerable<Produto>>.Ok(produtos);
    }

    // GET /produtos/{id}
    public ApiResponse<Produto> BuscarProduto(int id)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var produto = _service.Buscar(id);
        sw.Stop();
        
        if (produto == null)
        {
            LogMiddleware.LogRequest("GET", $"/produtos/{id}", sw.Elapsed, 404);
            return ApiResponse<Produto>.Erro("Produto não encontrado");
        }
        
        LogMiddleware.LogRequest("GET", $"/produtos/{id}", sw.Elapsed, 200);
        return ApiResponse<Produto>.Ok(produto);
    }

    // POST /produtos
    public ApiResponse<Produto> CriarProduto(ProdutoRequest request)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var (sucesso, produto, erro) = _service.Criar(request);
        sw.Stop();

        if (!sucesso)
        {
            LogMiddleware.LogRequest("POST", "/produtos", sw.Elapsed, 400);
            return ApiResponse<Produto>.Erro(erro);
        }

        LogMiddleware.LogRequest("POST", "/produtos", sw.Elapsed, 201);
        return ApiResponse<Produto>.Ok(produto!, "Produto criado com sucesso");
    }

    // PUT /produtos/{id}
    public ApiResponse<Produto> AtualizarProduto(int id, ProdutoRequest request)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var (sucesso, produto, erro) = _service.Atualizar(id, request);
        sw.Stop();

        if (!sucesso)
        {
            var statusCode = erro.Contains("não encontrado") ? 404 : 400;
            LogMiddleware.LogRequest("PUT", $"/produtos/{id}", sw.Elapsed, statusCode);
            return ApiResponse<Produto>.Erro(erro);
        }

        LogMiddleware.LogRequest("PUT", $"/produtos/{id}", sw.Elapsed, 200);
        return ApiResponse<Produto>.Ok(produto!, "Produto atualizado");
    }

    // DELETE /produtos/{id}
    public ApiResponse<object> RemoverProduto(int id)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        bool removido = _service.Remover(id);
        sw.Stop();

        if (!removido)
        {
            LogMiddleware.LogRequest("DELETE", $"/produtos/{id}", sw.Elapsed, 404);
            return ApiResponse<object>.Erro("Produto não encontrado");
        }

        LogMiddleware.LogRequest("DELETE", $"/produtos/{id}", sw.Elapsed, 204);
        return ApiResponse<object>.Ok(null, "Produto removido");
    }

    // GET /produtos/categoria/{categoria}
    public ApiResponse<IEnumerable<Produto>> FiltrarPorCategoria(string categoria)
    {
        var produtos = _service.Listar()
            .Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));
        return ApiResponse<IEnumerable<Produto>>.Ok(produtos);
    }

    // GET /stats
    public ApiResponse<object> Estatisticas()
    {
        var produtos = _service.Listar().ToList();
        var stats = new
        {
            TotalProdutos = produtos.Count,
            ValorTotalEstoque = produtos.Sum(p => p.Preco * p.QuantidadeEstoque),
            MediaPreco = produtos.Any() ? produtos.Average(p => p.Preco) : 0,
            Categorias = produtos.Select(p => p.Categoria).Distinct().Count()
        };
        return ApiResponse<object>.Ok(stats);
    }
}
