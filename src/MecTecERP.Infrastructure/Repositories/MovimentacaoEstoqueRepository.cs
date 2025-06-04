using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Domain.Enums;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class MovimentacaoEstoqueRepository : BaseRepository<MovimentacaoEstoque>, IMovimentacaoEstoqueRepository
{
    protected override string TableName => "MovimentacoesEstoque";
    
    protected override string InsertQuery => @"
        INSERT INTO MovimentacoesEstoque (ProdutoId, InventarioId, Tipo, Quantidade, 
                                         QuantidadeAnterior, QuantidadeAtual, Observacoes, 
                                         DataMovimentacao, DataCriacao, DataAtualizacao, 
                                         Ativo, UsuarioCriacao)
        VALUES (@ProdutoId, @InventarioId, @Tipo, @Quantidade, 
                @QuantidadeAnterior, @QuantidadeAtual, @Observacoes, 
                @DataMovimentacao, @DataCriacao, @DataAtualizacao, 
                @Ativo, @UsuarioCriacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE MovimentacoesEstoque 
        SET ProdutoId = @ProdutoId, 
            InventarioId = @InventarioId,
            Tipo = @Tipo,
            Quantidade = @Quantidade,
            QuantidadeAnterior = @QuantidadeAnterior,
            QuantidadeAtual = @QuantidadeAtual,
            Observacoes = @Observacoes,
            DataMovimentacao = @DataMovimentacao,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao
        WHERE Id = @Id";

    public MovimentacaoEstoqueRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorProdutoAsync(int produtoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE m.ProdutoId = @ProdutoId
            ORDER BY m.DataMovimentacao DESC";
        
        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            new { ProdutoId = produtoId },
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorInventarioAsync(int inventarioId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE m.InventarioId = @InventarioId
            ORDER BY p.Nome";
        
        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            new { InventarioId = inventarioId },
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE CAST(m.DataMovimentacao AS DATE) >= @DataInicio 
              AND CAST(m.DataMovimentacao AS DATE) <= @DataFim
            ORDER BY m.DataMovimentacao DESC";
        
        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            new { DataInicio = dataInicio.Date, DataFim = dataFim.Date },
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorTipoAsync(TipoMovimentacao tipo)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE m.Tipo = @Tipo
            ORDER BY m.DataMovimentacao DESC";
        
        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            new { Tipo = tipo },
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterPorFiltroAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? produtoId = null,
        int? inventarioId = null,
        TipoMovimentacao? tipo = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "DataMovimentacao",
        bool ordenarDesc = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (produtoId.HasValue)
        {
            whereConditions.Add("m.ProdutoId = @ProdutoId");
            parameters.Add("ProdutoId", produtoId.Value);
        }

        if (inventarioId.HasValue)
        {
            whereConditions.Add("m.InventarioId = @InventarioId");
            parameters.Add("InventarioId", inventarioId.Value);
        }

        if (tipo.HasValue)
        {
            whereConditions.Add("m.Tipo = @Tipo");
            parameters.Add("Tipo", tipo.Value);
        }

        if (dataInicio.HasValue)
        {
            whereConditions.Add("CAST(m.DataMovimentacao AS DATE) >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value.Date);
        }

        if (dataFim.HasValue)
        {
            whereConditions.Add("CAST(m.DataMovimentacao AS DATE) <= @DataFim");
            parameters.Add("DataFim", dataFim.Value.Date);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "datamovimentacao" => "m.DataMovimentacao",
            "produto" => "p.Nome",
            "tipo" => "m.Tipo",
            "quantidade" => "m.Quantidade",
            "datacriacao" => "m.DataCriacao",
            _ => "m.DataMovimentacao"
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            parameters,
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<int> ContarPorFiltroAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int? produtoId = null,
        int? inventarioId = null,
        TipoMovimentacao? tipo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (produtoId.HasValue)
        {
            whereConditions.Add("m.ProdutoId = @ProdutoId");
            parameters.Add("ProdutoId", produtoId.Value);
        }

        if (inventarioId.HasValue)
        {
            whereConditions.Add("m.InventarioId = @InventarioId");
            parameters.Add("InventarioId", inventarioId.Value);
        }

        if (tipo.HasValue)
        {
            whereConditions.Add("m.Tipo = @Tipo");
            parameters.Add("Tipo", tipo.Value);
        }

        if (dataInicio.HasValue)
        {
            whereConditions.Add("CAST(m.DataMovimentacao AS DATE) >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value.Date);
        }

        if (dataFim.HasValue)
        {
            whereConditions.Add("CAST(m.DataMovimentacao AS DATE) <= @DataFim");
            parameters.Add("DataFim", dataFim.Value.Date);
        }

        var sql = $@"
            SELECT COUNT(*) 
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<decimal> CalcularSaldoProdutoAsync(int produtoId, DateTime? dataLimite = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "ProdutoId = @ProdutoId" };
        var parameters = new DynamicParameters();
        parameters.Add("ProdutoId", produtoId);
        
        if (dataLimite.HasValue)
        {
            whereConditions.Add("DataMovimentacao <= @DataLimite");
            parameters.Add("DataLimite", dataLimite.Value);
        }
        
        var sql = $@"
            SELECT ISNULL(SUM(
                CASE 
                    WHEN Tipo IN (0, 2, 4) THEN Quantidade  -- Entrada, Ajuste Positivo, Devolução
                    WHEN Tipo IN (1, 3, 5) THEN -Quantidade -- Saída, Ajuste Negativo, Transferência
                    ELSE 0
                END
            ), 0)
            FROM MovimentacoesEstoque 
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<decimal>(sql, parameters);
    }

    public async Task<decimal> ObterSaldoAtualProdutoAsync(int produtoId)
    {
        return await CalcularSaldoProdutoAsync(produtoId);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> ObterUltimasMovimentacoesAsync(int quantidade = 10)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $@"
            SELECT TOP (@Quantidade) m.*, p.*, i.*
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Inventarios i ON m.InventarioId = i.Id
            ORDER BY m.DataMovimentacao DESC";
        
        var movimentacaoDict = new Dictionary<int, MovimentacaoEstoque>();
        
        var result = await connection.QueryAsync<MovimentacaoEstoque, Produto, Inventario, MovimentacaoEstoque>(
            sql,
            (movimentacao, produto, inventario) =>
            {
                if (!movimentacaoDict.TryGetValue(movimentacao.Id, out var movimentacaoEntry))
                {
                    movimentacaoEntry = movimentacao;
                    movimentacaoEntry.Produto = produto;
                    movimentacaoEntry.Inventario = inventario;
                    movimentacaoDict.Add(movimentacao.Id, movimentacaoEntry);
                }
                return movimentacaoEntry;
            },
            new { Quantidade = quantidade },
            splitOn: "Id,Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<object>> ObterRelatorioMovimentacaoAsync(
        DateTime dataInicio,
        DateTime dataFim,
        int? produtoId = null,
        int? categoriaId = null,
        TipoMovimentacao? tipo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> 
        { 
            "CAST(m.DataMovimentacao AS DATE) >= @DataInicio",
            "CAST(m.DataMovimentacao AS DATE) <= @DataFim"
        };
        var parameters = new DynamicParameters();
        parameters.Add("DataInicio", dataInicio.Date);
        parameters.Add("DataFim", dataFim.Date);
        
        if (produtoId.HasValue)
        {
            whereConditions.Add("m.ProdutoId = @ProdutoId");
            parameters.Add("ProdutoId", produtoId.Value);
        }
        
        if (categoriaId.HasValue)
        {
            whereConditions.Add("p.CategoriaId = @CategoriaId");
            parameters.Add("CategoriaId", categoriaId.Value);
        }
        
        if (tipo.HasValue)
        {
            whereConditions.Add("m.Tipo = @Tipo");
            parameters.Add("Tipo", tipo.Value);
        }
        
        var sql = $@"
            SELECT 
                p.Nome as ProdutoNome,
                p.Codigo as ProdutoCodigo,
                c.Nome as CategoriaNome,
                m.Tipo,
                m.Quantidade,
                m.QuantidadeAnterior,
                m.QuantidadeAtual,
                m.DataMovimentacao,
                m.Observacoes,
                CASE 
                    WHEN m.Tipo = 0 THEN 'Entrada'
                    WHEN m.Tipo = 1 THEN 'Saída'
                    WHEN m.Tipo = 2 THEN 'Ajuste Positivo'
                    WHEN m.Tipo = 3 THEN 'Ajuste Negativo'
                    WHEN m.Tipo = 4 THEN 'Devolução'
                    WHEN m.Tipo = 5 THEN 'Transferência'
                    ELSE 'Desconhecido'
                END as TipoDescricao
            FROM MovimentacoesEstoque m
            INNER JOIN Produtos p ON m.ProdutoId = p.Id
            LEFT JOIN Categorias c ON p.CategoriaId = c.Id
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY m.DataMovimentacao DESC, p.Nome";
        
        return await connection.QueryAsync(sql, parameters);
    }

    public async Task<bool> RegistrarMovimentacaoAsync(MovimentacaoEstoque movimentacao)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();
        
        try
        {
            // Inserir a movimentação
            var movimentacaoId = await connection.QuerySingleAsync<int>(
                InsertQuery, 
                movimentacao, 
                transaction
            );
            
            // Atualizar o estoque do produto
            var updateEstoqueSql = @"
                UPDATE Produtos 
                SET EstoqueAtual = @NovoEstoque,
                    DataAtualizacao = @DataAtualizacao,
                    UsuarioAtualizacao = @UsuarioAtualizacao
                WHERE Id = @ProdutoId";
            
            await connection.ExecuteAsync(
                updateEstoqueSql,
                new 
                {
                    NovoEstoque = movimentacao.EstoquePosterior,
                    DataAtualizacao = DateTime.Now,
                    UsuarioAtualizacao = movimentacao.UsuarioCriacao,
                    ProdutoId = movimentacao.ProdutoId
                },
                transaction
            );
            
            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}