using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;
using System.Text;

namespace MecTecERP.Infrastructure.Repositories;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    protected override string TableName => "Produtos";
    
    protected override string InsertQuery => @"
        INSERT INTO Produtos (Codigo, Nome, Descricao, Unidade, PrecoCusto, PrecoVenda, 
                             EstoqueAtual, EstoqueMinimo, EstoqueMaximo, Localizacao, FotoUrl,
                             Observacoes, CodigoBarras, CategoriaId, FornecedorId, 
                             DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@Codigo, @Nome, @Descricao, @Unidade, @PrecoCusto, @PrecoVenda, 
                @EstoqueAtual, @EstoqueMinimo, @EstoqueMaximo, @Localizacao, @FotoUrl,
                @Observacoes, @CodigoBarras, @CategoriaId, @FornecedorId, 
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);"; // Unidade aqui será o valor int do enum UnidadeMedida
    
    protected override string UpdateQuery => @"
        UPDATE Produtos SET 
            Codigo = @Codigo, Nome = @Nome, Descricao = @Descricao, Unidade = @Unidade,
            PrecoCusto = @PrecoCusto, PrecoVenda = @PrecoVenda, EstoqueAtual = @EstoqueAtual,
            EstoqueMinimo = @EstoqueMinimo, EstoqueMaximo = @EstoqueMaximo, 
            Localizacao = @Localizacao, FotoUrl = @FotoUrl, Observacoes = @Observacoes, CodigoBarras = @CodigoBarras,
            CategoriaId = @CategoriaId, FornecedorId = @FornecedorId, 
            DataAtualizacao = @DataAtualizacao, UsuarioAtualizacao = @UsuarioAtualizacao, Ativo = @Ativo
        WHERE Id = @Id"; // Unidade aqui será o valor int do enum UnidadeMedida

    public ProdutoRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? idExcluir = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereClause = idExcluir.HasValue 
            ? "Codigo = @Codigo AND Id != @IdExcluir AND Ativo = 1"
            : "Codigo = @Codigo AND Ativo = 1";
            
        var sql = $"SELECT COUNT(1) FROM Produtos WHERE {whereClause}";
        object parameters = idExcluir.HasValue 
            ? new { Codigo = codigo, IdExcluir = idExcluir.Value }
            : new { Codigo = codigo };
            
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count > 0;
    }

    public async Task<bool> ExisteCodigoBarrasAsync(string codigoBarras, int? idExcluir = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereClause = idExcluir.HasValue 
            ? "CodigoBarras = @CodigoBarras AND Id != @IdExcluir AND Ativo = 1"
            : "CodigoBarras = @CodigoBarras AND Ativo = 1";
            
        var sql = $"SELECT COUNT(1) FROM Produtos WHERE {whereClause}";
        object parameters = idExcluir.HasValue 
            ? new { CodigoBarras = codigoBarras, IdExcluir = idExcluir.Value }
            : new { CodigoBarras = codigoBarras };
            
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count > 0;
    }

    public async Task<string> ObterProximoCodigoAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT TOP 1 Codigo 
            FROM Produtos 
            WHERE Codigo LIKE 'PROD%' 
            ORDER BY CAST(SUBSTRING(Codigo, 5, LEN(Codigo) - 4) AS INT) DESC";
            
        var ultimoCodigo = await connection.QueryFirstOrDefaultAsync<string>(sql);
        
        if (string.IsNullOrEmpty(ultimoCodigo))
            return "PROD001";
            
        var numeroStr = ultimoCodigo.Substring(4);
        if (int.TryParse(numeroStr, out int numero))
        {
            return $"PROD{(numero + 1):D3}";
        }
        
        return "PROD001";
    }

    public async Task<IEnumerable<Produto>> ObterAtivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql);
    }

    public async Task<IEnumerable<Produto>> ObterPorCategoriaAsync(int categoriaId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE CategoriaId = @CategoriaId AND Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql, new { CategoriaId = categoriaId });
    }

    public async Task<IEnumerable<Produto>> ObterPorFornecedorAsync(int fornecedorId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE FornecedorId = @FornecedorId AND Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql, new { FornecedorId = fornecedorId });
    }

    public async Task<IEnumerable<Produto>> ObterComEstoqueBaixoAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE EstoqueAtual <= EstoqueMinimo AND Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql);
    }

    public async Task<IEnumerable<Produto>> ObterComEstoqueZeradoAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE EstoqueAtual = 0 AND Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql);
    }

    public async Task<IEnumerable<Produto>> ObterPorStatusEstoqueAsync(StatusEstoque status)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = status switch
        {
            StatusEstoque.Disponivel => "SELECT * FROM Produtos WHERE EstoqueAtual > EstoqueMinimo AND Ativo = 1 ORDER BY Nome",
            StatusEstoque.EstoqueBaixo => "SELECT * FROM Produtos WHERE EstoqueAtual <= EstoqueMinimo AND EstoqueAtual > 0 AND Ativo = 1 ORDER BY Nome",
            StatusEstoque.SemEstoque => "SELECT * FROM Produtos WHERE EstoqueAtual = 0 AND Ativo = 1 ORDER BY Nome",
            StatusEstoque.Bloqueado => "SELECT * FROM Produtos WHERE Ativo = 0 ORDER BY Nome",
            _ => "SELECT * FROM Produtos WHERE Ativo = 1 ORDER BY Nome"
        };
        
        return await connection.QueryAsync<Produto>(sql);
    }

    public async Task<decimal> CalcularValorTotalEstoqueAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT ISNULL(SUM(EstoqueAtual * PrecoCusto), 0) FROM Produtos WHERE Ativo = 1";
        return await connection.QuerySingleOrDefaultAsync<decimal>(sql);
    }

    public async Task<decimal> CalcularValorEstoquePorCategoriaAsync(int categoriaId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT ISNULL(SUM(EstoqueAtual * PrecoCusto), 0) FROM Produtos WHERE CategoriaId = @CategoriaId AND Ativo = 1";
        return await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { CategoriaId = categoriaId });
    }

    public async Task<decimal> CalcularValorEstoquePorFornecedorAsync(int fornecedorId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT ISNULL(SUM(EstoqueAtual * PrecoCusto), 0) FROM Produtos WHERE FornecedorId = @FornecedorId AND Ativo = 1";
        return await connection.QuerySingleOrDefaultAsync<decimal>(sql, new { FornecedorId = fornecedorId });
    }

    public async Task<IEnumerable<Produto>> ObterPorFiltroAsync(
        string? nome = null,
        string? codigo = null,
        string? codigoBarras = null,
        int? categoriaId = null,
        int? fornecedorId = null,
        decimal? precoVendaMin = null,
        decimal? precoVendaMax = null,
        StatusEstoque? statusEstoque = null,
        bool? ativo = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "Nome",
        bool ordenarDesc = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }
        else
        {
            whereConditions.Add("Ativo = 1");
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(codigo))
        {
            whereConditions.Add("Codigo LIKE @Codigo");
            parameters.Add("Codigo", $"%{codigo}%");
        }

        if (!string.IsNullOrWhiteSpace(codigoBarras))
        {
            whereConditions.Add("CodigoBarras LIKE @CodigoBarras");
            parameters.Add("CodigoBarras", $"%{codigoBarras}%");
        }

        if (categoriaId.HasValue)
        {
            whereConditions.Add("CategoriaId = @CategoriaId");
            parameters.Add("CategoriaId", categoriaId.Value);
        }

        if (fornecedorId.HasValue)
        {
            whereConditions.Add("FornecedorId = @FornecedorId");
            parameters.Add("FornecedorId", fornecedorId.Value);
        }

        if (precoVendaMin.HasValue)
        {
            whereConditions.Add("PrecoVenda >= @PrecoVendaMin");
            parameters.Add("PrecoVendaMin", precoVendaMin.Value);
        }

        if (precoVendaMax.HasValue)
        {
            whereConditions.Add("PrecoVenda <= @PrecoVendaMax");
            parameters.Add("PrecoVendaMax", precoVendaMax.Value);
        }

        if (statusEstoque.HasValue)
        {
            var statusCondition = statusEstoque.Value switch
            {
                StatusEstoque.Disponivel => "EstoqueAtual > EstoqueMinimo",
                StatusEstoque.EstoqueBaixo => "EstoqueAtual <= EstoqueMinimo AND EstoqueAtual > 0",
                StatusEstoque.SemEstoque => "EstoqueAtual = 0",
                StatusEstoque.Bloqueado => "Ativo = 0",
                _ => "1=1"
            };
            whereConditions.Add(statusCondition);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "nome" => "Nome",
            "codigo" => "Codigo",
            "precovenda" => "PrecoVenda",
            "precocusto" => "PrecoCusto",
            "estoqueatual" => "EstoqueAtual",
            "datacriacao" => "DataCriacao",
            _ => "Nome"
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT * FROM Produtos 
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        return await connection.QueryAsync<Produto>(sql, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? nome = null,
        string? codigo = null,
        string? codigoBarras = null,
        int? categoriaId = null,
        int? fornecedorId = null,
        decimal? precoVendaMin = null,
        decimal? precoVendaMax = null,
        StatusEstoque? statusEstoque = null,
        bool? ativo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }
        else
        {
            whereConditions.Add("Ativo = 1");
        }

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(codigo))
        {
            whereConditions.Add("Codigo LIKE @Codigo");
            parameters.Add("Codigo", $"%{codigo}%");
        }

        if (!string.IsNullOrWhiteSpace(codigoBarras))
        {
            whereConditions.Add("CodigoBarras LIKE @CodigoBarras");
            parameters.Add("CodigoBarras", $"%{codigoBarras}%");
        }

        if (categoriaId.HasValue)
        {
            whereConditions.Add("CategoriaId = @CategoriaId");
            parameters.Add("CategoriaId", categoriaId.Value);
        }

        if (fornecedorId.HasValue)
        {
            whereConditions.Add("FornecedorId = @FornecedorId");
            parameters.Add("FornecedorId", fornecedorId.Value);
        }

        if (precoVendaMin.HasValue)
        {
            whereConditions.Add("PrecoVenda >= @PrecoVendaMin");
            parameters.Add("PrecoVendaMin", precoVendaMin.Value);
        }

        if (precoVendaMax.HasValue)
        {
            whereConditions.Add("PrecoVenda <= @PrecoVendaMax");
            parameters.Add("PrecoVendaMax", precoVendaMax.Value);
        }

        if (statusEstoque.HasValue)
        {
            var statusCondition = statusEstoque.Value switch
            {
                StatusEstoque.Disponivel => "EstoqueAtual > EstoqueMinimo",
                StatusEstoque.EstoqueBaixo => "EstoqueAtual <= EstoqueMinimo AND EstoqueAtual > 0",
                StatusEstoque.SemEstoque => "EstoqueAtual = 0",
                StatusEstoque.Bloqueado => "Ativo = 0",
                _ => "1=1"
            };
            whereConditions.Add(statusCondition);
        }

        var sql = $@"
            SELECT COUNT(*) FROM Produtos 
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<Produto>> ObterParaSelectAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Id, Nome, Codigo FROM Produtos WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Produto>(sql);
    }

    public async Task<IEnumerable<Produto>> BuscarPorCodigoAsync(string codigo)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE Codigo LIKE @Codigo AND Ativo = 1";
        return await connection.QueryAsync<Produto>(sql, new { Codigo = $"%{codigo}%" });
    }

    public async Task<IEnumerable<Produto>> BuscarPorNomeAsync(string nome)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Produtos WHERE Nome LIKE @Nome AND Ativo = 1";
        return await connection.QueryAsync<Produto>(sql, new { Nome = $"%{nome}%" });
    }
}