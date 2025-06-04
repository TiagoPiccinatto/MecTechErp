using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Domain.Enums;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class InventarioRepository : BaseRepository<Inventario>, IInventarioRepository
{
    protected override string TableName => "Inventarios";
    
    protected override string InsertQuery => @"
        INSERT INTO Inventarios (Descricao, Status, DataInicio, DataFinalizacao, 
                                Observacoes, DataCriacao, DataAtualizacao, 
                                Ativo, UsuarioCriacao)
        VALUES (@Descricao, @Status, @DataInicio, @DataFinalizacao, 
                @Observacoes, @DataCriacao, @DataAtualizacao, 
                @Ativo, @UsuarioCriacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Inventarios 
        SET Descricao = @Descricao, 
            Status = @Status,
            DataInicio = @DataInicio,
            DataFinalizacao = @DataFinalizacao,
            Observacoes = @Observacoes,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao
        WHERE Id = @Id";

    public InventarioRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<IEnumerable<Inventario>> ObterPorStatusAsync(StatusInventario status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Inventarios WHERE Status = @Status ORDER BY DataCriacao DESC";
        return await connection.QueryAsync<Inventario>(sql, new { Status = status });
    }

    public async Task<IEnumerable<Inventario>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT * FROM Inventarios 
            WHERE CAST(DataCriacao AS DATE) >= @DataInicio 
              AND CAST(DataCriacao AS DATE) <= @DataFim
            ORDER BY DataCriacao DESC";
        
        return await connection.QueryAsync<Inventario>(sql, new 
        { 
            DataInicio = dataInicio.Date, 
            DataFim = dataFim.Date 
        });
    }

    public async Task<Inventario?> ObterComItensAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT i.*, ii.*, p.*, c.*
            FROM Inventarios i
            LEFT JOIN ItensInventario ii ON i.Id = ii.InventarioId
            LEFT JOIN Produtos p ON ii.ProdutoId = p.Id
            LEFT JOIN Categorias c ON p.CategoriaId = c.Id
            WHERE i.Id = @Id";
        
        var inventarioDict = new Dictionary<int, Inventario>();
        
        var result = await connection.QueryAsync<Inventario, InventarioItem, Produto, Categoria, Inventario>(
            sql,
            (inventario, item, produto, categoria) =>
            {
                if (!inventarioDict.TryGetValue(inventario.Id, out var inventarioEntry))
                {
                    inventarioEntry = inventario;
                    inventarioEntry.Itens = new List<InventarioItem>();
                    inventarioDict.Add(inventario.Id, inventarioEntry);
                }
                
                if (item != null)
                {
                    if (produto != null)
                    {
                        produto.Categoria = categoria;
                        item.Produto = produto;
                    }
                    inventarioEntry.Itens.Add(item);
                }
                
                return inventarioEntry;
            },
            new { Id = id },
            splitOn: "Id,Id,Id"
        );
        
        return result.FirstOrDefault();
    }

    public async Task<bool> ExisteInventarioAbertoAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Inventarios WHERE Status = @Status";
        var count = await connection.QuerySingleAsync<int>(sql, new { Status = StatusInventario.EmAndamento });
        return count > 0;
    }

    public async Task<IEnumerable<Inventario>> ObterPorFiltroAsync(
        string? descricao = null,
        StatusInventario? status = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "DataCriacao",
        bool ordenarDesc = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(descricao))
        {
            whereConditions.Add("Descricao LIKE @Descricao");
            parameters.Add("Descricao", $"%{descricao}%");
        }

        if (status.HasValue)
        {
            whereConditions.Add("Status = @Status");
            parameters.Add("Status", status.Value);
        }

        if (dataInicio.HasValue)
        {
            whereConditions.Add("CAST(DataCriacao AS DATE) >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value.Date);
        }

        if (dataFim.HasValue)
        {
            whereConditions.Add("CAST(DataCriacao AS DATE) <= @DataFim");
            parameters.Add("DataFim", dataFim.Value.Date);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "datacriacao" => "DataCriacao",
            "descricao" => "Descricao",
            "status" => "Status",
            "datafinalizacao" => "DataFinalizacao",
            _ => "DataCriacao"
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT * FROM Inventarios 
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        return await connection.QueryAsync<Inventario>(sql, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? descricao = null,
        StatusInventario? status = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(descricao))
        {
            whereConditions.Add("Descricao LIKE @Descricao");
            parameters.Add("Descricao", $"%{descricao}%");
        }

        if (status.HasValue)
        {
            whereConditions.Add("Status = @Status");
            parameters.Add("Status", status.Value);
        }

        if (dataInicio.HasValue)
        {
            whereConditions.Add("CAST(DataCriacao AS DATE) >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value.Date);
        }

        if (dataFim.HasValue)
        {
            whereConditions.Add("CAST(DataCriacao AS DATE) <= @DataFim");
            parameters.Add("DataFim", dataFim.Value.Date);
        }

        var sql = $@"
            SELECT COUNT(*) FROM Inventarios 
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<object>> ObterRelatorioInventarioAsync(int inventarioId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT 
                ii.ProdutoId,
                p.Nome as ProdutoNome,
                p.Codigo as ProdutoCodigo,
                c.Nome as CategoriaNome,
                ii.EstoqueSistema,
                ii.EstoqueContado,
                (ii.EstoqueContado - ii.EstoqueSistema) as Divergencia,
                p.PrecoCompra as ValorUnitario,
                ((ii.EstoqueContado - ii.EstoqueSistema) * p.PrecoCompra) as ValorDivergencia,
                ii.Observacoes
            FROM ItensInventario ii
            INNER JOIN Produtos p ON ii.ProdutoId = p.Id
            INNER JOIN Categorias c ON p.CategoriaId = c.Id
            WHERE ii.InventarioId = @InventarioId";
        
        return await connection.QueryAsync(sql, new { InventarioId = inventarioId });
    }

    public async Task<IEnumerable<object>> ObterRelatorioDivergenciasAsync(int inventarioId)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = @"
            SELECT 
                ii.ProdutoId,
                p.Nome as ProdutoNome,
                p.Codigo as ProdutoCodigo,
                c.Nome as CategoriaNome,
                ii.EstoqueSistema,
                ii.EstoqueContado,
                (ii.EstoqueContado - ii.EstoqueSistema) as Divergencia,
                CASE 
                    WHEN ii.EstoqueContado > ii.EstoqueSistema THEN 'Sobra'
                    ELSE 'Falta'
                END as TipoDivergencia,
                p.PrecoCompra as ValorUnitario,
                ((ii.EstoqueContado - ii.EstoqueSistema) * p.PrecoCompra) as ValorDivergencia,
                ii.Observacoes
            FROM ItensInventario ii
            INNER JOIN Produtos p ON ii.ProdutoId = p.Id
            INNER JOIN Categorias c ON p.CategoriaId = c.Id
            WHERE ii.InventarioId = @InventarioId 
              AND ii.EstoqueContado != ii.EstoqueSistema
            ORDER BY ABS((ii.EstoqueContado - ii.EstoqueSistema) * p.PrecoCompra) DESC";
        
        return await connection.QueryAsync(sql, new { InventarioId = inventarioId });
    }
}