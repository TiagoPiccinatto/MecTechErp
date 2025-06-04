using System.Data;

namespace MecTecERP.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<IDbConnection> CreateConnectionAsync();
}