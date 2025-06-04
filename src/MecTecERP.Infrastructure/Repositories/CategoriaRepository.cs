using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
{
    protected override string TableName => "Categorias";
    
    protected override string InsertQuery => @"
        INSERT INTO Categorias (Nome, Descricao, DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao)
        VALUES (@Nome, @Descricao, @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Categorias 
        SET Nome = @Nome, 
            Descricao = @Descricao, 
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao
        WHERE Id = @Id";

    public CategoriaRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<bool> ExisteNomeAsync(string nome, int? idExcluir = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COUNT(1) FROM Categorias WHERE LOWER(Nome) = LOWER(@Nome) AND Ativo = 1";
        
        if (idExcluir.HasValue)
        {
            sql += " AND Id != @IdExcluir";
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, new { Nome = nome, IdExcluir = idExcluir });
        return count > 0;
    }

    public async Task<IEnumerable<Categoria>> ObterAtivasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Categorias WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Categoria>(sql);
    }

    public async Task<IEnumerable<Categoria>> ObterPorFiltroAsync(
        string? nome = null,
        bool? ativa = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "Nome",
        bool ordenarDesc = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (ativa.HasValue)
        {
            whereConditions.Add("Ativo = @Ativa");
            parameters.Add("Ativa", ativa.Value);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "nome" => "Nome",
            "datacriacao" => "DataCriacao",
            "dataatualizacao" => "DataAtualizacao",
            _ => "Nome"
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT * FROM Categorias 
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        return await connection.QueryAsync<Categoria>(sql, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? nome = null,
        bool? ativa = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (ativa.HasValue)
        {
            whereConditions.Add("Ativo = @Ativa");
            parameters.Add("Ativa", ativa.Value);
        }

        var sql = $"SELECT COUNT(*) FROM Categorias WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<Categoria>> ObterComProdutosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT DISTINCT c.* 
            FROM Categorias c
            INNER JOIN Produtos p ON c.Id = p.CategoriaId
            WHERE c.Ativo = 1 AND p.Ativo = 1
            ORDER BY c.Nome";
        
        return await connection.QueryAsync<Categoria>(sql);
    }

    public async Task<IEnumerable<Categoria>> ObterParaSelectAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Id, Nome FROM Categorias WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Categoria>(sql);
    }
}