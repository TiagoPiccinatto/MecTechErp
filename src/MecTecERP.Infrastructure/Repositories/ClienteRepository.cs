using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    protected override string TableName => "Clientes";
    
    protected override string InsertQuery => @"
        INSERT INTO Clientes (TipoPessoa, NomeRazaoSocial, CpfCnpj, RgIe, Telefone1, Telefone2, Email,
                             Cep, Logradouro, Numero, Complemento, Bairro, Cidade, Uf, Observacoes,
                             DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe, @Telefone1, @Telefone2, @Email,
                @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Uf, @Observacoes,
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Clientes 
        SET TipoPessoa = @TipoPessoa,
            NomeRazaoSocial = @NomeRazaoSocial,
            CpfCnpj = @CpfCnpj,
            RgIe = @RgIe,
            Telefone1 = @Telefone1,
            Telefone2 = @Telefone2,
            Email = @Email,
            Cep = @Cep,
            Logradouro = @Logradouro,
            Numero = @Numero,
            Complemento = @Complemento,
            Bairro = @Bairro,
            Cidade = @Cidade,
            Uf = @Uf,
            Observacoes = @Observacoes,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao,
            Ativo = @Ativo
        WHERE Id = @Id";

    public ClienteRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<bool> ExisteCpfCnpjAsync(string cpfCnpj, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COUNT(1) FROM Clientes WHERE CpfCnpj = @CpfCnpj";
        
        if (excludeId.HasValue)
        {
            sql += " AND Id != @ExcludeId";
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, new { CpfCnpj = cpfCnpj, ExcludeId = excludeId });
        return count > 0;
    }

    public async Task<bool> ExisteEmailAsync(string email, int? excludeId = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var sql = "SELECT COUNT(1) FROM Clientes WHERE Email = @Email";
        
        if (excludeId.HasValue)
        {
            sql += " AND Id != @ExcludeId";
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, new { Email = email, ExcludeId = excludeId });
        return count > 0;
    }

    public async Task<Cliente?> ObterPorCpfCnpjAsync(string cpfCnpj)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Clientes WHERE CpfCnpj = @CpfCnpj";
        return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { CpfCnpj = cpfCnpj });
    }

    public async Task<Cliente?> ObterPorEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Clientes WHERE Email = @Email";
        return await connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Email = email });
    }

    public async Task<IEnumerable<Cliente>> ObterAtivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Clientes WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Cliente>(sql);
    }

    public async Task<bool> PossuiOrdensServicoAsync(int clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM OrdensServico WHERE ClienteId = @ClienteId";
        var count = await connection.QuerySingleAsync<int>(sql, new { ClienteId = clienteId });
        return count > 0;
    }

    public async Task<IEnumerable<Cliente>> ObterPorFiltroAsync(
        string? nome = null,
        string? cpfCnpj = null,
        string? email = null,
        string? telefone = null,
        string? cidade = null,
        bool? ativo = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "Nome",
        bool ordenarDesc = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome)) // nome aqui se refere a NomeRazaoSocial
        {
            whereConditions.Add("NomeRazaoSocial LIKE @NomeRazaoSocial");
            parameters.Add("NomeRazaoSocial", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(cpfCnpj))
        {
            whereConditions.Add("CpfCnpj LIKE @CpfCnpj");
            parameters.Add("CpfCnpj", $"%{cpfCnpj}%");
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            whereConditions.Add("Email LIKE @Email");
            parameters.Add("Email", $"%{email}%");
        }

        if (!string.IsNullOrWhiteSpace(telefone)) // telefone aqui pode ser Telefone1 ou Telefone2
        {
            whereConditions.Add("(Telefone1 LIKE @Telefone OR Telefone2 LIKE @Telefone)");
            parameters.Add("Telefone", $"%{telefone}%");
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            whereConditions.Add("Cidade LIKE @Cidade");
            parameters.Add("Cidade", $"%{cidade}%");
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "nomerazaosocial" => "NomeRazaoSocial", // Ajustado
            "nome" => "NomeRazaoSocial", // Alias para compatibilidade
            "cpfcnpj" => "CpfCnpj",
            "email" => "Email",
            "cidade" => "Cidade",
            "uf" => "Uf", // Ajustado de "estado"
            "datacriacao" => "DataCriacao",
            "dataatualizacao" => "DataAtualizacao",
            _ => "NomeRazaoSocial" // Ajustado Default
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT * FROM Clientes 
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        return await connection.QueryAsync<Cliente>(sql, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? nome = null,
        string? cpfCnpj = null,
        string? email = null,
        string? telefone = null,
        string? cidade = null,
        bool? ativo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome)) // nome aqui se refere a NomeRazaoSocial
        {
            whereConditions.Add("NomeRazaoSocial LIKE @NomeRazaoSocial");
            parameters.Add("NomeRazaoSocial", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(cpfCnpj))
        {
            whereConditions.Add("CpfCnpj LIKE @CpfCnpj");
            parameters.Add("CpfCnpj", $"%{cpfCnpj}%");
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            whereConditions.Add("Email LIKE @Email");
            parameters.Add("Email", $"%{email}%");
        }

        if (!string.IsNullOrWhiteSpace(telefone)) // telefone aqui pode ser Telefone1 ou Telefone2
        {
            whereConditions.Add("(Telefone1 LIKE @Telefone OR Telefone2 LIKE @Telefone)");
            parameters.Add("Telefone", $"%{telefone}%");
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            whereConditions.Add("Cidade LIKE @Cidade");
            parameters.Add("Cidade", $"%{cidade}%");
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var sql = $"SELECT COUNT(*) FROM Clientes WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<Cliente>> ObterComVeiculosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            SELECT DISTINCT c.* 
            FROM Clientes c
            INNER JOIN Veiculos v ON c.Id = v.ClienteId
            WHERE c.Ativo = 1 AND v.Ativo = 1
            ORDER BY c.Nome";
        
        return await connection.QueryAsync<Cliente>(sql);
    }

    public async Task<IEnumerable<Cliente>> ObterParaSelectAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Id, Nome FROM Clientes WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Cliente>(sql);
    }
}