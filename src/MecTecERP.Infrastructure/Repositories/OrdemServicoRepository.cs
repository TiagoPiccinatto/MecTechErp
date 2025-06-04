using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Enums;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;
using System.Text;

namespace MecTecERP.Infrastructure.Repositories;

public class OrdemServicoRepository : BaseRepository<OrdemServico>, IOrdemServicoRepository
{
    protected override string TableName => "OrdensServico";
    
    protected override string InsertQuery => @"
        INSERT INTO OrdensServico (Numero, ClienteId, VeiculoId, DataAbertura, DataPrevista, 
                                  DataConclusao, StatusOrdem, Descricao, Observacoes, 
                                  ValorServicos, ValorPecas, ValorTotal, Desconto, 
                                  DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@Numero, @ClienteId, @VeiculoId, @DataAbertura, @DataPrevista, 
                @DataConclusao, @StatusOrdem, @Descricao, @Observacoes, 
                @ValorServicos, @ValorPecas, @ValorTotal, @Desconto, 
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE OrdensServico SET 
            Numero = @Numero, ClienteId = @ClienteId, VeiculoId = @VeiculoId, 
            DataAbertura = @DataAbertura, DataPrevista = @DataPrevista, 
            DataConclusao = @DataConclusao, StatusOrdem = @StatusOrdem, 
            Descricao = @Descricao, Observacoes = @Observacoes, 
            ValorServicos = @ValorServicos, ValorPecas = @ValorPecas, 
            ValorTotal = @ValorTotal, Desconto = @Desconto, 
            DataAtualizacao = @DataAtualizacao, UsuarioAtualizacao = @UsuarioAtualizacao
        WHERE Id = @Id";

    public OrdemServicoRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<string> ObterProximoNumeroAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT TOP 1 Numero 
            FROM OrdensServico 
            WHERE Numero LIKE 'OS%' 
            ORDER BY CAST(SUBSTRING(Numero, 3, LEN(Numero) - 2) AS INT) DESC";
            
        var ultimoNumero = await connection.QueryFirstOrDefaultAsync<string>(query);
        
        if (string.IsNullOrEmpty(ultimoNumero))
            return "OS001";
            
        var numeroStr = ultimoNumero.Substring(2);
        if (int.TryParse(numeroStr, out int numero))
        {
            return $"OS{(numero + 1):D3}";
        }
        
        return "OS001";
    }

    public async Task<OrdemServico?> ObterPorNumeroAsync(string numero)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery("Numero = @Numero AND Ativo = 1");
        return await connection.QueryFirstOrDefaultAsync<OrdemServico>(query, new { Numero = numero });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorClienteAsync(int clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery("ClienteId = @ClienteId AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorVeiculoAsync(int veiculoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery("VeiculoId = @VeiculoId AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { VeiculoId = veiculoId });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery("StatusOrdem = @Status AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { Status = (int)status });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            DataAbertura >= @DataInicio AND DataAbertura <= @DataFim 
            AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { DataInicio = dataInicio, DataFim = dataFim });
    }

