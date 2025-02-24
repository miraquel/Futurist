using System.Data;
using Dapper;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class CommonRepository : ICommonRepository
{
    private readonly IDbTransaction _transaction;
    private readonly IDbConnection _connection;

    public CommonRepository(IDbTransaction transaction, IDbConnection connection)
    {
        _transaction = transaction;
        _connection = connection;
    }

    public async Task<int> GetLastInsertedIdAsync()
    {
        return await _connection.ExecuteScalarAsync<int>("SELECT SCOPE_IDENTITY()", transaction: _transaction);
    }
}