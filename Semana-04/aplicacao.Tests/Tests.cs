using Semana04.APIControleEstoque;

namespace Semana04.Tests;

public class ProdutoServiceTests
{
    private readonly IRepositorioProduto _repositorio;
    private readonly IProdutoService _service;

    public ProdutoServiceTests()
    {
        _repositorio = new RepositorioProdutoMemoria();
        _service = new ProdutoService(_repositorio);
    }

    [Fact]
    public void Service_CriarProdutoValido_DeveRetornarSucesso()
    {
        var request = new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100);
        
        var (sucesso, produto, erro) = _service.Criar(request);
        
        Assert.True(sucesso);
        Assert.NotNull(produto);
        Assert.Equal("Arroz", produto!.Nome);
        Assert.Equal(1, produto.Id);
    }

    [Fact]
    public void Service_CriarProdutoSemNome_DeveRetornarErro()
    {
        var request = new ProdutoRequest("", "Alimentos", 15.90m, 100);
        
        var (sucesso, produto, erro) = _service.Criar(request);
        
        Assert.False(sucesso);
        Assert.Null(produto);
        Assert.Contains("Nome", erro);
    }

    [Fact]
    public void Service_CriarProdutoPrecoNegativo_DeveRetornarErro()
    {
        var request = new ProdutoRequest("Arroz", "Alimentos", -10m, 100);
        
        var (sucesso, produto, erro) = _service.Criar(request);
        
        Assert.False(sucesso);
        Assert.Contains("Preço", erro);
    }

    [Fact]
    public void Service_CriarProdutoQuantidadeNegativa_DeveRetornarErro()
    {
        var request = new ProdutoRequest("Arroz", "Alimentos", 15.90m, -5);
        
        var (sucesso, produto, erro) = _service.Criar(request);
        
        Assert.False(sucesso);
        Assert.Contains("Quantidade", erro);
    }

    [Fact]
    public void Service_BuscarProdutoExistente_DeveRetornarProduto()
    {
        var request = new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100);
        _service.Criar(request);
        
        var produto = _service.Buscar(1);
        
        Assert.NotNull(produto);
        Assert.Equal("Arroz", produto!.Nome);
    }

    [Fact]
    public void Service_BuscarProdutoInexistente_DeveRetornarNull()
    {
        var produto = _service.Buscar(999);
        
        Assert.Null(produto);
    }

    [Fact]
    public void Service_AtualizarProdutoExistente_DeveRetornarSucesso()
    {
        _service.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var (sucesso, produto, erro) = _service.Atualizar(1, new ProdutoRequest("Arroz Integral", "Alimentos", 17.90m, 120));
        
        Assert.True(sucesso);
        Assert.NotNull(produto);
        Assert.Equal("Arroz Integral", produto!.Nome);
        Assert.Equal(17.90m, produto.Preco);
    }

    [Fact]
    public void Service_AtualizarProdutoInexistente_DeveRetornarErro()
    {
        var (sucesso, produto, erro) = _service.Atualizar(999, new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        Assert.False(sucesso);
        Assert.Contains("não encontrado", erro);
    }

    [Fact]
    public void Service_RemoverProdutoExistente_DeveRetornarTrue()
    {
        _service.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        bool removido = _service.Remover(1);
        
        Assert.True(removido);
        Assert.Null(_service.Buscar(1));
    }

    [Fact]
    public void Service_RemoverProdutoInexistente_DeveRetornarFalse()
    {
        bool removido = _service.Remover(999);
        
        Assert.False(removido);
    }

    [Fact]
    public void Service_Listar_DeveRetornarTodosProdutos()
    {
        _service.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        _service.Criar(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        _service.Criar(new ProdutoRequest("Notebook", "Eletrônicos", 3500m, 10));
        
        var produtos = _service.Listar().ToList();
        
        Assert.Equal(3, produtos.Count);
    }
}

public class RepositorioProdutoMemoriaTests
{
    [Fact]
    public void Repositorio_Criar_DeveAtribuirIdSequencial()
    {
        var repo = new RepositorioProdutoMemoria();
        
        var p1 = repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        var p2 = repo.Criar(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        
        Assert.Equal(1, p1.Id);
        Assert.Equal(2, p2.Id);
    }

    [Fact]
    public void Repositorio_Buscar_DeveRetornarProdutoCorreto()
    {
        var repo = new RepositorioProdutoMemoria();
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var produto = repo.Buscar(1);
        
        Assert.NotNull(produto);
        Assert.Equal("Arroz", produto!.Nome);
    }

    [Fact]
    public void Repositorio_Atualizar_DeveModificarProduto()
    {
        var repo = new RepositorioProdutoMemoria();
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var atualizado = repo.Atualizar(1, new ProdutoRequest("Arroz Integral", "Alimentos", 17.90m, 120));
        
        Assert.NotNull(atualizado);
        Assert.Equal("Arroz Integral", atualizado!.Nome);
        Assert.Equal(17.90m, atualizado.Preco);
    }

    [Fact]
    public void Repositorio_Remover_DeveExcluirProduto()
    {
        var repo = new RepositorioProdutoMemoria();
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        bool removido = repo.Remover(1);
        
        Assert.True(removido);
        Assert.Null(repo.Buscar(1));
    }

    [Fact]
    public void Repositorio_Listar_DeveRetornarTodos()
    {
        var repo = new RepositorioProdutoMemoria();
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        repo.Criar(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        
        var lista = repo.Listar().ToList();
        
        Assert.Equal(2, lista.Count);
    }
}

public class ApiSimulatorTests
{
    private readonly ApiSimulator _api;

    public ApiSimulatorTests()
    {
        IRepositorioProduto repositorio = new RepositorioProdutoMemoria();
        IProdutoService service = new ProdutoService(repositorio);
        _api = new ApiSimulator(service);
    }

    [Fact]
    public void Api_ListarProdutos_DeveRetornarSucesso()
    {
        var resultado = _api.ListarProdutos();
        
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dado);
    }

    [Fact]
    public void Api_CriarProduto_DeveRetornar201()
    {
        var resultado = _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dado);
        Assert.Equal("Arroz", resultado.Dado!.Nome);
    }

    [Fact]
    public void Api_CriarProdutoInvalido_DeveRetornar400()
    {
        var resultado = _api.CriarProduto(new ProdutoRequest("", "Alimentos", -10m, -5));
        
        Assert.False(resultado.Sucesso);
        Assert.NotNull(resultado.Mensagem);
    }

    [Fact]
    public void Api_BuscarProdutoExistente_DeveRetornar200()
    {
        _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var resultado = _api.BuscarProduto(1);
        
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dado);
    }

    [Fact]
    public void Api_BuscarProdutoInexistente_DeveRetornar404()
    {
        var resultado = _api.BuscarProduto(999);
        
        Assert.False(resultado.Sucesso);
        Assert.Equal("Produto não encontrado", resultado.Mensagem);
    }

    [Fact]
    public void Api_AtualizarProduto_DeveRetornar200()
    {
        _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var resultado = _api.AtualizarProduto(1, new ProdutoRequest("Arroz Integral", "Alimentos", 17.90m, 120));
        
        Assert.True(resultado.Sucesso);
        Assert.Equal("Arroz Integral", resultado.Dado!.Nome);
    }

    [Fact]
    public void Api_RemoverProduto_DeveRetornarSucesso()
    {
        _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        var resultado = _api.RemoverProduto(1);
        
        Assert.True(resultado.Sucesso);
    }

    [Fact]
    public void Api_RemoverProdutoInexistente_DeveRetornar404()
    {
        var resultado = _api.RemoverProduto(999);
        
        Assert.False(resultado.Sucesso);
        Assert.Equal("Produto não encontrado", resultado.Mensagem);
    }

    [Fact]
    public void Api_FiltrarPorCategoria_DeveRetornarApenasCategoria()
    {
        _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        _api.CriarProduto(new ProdutoRequest("Notebook", "Eletrônicos", 3500m, 10));
        
        var resultado = _api.FiltrarPorCategoria("Alimentos");
        
        Assert.True(resultado.Sucesso);
        Assert.Single(resultado.Dado!);
        Assert.Equal("Arroz", resultado.Dado!.First().Nome);
    }

    [Fact]
    public void Api_Estatisticas_DeveRetornarDados()
    {
        _api.CriarProduto(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        _api.CriarProduto(new ProdutoRequest("Feijão", "Alimentos", 8.50m, 80));
        
        var resultado = _api.Estatisticas();
        
        Assert.True(resultado.Sucesso);
        Assert.NotNull(resultado.Dado);
    }
}

public class ProdutoRequestTests
{
    [Fact]
    public void ProdutoRequest_Criar_DeveArmazenarDados()
    {
        var request = new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100);
        
        Assert.Equal("Arroz", request.Nome);
        Assert.Equal("Alimentos", request.Categoria);
        Assert.Equal(15.90m, request.Preco);
        Assert.Equal(100, request.QuantidadeEstoque);
    }

    [Fact]
    public void ProdutoRequest_Equality_DeveCompararValores()
    {
        var r1 = new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100);
        var r2 = new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100);
        
        Assert.Equal(r1, r2);
    }
}