    public async Task<IEnumerable<OrdemServico>> ObterAbertasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            StatusOrdem IN (@Aberta, @EmAndamento) 
            AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new 
        { 
            Aberta = (int)StatusOrdemServico.Aberta, 
            EmAndamento = (int)StatusOrdemServico.EmAndamento 
        });
    }

    public async Task<IEnumerable<OrdemServico>> ObterConcluidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            StatusOrdem = @Concluida 
            AND Ativo = 1 ORDER BY DataConclusao DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { Concluida = (int)StatusOrdemServico.Finalizada });
    }

    public async Task<IEnumerable<OrdemServico>> ObterCanceladasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            StatusOrdem = @Cancelada 
            AND Ativo = 1 ORDER BY DataAbertura DESC");
        return await connection.QueryAsync<OrdemServico>(query, new { Cancelada = (int)StatusOrdemServico.Cancelada });
    }

    public async Task<IEnumerable<OrdemServico>> ObterVencidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            DataPrevista < @DataAtual 
            AND StatusOrdem IN (@Aberta, @EmAndamento) 
            AND Ativo = 1 ORDER BY DataPrevista ASC");
        return await connection.QueryAsync<OrdemServico>(query, new 
        { 
            DataAtual = DateTime.Now.Date,
            Aberta = (int)StatusOrdemServico.Aberta, 
            EmAndamento = (int)StatusOrdemServico.EmAndamento 
        });
    }

    public async Task<decimal> ObterValorTotalPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT ISNULL(SUM(ValorTotal), 0) 
            FROM OrdensServico 
            WHERE DataAbertura >= @DataInicio AND DataAbertura <= @DataFim 
            AND StatusOrdem = @Concluida AND Ativo = 1";
        return await connection.QuerySingleOrDefaultAsync<decimal>(query, new 
        { 
            DataInicio = dataInicio, 
            DataFim = dataFim, 
            Concluida = (int)StatusOrdemServico.Finalizada 
        });
    }

    public async Task<int> ContarPorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery("StatusOrdem = @Status AND Ativo = 1");
        return await connection.QuerySingleAsync<int>(query, new { Status = (int)status });
    }

    public async Task<int> ContarAbertasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery(@"
            StatusOrdem IN (@Aberta, @EmAndamento) AND Ativo = 1");
        return await connection.QuerySingleAsync<int>(query, new 
        { 
            Aberta = (int)StatusOrdemServico.Aberta, 
            EmAndamento = (int)StatusOrdemServico.EmAndamento 
        });
    }

    public async Task<int> ContarVencidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery(@"
            DataPrevista < @DataAtual 
            AND StatusOrdem IN (@Aberta, @EmAndamento) 
            AND Ativo = 1");
        return await connection.QuerySingleAsync<int>(query, new 
        { 
            DataAtual = DateTime.Now.Date,
            Aberta = (int)StatusOrdemServico.Aberta, 
            EmAndamento = (int)StatusOrdemServico.EmAndamento 
        });
    }

    public async Task<bool> ExisteNumeroAsync(string numero, int? ordemId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereClause = ordemId.HasValue 
            ? "Numero = @Numero AND Id != @OrdemId AND Ativo = 1"
            : "Numero = @Numero AND Ativo = 1";
            
        var query = BuildCountQuery(whereClause);
        var parameters = ordemId.HasValue 
            ? new { Numero = numero, OrdemId = (int?)ordemId.Value }
            : new { Numero = numero, OrdemId = (int?)null };
            
        var count = await connection.QuerySingleAsync<int>(query, parameters);
        return count > 0;
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorFiltroAsync(
        string? numero = null,
        int? clienteId = null,
        int? veiculoId = null,
        StatusOrdemServico? status = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "DataAbertura",
        bool ordenarDesc = true)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "Ativo = 1" };
        var parameters = new DynamicParameters();
        
        if (!string.IsNullOrEmpty(numero))
        {
            whereConditions.Add("Numero LIKE @Numero");
            parameters.Add("Numero", $"%{numero}%");
        }
        
        if (clienteId.HasValue)
        {
            whereConditions.Add("ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }
        
        if (veiculoId.HasValue)
        {
            whereConditions.Add("VeiculoId = @VeiculoId");
            parameters.Add("VeiculoId", veiculoId.Value);
        }
        
        if (status.HasValue)
        {
            whereConditions.Add("StatusOrdem = @Status");
            parameters.Add("Status", (int)status.Value);
        }
        
        if (dataInicio.HasValue)
        {
            whereConditions.Add("DataAbertura >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value);
        }
        
        if (dataFim.HasValue)
        {
            whereConditions.Add("DataAbertura <= @DataFim");
            parameters.Add("DataFim", dataFim.Value);
        }
        
        var whereClause = string.Join(" AND ", whereConditions);
        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        
        var query = $@"
            SELECT * FROM {TableName} 
            WHERE {whereClause}
            ORDER BY {ordenarPor} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            
        parameters.Add("Offset", (pagina - 1) * tamanhoPagina);
        parameters.Add("PageSize", tamanhoPagina);
        
        return await connection.QueryAsync<OrdemServico>(query, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? numero = null,
        int? clienteId = null,
        int? veiculoId = null,
        StatusOrdemServico? status = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "Ativo = 1" };
        var parameters = new DynamicParameters();
        
        if (!string.IsNullOrEmpty(numero))
        {
            whereConditions.Add("Numero LIKE @Numero");
            parameters.Add("Numero", $"%{numero}%");
        }
        
        if (clienteId.HasValue)
        {
            whereConditions.Add("ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }
        
        if (veiculoId.HasValue)
        {
            whereConditions.Add("VeiculoId = @VeiculoId");
            parameters.Add("VeiculoId", veiculoId.Value);
        }
        
        if (status.HasValue)
        {
            whereConditions.Add("StatusOrdem = @Status");
            parameters.Add("Status", (int)status.Value);
        }
        
        if (dataInicio.HasValue)
        {
            whereConditions.Add("DataAbertura >= @DataInicio");
            parameters.Add("DataInicio", dataInicio.Value);
        }
        
        if (dataFim.HasValue)
        {
            whereConditions.Add("DataAbertura <= @DataFim");
            parameters.Add("DataFim", dataFim.Value);
        }
        
        var whereClause = string.Join(" AND ", whereConditions);
        var query = $"SELECT COUNT(*) FROM {TableName} WHERE {whereClause}";
        
        return await connection.QuerySingleAsync<int>(query, parameters);
    }

    public async Task<OrdemServico?> ObterCompletaAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT os.*, c.Nome as ClienteNome, v.Placa as VeiculoPlaca
            FROM OrdensServico os
            LEFT JOIN Clientes c ON os.ClienteId = c.Id
            LEFT JOIN Veiculos v ON os.VeiculoId = v.Id
            WHERE os.Id = @Id AND os.Ativo = 1";
        return await connection.QueryFirstOrDefaultAsync<OrdemServico>(query, new { Id = id });
    }

    public async Task<IEnumerable<OrdemServico>> ObterTodasCompletasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT os.*, c.Nome as ClienteNome, v.Placa as VeiculoPlaca
            FROM OrdensServico os
            LEFT JOIN Clientes c ON os.ClienteId = c.Id
            LEFT JOIN Veiculos v ON os.VeiculoId = v.Id
            WHERE os.Ativo = 1
            ORDER BY os.DataAbertura DESC";
        return await connection.QueryAsync<OrdemServico>(query);
    }

    public async Task<decimal> ObterFaturamentoPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT ISNULL(SUM(ValorTotal), 0) 
            FROM OrdensServico 
            WHERE DataConclusao >= @DataInicio AND DataConclusao <= @DataFim 
            AND StatusOrdem = @Concluida AND Ativo = 1";
        return await connection.QuerySingleOrDefaultAsync<decimal>(query, new 
        { 
            DataInicio = dataInicio, 
            DataFim = dataFim, 
            Concluida = (int)StatusOrdemServico.Finalizada 
        });
    }

    public async Task<int> ObterQuantidadePorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = "SELECT COUNT(*) FROM OrdensServico WHERE StatusOrdem = @Status AND Ativo = 1";
        return await connection.QuerySingleAsync<int>(query, new { Status = (int)status });
    }

    public async Task<IEnumerable<OrdemServico>> ObterMaisRecentesAsync(int quantidade = 10)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = $@"
            SELECT TOP (@Quantidade) * 
            FROM OrdensServico 
            WHERE Ativo = 1 
            ORDER BY DataAbertura DESC";
        return await connection.QueryAsync<OrdemServico>(query, new { Quantidade = quantidade });
    }
}