using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class FornecedorRepository : BaseRepository<Fornecedor>, IFornecedorRepository
{
    protected override string TableName => "Fornecedores";
    
    protected override string InsertQuery => @"
        INSERT INTO Fornecedores (RazaoSocial, NomeFantasia, Cnpj, InscricaoEstadual,
                                 Telefone1, Telefone2, Email, Cep, Logradouro, Numero, Complemento, Bairro, Cidade, Uf,
                                 NomeContato, TelefoneContato, EmailContato, Observacoes,
                                 DataCriacao, DataAtualizacao, Ativo, UsuarioCriacao, UsuarioAtualizacao)
        VALUES (@RazaoSocial, @NomeFantasia, @Cnpj, @InscricaoEstadual,
                @Telefone1, @Telefone2, @Email, @Cep, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Uf,
                @NomeContato, @TelefoneContato, @EmailContato, @Observacoes,
                @DataCriacao, @DataAtualizacao, @Ativo, @UsuarioCriacao, @UsuarioAtualizacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Fornecedores 
        SET RazaoSocial = @RazaoSocial,
            NomeFantasia = @NomeFantasia,
            Cnpj = @Cnpj,
            InscricaoEstadual = @InscricaoEstadual,
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
            NomeContato = @NomeContato,
            TelefoneContato = @TelefoneContato,
            EmailContato = @EmailContato,
            Observacoes = @Observacoes,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao,
            Ativo = @Ativo
        WHERE Id = @Id";

    public FornecedorRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<bool> ExisteCnpjAsync(string cnpj, int? idExcluir = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Fornecedores WHERE Cnpj = @Cnpj";
        object parameters;
        
        if (idExcluir.HasValue)
        {
            sql += " AND Id != @IdExcluir";
            parameters = new { Cnpj = cnpj, IdExcluir = idExcluir.Value };
        }
        else
        {
            parameters = new { Cnpj = cnpj };
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count > 0;
    }

    public async Task<bool> ExisteEmailAsync(string email, int? idExcluir = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Fornecedores WHERE LOWER(Email) = LOWER(@Email)";
        object parameters;
        
        if (idExcluir.HasValue)
        {
            sql += " AND Id != @IdExcluir";
            parameters = new { Email = email, IdExcluir = idExcluir.Value };
        }
        else
        {
            parameters = new { Email = email };
        }
        
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count > 0;
    }

    public async Task<IEnumerable<Fornecedor>> ObterAtivosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT * FROM Fornecedores WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Fornecedor>(sql);
    }

    public async Task<IEnumerable<Fornecedor>> ObterPorFiltroAsync(
        string? nome = null,
        string? cnpj = null,
        string? email = null,
        string? cidade = null,
        string? estado = null,
        bool? ativo = null,
        int pagina = 1,
        int tamanhoPagina = 10,
        string ordenarPor = "Nome",
        bool ordenarDesc = false)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome)) // nome pode ser RazaoSocial ou NomeFantasia
        {
            whereConditions.Add("(RazaoSocial LIKE @Nome OR NomeFantasia LIKE @Nome)");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            whereConditions.Add("Cnpj LIKE @Cnpj");
            parameters.Add("Cnpj", $"%{cnpj}%");
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            whereConditions.Add("Email LIKE @Email");
            parameters.Add("Email", $"%{email}%");
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            whereConditions.Add("Cidade LIKE @Cidade");
            parameters.Add("Cidade", $"%{cidade}%");
        }

        if (!string.IsNullOrWhiteSpace(estado)) // estado agora é Uf
        {
            whereConditions.Add("Uf LIKE @Uf");
            parameters.Add("Uf", $"%{estado}%"); // Mantendo parametro 'estado' por compatibilidade da interface, mas usando Uf
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "razaosocial" => "RazaoSocial", // Ajustado
            "nome" => "RazaoSocial", // Alias
            "cnpj" => "Cnpj",
            "email" => "Email",
            "cidade" => "Cidade",
            "uf" => "Uf", // Ajustado de "estado"
            "datacriacao" => "DataCriacao",
            "ativo" => "Ativo",
            _ => "RazaoSocial" // Ajustado Default
        };

        var offset = (pagina - 1) * tamanhoPagina;
        
        var sql = $@"
            SELECT * FROM Fornecedores 
            WHERE {string.Join(" AND ", whereConditions)}
            ORDER BY {orderColumn} {orderDirection}
            OFFSET @Offset ROWS FETCH NEXT @TamanhoPagina ROWS ONLY";
        
        parameters.Add("Offset", offset);
        parameters.Add("TamanhoPagina", tamanhoPagina);

        return await connection.QueryAsync<Fornecedor>(sql, parameters);
    }

    public async Task<int> ContarPorFiltroAsync(
        string? nome = null,
        string? cnpj = null,
        string? email = null,
        string? cidade = null,
        string? estado = null,
        bool? ativo = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        var whereConditions = new List<string> { "1=1" };
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(nome)) // nome pode ser RazaoSocial ou NomeFantasia
        {
            whereConditions.Add("(RazaoSocial LIKE @Nome OR NomeFantasia LIKE @Nome)");
            parameters.Add("Nome", $"%{nome}%");
        }

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            whereConditions.Add("Cnpj LIKE @Cnpj");
            parameters.Add("Cnpj", $"%{cnpj}%");
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            whereConditions.Add("Email LIKE @Email");
            parameters.Add("Email", $"%{email}%");
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            whereConditions.Add("Cidade LIKE @Cidade");
            parameters.Add("Cidade", $"%{cidade}%");
        }

        if (!string.IsNullOrWhiteSpace(estado)) // estado agora é Uf
        {
            whereConditions.Add("Uf LIKE @Uf");
            parameters.Add("Uf", $"%{estado}%"); // Mantendo parametro 'estado' por compatibilidade da interface, mas usando Uf
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var sql = $@"
            SELECT COUNT(*) FROM Fornecedores 
            WHERE {string.Join(" AND ", whereConditions)}";
        
        return await connection.QuerySingleAsync<int>(sql, parameters);
    }

    public async Task<IEnumerable<Fornecedor>> ObterParaSelectAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT Id, Nome FROM Fornecedores WHERE Ativo = 1 ORDER BY Nome"; // Deveria ser RazaoSocial ou NomeFantasia
        return await connection.QueryAsync<Fornecedor>(sql);
    }

    public async Task<bool> PossuiProdutosAsync(int fornecedorId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Produtos WHERE FornecedorId = @FornecedorId AND Ativo = 1";
        var count = await connection.QuerySingleAsync<int>(sql, new { FornecedorId = fornecedorId });
        return count > 0;
    }
}