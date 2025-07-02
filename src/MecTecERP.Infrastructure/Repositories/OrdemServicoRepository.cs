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
        INSERT INTO OrdensServico (Numero, ClienteId, VeiculoId, DataEntrada, DataPrevisaoEntrega, DataConclusao,
                                  ProblemaRelatado, DiagnosticoTecnico, ValorServicos, ValorPecas, ValorDesconto, ValorTotal,
                                  Status, ObservacoesInternas, ObservacoesCliente, MecanicoResponsavelId,
                                  OrcamentoAprovado, DataAprovacaoOrcamento, QuilometragemEntrada,
                                  DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@Numero, @ClienteId, @VeiculoId, @DataEntrada, @DataPrevisaoEntrega, @DataConclusao,
                @ProblemaRelatado, @DiagnosticoTecnico, @ValorServicos, @ValorPecas, @ValorDesconto, @ValorTotal,
                @Status, @ObservacoesInternas, @ObservacoesCliente, @MecanicoResponsavelId,
                @OrcamentoAprovado, @DataAprovacaoOrcamento, @QuilometragemEntrada,
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE OrdensServico SET 
            Numero = @Numero, ClienteId = @ClienteId, VeiculoId = @VeiculoId, DataEntrada = @DataEntrada,
            DataPrevisaoEntrega = @DataPrevisaoEntrega, DataConclusao = @DataConclusao,
            ProblemaRelatado = @ProblemaRelatado, DiagnosticoTecnico = @DiagnosticoTecnico,
            ValorServicos = @ValorServicos, ValorPecas = @ValorPecas, ValorDesconto = @ValorDesconto, ValorTotal = @ValorTotal,
            Status = @Status, ObservacoesInternas = @ObservacoesInternas, ObservacoesCliente = @ObservacoesCliente,
            MecanicoResponsavelId = @MecanicoResponsavelId, OrcamentoAprovado = @OrcamentoAprovado,
            DataAprovacaoOrcamento = @DataAprovacaoOrcamento, QuilometragemEntrada = @QuilometragemEntrada,
            DataAtualizacao = @DataAtualizacao, UsuarioAtualizacao = @UsuarioAtualizacao, Ativo = @Ativo
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
        var query = BuildSelectQuery("ClienteId = @ClienteId AND Ativo = 1 ORDER BY DataEntrada DESC"); // DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorVeiculoAsync(int veiculoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery("VeiculoId = @VeiculoId AND Ativo = 1 ORDER BY DataEntrada DESC"); // DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { VeiculoId = veiculoId });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        // StatusOrdem -> Status
        var query = BuildSelectQuery("Status = @Status AND Ativo = 1 ORDER BY DataEntrada DESC"); // DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { Status = (int)status });
    }

    public async Task<IEnumerable<OrdemServico>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            DataEntrada >= @DataInicio AND DataEntrada <= @DataFim
            AND Ativo = 1 ORDER BY DataEntrada DESC"); // DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { DataInicio = dataInicio, DataFim = dataFim });
    }

    public async Task<IEnumerable<OrdemServico>> ObterAbertasAsync() // Os status de "aberta" mudaram
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            Status IN (@Protocolo, @Orcamento, @Aprovado, @Execucao, @AguardandoPeca)
            AND Ativo = 1 ORDER BY DataEntrada DESC"); // DataAbertura -> DataEntrada, StatusOrdem -> Status
        return await connection.QueryAsync<OrdemServico>(query, new 
        { 
            Protocolo = (int)StatusOrdemServico.Protocolo,
            Orcamento = (int)StatusOrdemServico.Orcamento,
            Aprovado = (int)StatusOrdemServico.Aprovado,
            Execucao = (int)StatusOrdemServico.Execucao,
            AguardandoPeca = (int)StatusOrdemServico.AguardandoPeca
        });
    }

    public async Task<IEnumerable<OrdemServico>> ObterConcluidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            Status = @Finalizado
            AND Ativo = 1 ORDER BY DataConclusao DESC"); // StatusOrdem -> Status
        return await connection.QueryAsync<OrdemServico>(query, new { Finalizado = (int)StatusOrdemServico.Finalizado });
    }

    public async Task<IEnumerable<OrdemServico>> ObterCanceladasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            Status = @Cancelado
            AND Ativo = 1 ORDER BY DataEntrada DESC"); // StatusOrdem -> Status, DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { Cancelado = (int)StatusOrdemServico.Cancelado });
    }

    public async Task<IEnumerable<OrdemServico>> ObterVencidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildSelectQuery(@"
            DataPrevisaoEntrega < @DataAtual
            AND Status IN (@Protocolo, @Orcamento, @Aprovado, @Execucao, @AguardandoPeca)
            AND Ativo = 1 ORDER BY DataPrevisaoEntrega ASC"); // DataPrevista -> DataPrevisaoEntrega, StatusOrdem -> Status
        return await connection.QueryAsync<OrdemServico>(query, new 
        { 
            DataAtual = DateTime.Now.Date,
            Protocolo = (int)StatusOrdemServico.Protocolo,
            Orcamento = (int)StatusOrdemServico.Orcamento,
            Aprovado = (int)StatusOrdemServico.Aprovado,
            Execucao = (int)StatusOrdemServico.Execucao,
            AguardandoPeca = (int)StatusOrdemServico.AguardandoPeca
        });
    }

    public async Task<decimal> ObterValorTotalPorPeriodoAsync(DateTime dataInicio, DateTime dataFim) // Usar DataConclusao para faturamento
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT ISNULL(SUM(ValorTotal), 0) 
            FROM OrdensServico 
            WHERE DataConclusao >= @DataInicio AND DataConclusao <= @DataFim
            AND Status = @Finalizado AND Ativo = 1"; // DataAbertura -> DataConclusao, StatusOrdem -> Status
        return await connection.QuerySingleOrDefaultAsync<decimal>(query, new 
        { 
            DataInicio = dataInicio, 
            DataFim = dataFim, 
            Finalizado = (int)StatusOrdemServico.Finalizado
        });
    }

    public async Task<int> ContarPorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery("Status = @Status AND Ativo = 1"); // StatusOrdem -> Status
        return await connection.QuerySingleAsync<int>(query, new { Status = (int)status });
    }

    public async Task<int> ContarAbertasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery(@"
            Status IN (@Protocolo, @Orcamento, @Aprovado, @Execucao, @AguardandoPeca) AND Ativo = 1"); // StatusOrdem -> Status
        return await connection.QuerySingleAsync<int>(query, new 
        { 
            Protocolo = (int)StatusOrdemServico.Protocolo,
            Orcamento = (int)StatusOrdemServico.Orcamento,
            Aprovado = (int)StatusOrdemServico.Aprovado,
            Execucao = (int)StatusOrdemServico.Execucao,
            AguardandoPeca = (int)StatusOrdemServico.AguardandoPeca
        });
    }

    public async Task<int> ContarVencidasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = BuildCountQuery(@"
            DataPrevisaoEntrega < @DataAtual
            AND Status IN (@Protocolo, @Orcamento, @Aprovado, @Execucao, @AguardandoPeca)
            AND Ativo = 1"); // DataPrevista -> DataPrevisaoEntrega, StatusOrdem -> Status
        return await connection.QuerySingleAsync<int>(query, new 
        { 
            DataAtual = DateTime.Now.Date,
            Protocolo = (int)StatusOrdemServico.Protocolo,
            Orcamento = (int)StatusOrdemServico.Orcamento,
            Aprovado = (int)StatusOrdemServico.Aprovado,
            Execucao = (int)StatusOrdemServico.Execucao,
            AguardandoPeca = (int)StatusOrdemServico.AguardandoPeca
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
        string ordenarPor = "DataEntrada", // Interface já foi ajustada para DataEntrada
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
            whereConditions.Add("Status = @Status"); // StatusOrdem -> Status
            parameters.Add("Status", (int)status.Value);
        }
        
        if (dataInicio.HasValue)
        {
            whereConditions.Add("DataEntrada >= @DataInicio"); // DataAbertura -> DataEntrada
            parameters.Add("DataInicio", dataInicio.Value);
        }
        
        if (dataFim.HasValue)
        {
            whereConditions.Add("DataEntrada <= @DataFim"); // DataAbertura -> DataEntrada
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
            whereConditions.Add("Status = @Status"); // StatusOrdem -> Status
            parameters.Add("Status", (int)status.Value);
        }
        
        if (dataInicio.HasValue)
        {
            whereConditions.Add("DataEntrada >= @DataInicio"); // DataAbertura -> DataEntrada
            parameters.Add("DataInicio", dataInicio.Value);
        }
        
        if (dataFim.HasValue)
        {
            whereConditions.Add("DataEntrada <= @DataFim"); // DataAbertura -> DataEntrada
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
        // O mapeamento de ClienteNome e VeiculoPlaca precisaria ser feito no DTO ou manualmente após a query,
        // pois a entidade OrdemServico não possui essas propriedades diretamente.
        // Por ora, a query retorna os campos, mas o Dapper não os mapeará para a entidade OrdemServico.
        return await connection.QueryFirstOrDefaultAsync<OrdemServico>(query, new { Id = id });
    }

    public async Task<IEnumerable<OrdemServico>> ObterTodasCompletasAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT os.*, c.NomeRazaoSocial as ClienteNome, v.Placa as VeiculoPlaca
            FROM OrdensServico os
            LEFT JOIN Clientes c ON os.ClienteId = c.Id
            LEFT JOIN Veiculos v ON os.VeiculoId = v.Id
            WHERE os.Ativo = 1
            ORDER BY os.DataEntrada DESC"; // DataAbertura -> DataEntrada, c.Nome -> c.NomeRazaoSocial
        return await connection.QueryAsync<OrdemServico>(query); // Similar ao acima, ClienteNome e VeiculoPlaca não serão mapeados para OrdemServico
    }

    public async Task<decimal> ObterFaturamentoPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = @"
            SELECT ISNULL(SUM(ValorTotal), 0) 
            FROM OrdensServico 
            WHERE DataConclusao >= @DataInicio AND DataConclusao <= @DataFim 
            AND Status = @Finalizado AND Ativo = 1"; // StatusOrdem -> Status
        return await connection.QuerySingleOrDefaultAsync<decimal>(query, new 
        { 
            DataInicio = dataInicio, 
            DataFim = dataFim, 
            Finalizado = (int)StatusOrdemServico.Finalizado
        });
    }

    public async Task<int> ObterQuantidadePorStatusAsync(StatusOrdemServico status)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = "SELECT COUNT(*) FROM OrdensServico WHERE Status = @Status AND Ativo = 1"; // StatusOrdem -> Status
        return await connection.QuerySingleAsync<int>(query, new { Status = (int)status });
    }

    public async Task<IEnumerable<OrdemServico>> ObterMaisRecentesAsync(int quantidade = 10)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = $@"
            SELECT TOP (@Quantidade) * 
            FROM OrdensServico 
            WHERE Ativo = 1 
            ORDER BY DataEntrada DESC"; // DataAbertura -> DataEntrada
        return await connection.QueryAsync<OrdemServico>(query, new { Quantidade = quantidade });
    }
}