using Dapper;
using MecTecERP.Domain.Entities;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;

namespace MecTecERP.Infrastructure.Repositories;

public class FornecedorRepository : BaseRepository<Fornecedor>, IFornecedorRepository
{
    protected override string TableName => "Fornecedores";
    
    protected override string InsertQuery => @"
        INSERT INTO Fornecedores (Nome, Cnpj, Email, Telefone, Endereco, Numero, 
                                 Complemento, Bairro, Cidade, Estado, Cep, 
                                 Observacoes, DataCriacao, DataAtualizacao, 
                                 Ativo, UsuarioCriacao)
        VALUES (@Nome, @Cnpj, @Email, @Telefone, @Endereco, @Numero, 
                @Complemento, @Bairro, @Cidade, @Estado, @Cep, 
                @Observacoes, @DataCriacao, @DataAtualizacao, 
                @Ativo, @UsuarioCriacao);
        SELECT CAST(SCOPE_IDENTITY() as int);";
    
    protected override string UpdateQuery => @"
        UPDATE Fornecedores 
        SET Nome = @Nome, 
            Cnpj = @Cnpj,
            Email = @Email,
            Telefone = @Telefone,
            Endereco = @Endereco,
            Numero = @Numero,
            Complemento = @Complemento,
            Bairro = @Bairro,
            Cidade = @Cidade,
            Estado = @Estado,
            Cep = @Cep,
            Observacoes = @Observacoes,
            DataAtualizacao = @DataAtualizacao, 
            UsuarioAtualizacao = @UsuarioAtualizacao
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

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
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

        if (!string.IsNullOrWhiteSpace(estado))
        {
            whereConditions.Add("Estado LIKE @Estado");
            parameters.Add("Estado", $"%{estado}%");
        }

        if (ativo.HasValue)
        {
            whereConditions.Add("Ativo = @Ativo");
            parameters.Add("Ativo", ativo.Value);
        }

        var orderDirection = ordenarDesc ? "DESC" : "ASC";
        var orderColumn = ordenarPor.ToLower() switch
        {
            "nome" => "Nome",
            "cnpj" => "Cnpj",
            "email" => "Email",
            "cidade" => "Cidade",
            "estado" => "Estado",
            "datacriacao" => "DataCriacao",
            "ativo" => "Ativo",
            _ => "Nome"
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

        if (!string.IsNullOrWhiteSpace(nome))
        {
            whereConditions.Add("Nome LIKE @Nome");
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

        if (!string.IsNullOrWhiteSpace(estado))
        {
            whereConditions.Add("Estado LIKE @Estado");
            parameters.Add("Estado", $"%{estado}%");
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
        var sql = "SELECT Id, Nome FROM Fornecedores WHERE Ativo = 1 ORDER BY Nome";
        return await connection.QueryAsync<Fornecedor>(sql);
    }
}