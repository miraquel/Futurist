using System.Data;
using Dapper;
using Futurist.Repository.Command.CommonCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class CommonRepository : ICommonRepository
{
    private readonly IDbConnection _connection;

    public CommonRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> GetLastInsertedIdAsync(GetLastInsertedIdCommand command)
    {
        return await _connection.ExecuteScalarAsync<int>("SELECT SCOPE_IDENTITY()", transaction: command.DbTransaction);
    }
}