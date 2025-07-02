using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class VeiculoRepository : BaseRepository<Veiculo>, IVeiculoRepository
{
    protected override string TableName => "Veiculos";
    
    protected override string InsertQuery => @"
        INSERT INTO Veiculos (ClienteId, Placa, Marca, Modelo, AnoFabricacao, AnoModelo, Cor, Chassi,
                             KmAtual, TipoCombustivel, Renavam, Foto, Observacoes,
                             DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@ClienteId, @Placa, @Marca, @Modelo, @AnoFabricacao, @AnoModelo, @Cor, @Chassi,
                @KmAtual, @TipoCombustivel, @Renavam, @Foto, @Observacoes,
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Veiculos 
        SET ClienteId = @ClienteId,
            Placa = @Placa,
            Marca = @Marca,
            Modelo = @Modelo,
            AnoFabricacao = @AnoFabricacao,
            AnoModelo = @AnoModelo,
            Cor = @Cor,
            Chassi = @Chassi,
            KmAtual = @KmAtual,
            TipoCombustivel = @TipoCombustivel,
            Renavam = @Renavam,
            Foto = @Foto,
            Observacoes = @Observacoes,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao,
            Ativo = @Ativo
        WHERE Id = @Id";

    public VeiculoRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<bool> ExistePlacaAsync(string placa, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COUNT(1) FROM Veiculos WHERE Placa = @Placa";
        
        if (excludeId.HasValue)
        {
            sql += " AND Id != @ExcludeId";
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, new { Placa = placa, ExcludeId = excludeId });
        return count > 0;
    }

    public async Task<Veiculo?> ObterPorPlacaAsync(string placa)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Veiculos WHERE Placa = @Placa";
        return await connection.QueryFirstOrDefaultAsync<Veiculo>(sql, new { Placa = placa });
    }

    public async Task<Veiculo?> ObterPorPlacaComClienteAsync(string placa)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE v.Placa = @Placa";
        
        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            new { Placa = placa },
            splitOn: "Id"
        );
        
        return result.FirstOrDefault();
    }

    public async Task<Veiculo?> ObterComClienteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE v.Id = @Id";
        
        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            new { Id = id },
            splitOn: "Id"
        );
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Veiculo>> ObterTodosComClienteAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            ORDER BY v.Placa";
        
        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            splitOn: "Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<Veiculo>> ObterPorClienteAsync(int clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Veiculos WHERE ClienteId = @ClienteId AND Ativo = 1 ORDER BY Placa";
        return await connection.QueryAsync<Veiculo>(sql, new { ClienteId = clienteId });
    }

    public async Task<bool> PossuiOrdensServicoAsync(int veiculoId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM OrdensServico WHERE VeiculoId = @VeiculoId";
        var count = await connection.QuerySingleAsync<int>(sql, new { VeiculoId = veiculoId });
        return count > 0;
    }

    public async Task<IEnumerable<Veiculo>> ObterPorClienteComClienteAsync(int clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE v.ClienteId = @ClienteId AND v.Ativo = 1
            ORDER BY v.Placa";
        
        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            new { ClienteId = clienteId },
            splitOn: "Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<Veiculo>> ObterAtivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Veiculos WHERE Ativo = 1 ORDER BY Placa";
        return await connection.QueryAsync<Veiculo>(sql);
    }

    public async Task<IEnumerable<Veiculo>> ObterAtivosComClienteAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE v.Ativo = 1
            ORDER BY v.Placa";
        
        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            splitOn: "Id"
        );
        
        return result.Distinct();
    }

    public async Task<IEnumerable<Veiculo>> ObterPorFiltroAsync(
        string? placa = null,
        string? modelo = null,
        string? marca = null,
        int? clienteId = null,
        bool? ativo = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "Placa",
        bool ordenarDesc = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(placa))
        {
            whereConditions.Add("v.Placa LIKE @Placa");
            parameters.Add("Placa", $"%{placa}%");
        }

        if (!string.IsNullOrWhiteSpace(marca))
        {
            whereConditions.Add("v.Marca LIKE @Marca");
            parameters.Add("Marca", $"%{marca}%");
        }

        if (!string.IsNullOrWhiteSpace(modelo))
        {
            whereConditions.Add("v.Modelo LIKE @Modelo");
            parameters.Add("Modelo", $"%{modelo}%");
        }



        if (clienteId.HasValue)
        {
            whereConditions.Add("v.ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("v.Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "placa" => "v.Placa",
            "marca" => "v.Marca",
            "modelo" => "v.Modelo",
            "anofabricacao" => "v.AnoFabricacao", // Ajustado de "ano"
            "cor" => "v.Cor",
            "cliente" => "c.NomeRazaoSocial", // Ajustado para o nome correto do campo em Cliente
            "datacriacao" => "v.DataCriacao",
            "dataatualizacao" => "v.DataAtualizacao",
            _ => "v.Placa"
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT v.*, c.*
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        var veiculoDict = new Dictionary<int, Veiculo>();
        
        var result = await connection.QueryAsync<Veiculo, Cliente, Veiculo>(
            sql,
            (veiculo, cliente) =>
            {
                if (!veiculoDict.TryGetValue(veiculo.Id, out var veiculoEntry))
                {
                    veiculoEntry = veiculo;
                    veiculoEntry.Cliente = cliente;
                    veiculoDict.Add(veiculo.Id, veiculoEntry);
                }
                return veiculoEntry;
            },
            parameters,
            splitOn: "Id"
        );
        
        return result.Distinct();
    }

    public async Task<int> ContarPorFiltroAsync(
        string? placa = null,
        string? modelo = null,
        string? marca = null,
        int? clienteId = null,
        bool? ativo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(placa))
        {
            whereConditions.Add("v.Placa LIKE @Placa");
            parameters.Add("Placa", $"%{placa}%");
        }

        if (!string.IsNullOrWhiteSpace(marca))
        {
            whereConditions.Add("v.Marca LIKE @Marca");
            parameters.Add("Marca", $"%{marca}%");
        }

        if (!string.IsNullOrWhiteSpace(modelo))
        {
            whereConditions.Add("v.Modelo LIKE @Modelo");
            parameters.Add("Modelo", $"%{modelo}%");
        }

        if (clienteId.HasValue)
        {
            whereConditions.Add("v.ClienteId = @ClienteId");
            parameters.Add("ClienteId", clienteId.Value);
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("v.Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var sql = $@"
            SELECT COUNT(*) 
            FROM Veiculos v
            INNER JOIN Clientes c ON v.ClienteId = c.Id
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<Veiculo>> ObterParaSelectAsync(int? clienteId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        if (clienteId.HasValue)
        {
            var sql = "SELECT Id, Placa, Marca, Modelo FROM Veiculos WHERE ClienteId = @ClienteId AND Ativo = 1 ORDER BY Placa";
            return await connection.QueryAsync<Veiculo>(sql, new { ClienteId = clienteId.Value });
        }
        else
        {
            var sql = "SELECT Id, Placa, Marca, Modelo FROM Veiculos WHERE Ativo = 1 ORDER BY Placa";
            return await connection.QueryAsync<Veiculo>(sql);
        }
    }

    public async Task<IEnumerable<Veiculo>> ObterPorClienteParaSelectAsync(int clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Id, Placa, Marca, Modelo FROM Veiculos WHERE ClienteId = @ClienteId AND Ativo = 1 ORDER BY Placa";
        return await connection.QueryAsync<Veiculo>(sql, new { ClienteId = clienteId });
    }
}