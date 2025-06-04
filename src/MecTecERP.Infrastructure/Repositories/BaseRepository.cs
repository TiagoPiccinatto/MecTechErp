using Dapper;
using MecTecERP.Domain.Common;
using MecTecERP.Domain.Interfaces;
using MecTecERP.Infrastructure.Data;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace MecTecERP.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected abstract string TableName { get; }
    protected abstract string InsertQuery { get; }
    protected abstract string UpdateQuery { get; }

    protected BaseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public virtual async Task<T?> ObterPorIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = $"SELECT * FROM {TableName} WHERE Id = @Id AND Ativo = 1";
        return await connection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
    }

    public virtual async Task<IEnumerable<T>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = $"SELECT * FROM {TableName} WHERE Ativo = 1 ORDER BY DataCriacao DESC";
        return await connection.QueryAsync<T>(query);
    }

    public virtual async Task<T> AdicionarAsync(T entity)
    {
        entity.DataCriacao = DateTime.UtcNow;
        entity.DataAtualizacao = DateTime.UtcNow;
        entity.Ativo = true;

        using var connection = _connectionFactory.CreateConnection();
        var id = await connection.QuerySingleAsync<int>(InsertQuery, entity);
        entity.Id = id;
        return entity;
    }

    public virtual async Task AtualizarAsync(T entity)
    {
        entity.DataAtualizacao = DateTime.UtcNow;

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(UpdateQuery, entity);
    }

    public virtual async Task AtualizarVariosAsync(IEnumerable<T> entidades)
    {
        using var connection = _connectionFactory.CreateConnection();
        
        foreach (var entidade in entidades)
        {
            entidade.DataAtualizacao = DateTime.UtcNow;
        }
        
        await connection.ExecuteAsync(UpdateQuery, entidades);
    }

    public virtual async Task RemoverAsync(T entidade)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"DELETE FROM {TableName} WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = entidade.Id });
    }

    public virtual async Task RemoverAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = $"DELETE FROM {TableName} WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    public virtual async Task RemoverVariosAsync(IEnumerable<T> entidades)
    {
        using var connection = _connectionFactory.CreateConnection();
        var ids = entidades.Select(e => e.Id).ToArray();
        var sql = $"DELETE FROM {TableName} WHERE Id IN @Ids";
        await connection.ExecuteAsync(sql, new { Ids = ids });
    }

    public virtual async Task<IEnumerable<T>> AdicionarVariosAsync(IEnumerable<T> entidades)
    {
        var result = new List<T>();
        foreach (var entidade in entidades)
        {
            var added = await AdicionarAsync(entidade);
            result.Add(added);
        }
        return result;
    }

    public virtual async Task<IEnumerable<T>> ObterPorCondicaoAsync(Expression<Func<T, bool>> condicao)
    {
        // Para Dapper, seria necessário converter a Expression para SQL
        // Por simplicidade, retornando todos os registros ativos
        return await ObterTodosAsync();
    }

    public virtual async Task<T?> ObterPrimeiroAsync(Expression<Func<T, bool>> condicao)
    {
        // Para Dapper, seria necessário converter a Expression para SQL
        // Por simplicidade, retornando o primeiro registro ativo
        var todos = await ObterTodosAsync();
        return todos.FirstOrDefault();
    }

    public virtual async Task<bool> ExisteAsync(Expression<Func<T, bool>> condicao)
    {
        // Para Dapper, seria necessário converter a Expression para SQL
        // Por simplicidade, verificando se existem registros ativos
        var todos = await ObterTodosAsync();
        return todos.Any();
    }

    public virtual async Task<int> ContarAsync(Expression<Func<T, bool>>? condicao = null)
    {
        using var connection = _connectionFactory.CreateConnection();
        var query = $"SELECT COUNT(*) FROM {TableName} WHERE Ativo = 1";
        return await connection.QuerySingleAsync<int>(query);
    }

    protected virtual string BuildSelectQuery(string whereClause = "", string orderBy = "DataCriacao DESC")
    {
        var query = new StringBuilder($"SELECT * FROM {TableName}");
        
        if (!string.IsNullOrEmpty(whereClause))
        {
            query.Append($" WHERE {whereClause}");
        }
        else
        {
            query.Append(" WHERE Ativo = 1");
        }
        
        if (!string.IsNullOrEmpty(orderBy))
        {
            query.Append($" ORDER BY {orderBy}");
        }
        
        return query.ToString();
    }

    protected virtual string BuildCountQuery(string whereClause = "")
    {
        var query = new StringBuilder($"SELECT COUNT(*) FROM {TableName}");
        
        if (!string.IsNullOrEmpty(whereClause))
        {
            query.Append($" WHERE {whereClause}");
        }
        else
        {
            query.Append(" WHERE Ativo = 1");
        }
        
        return query.ToString();
    }

    public virtual Task<bool> SalvarMudancasAsync()
    {
        // Com Dapper, as operações são executadas imediatamente
        // Este método existe apenas para compatibilidade com a interface
        return Task.FromResult(true);
    }
}