public class ApiResponseTests
{
    [Fact]
    public void ApiResponse_Ok_DeveRetornarSucesso()
    {
        var response = ApiResponse<string>.Ok("dados", "mensagem");
        
        Assert.True(response.Sucesso);
        Assert.Equal("mensagem", response.Mensagem);
        Assert.Equal("dados", response.Dado);
    }

    [Fact]
    public void ApiResponse_Erro_DeveRetornarFalso()
    {
        var response = ApiResponse<string>.Erro("erro ocorreu");
        
        Assert.False(response.Sucesso);
        Assert.Equal("erro ocorreu", response.Mensagem);
        Assert.Null(response.Dado);
    }
}

public class RepositorioProdutoJsonTests
{
    private readonly string _caminhoTeste = "test_produtos.json";

    public RepositorioProdutoJsonTests()
    {
        // Limpar arquivo de teste se existir
        if (File.Exists(_caminhoTeste))
            File.Delete(_caminhoTeste);
    }

    [Fact]
    public void RepositorioJson_Criar_DeveSalvarEmArquivo()
    {
        var repo = new RepositorioProdutoJson(_caminhoTeste);
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        Assert.True(File.Exists(_caminhoTeste));
        
        string json = File.ReadAllText(_caminhoTeste);
        Assert.Contains("Arroz", json);
    }

    [Fact]
    public void RepositorioJson_Carregar_DeveRecuperarDados()
    {
        // Criar e salvar
        var repo1 = new RepositorioProdutoJson(_caminhoTeste);
        repo1.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        
        // Carregar em nova instância
        var repo2 = new RepositorioProdutoJson(_caminhoTeste);
        var produto = repo2.Buscar(1);
        
        Assert.NotNull(produto);
        Assert.Equal("Arroz", produto!.Nome);
    }

    [Fact]
    public void RepositorioJson_Remover_DeveAtualizarArquivo()
    {
        var repo = new RepositorioProdutoJson(_caminhoTeste);
        repo.Criar(new ProdutoRequest("Arroz", "Alimentos", 15.90m, 100));
        repo.Remover(1);
        
        var repo2 = new RepositorioProdutoJson(_caminhoTeste);
        Assert.Null(repo2.Buscar(1));
    }

    [Fact]
    public void RepositorioJson_ArquivoCorrompido_DeveComecarVazio()
    {
        File.WriteAllText(_caminhoTeste, "json inválido {{{");
        
        var repo = new RepositorioProdutoJson(_caminhoTeste);
        
        Assert.Empty(repo.Listar());
    }
}
