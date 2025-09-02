using System.Data;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.CurrentDataCommands;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class CurrentDataRepository : ICurrentDataRepository
{
    private readonly IDbConnection _sqlConnection;

    public CurrentDataRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<ItemOnHand>> GetItemOnHandAsync(GetItemOnHandCommand command, CancellationToken cancellationToken)
    {
        const string query = "ItemOnhand_Select";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<ItemOnHand>(sqlCommand);
    }

    public async Task<IEnumerable<ItemPoIntransit>> GetItemPoIntransitAsync(GetItemPoIntransitCommand command, CancellationToken cancellationToken)
    {
        const string query = "ItemPoIntransit_Select";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<ItemPoIntransit>(sqlCommand);
    }

    public async Task<IEnumerable<ItemPag>> GetItemPagAsync(GetItemPagCommand command, CancellationToken cancellationToken)
    {
        const string query = "ItemPag_Select";
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<ItemPag>(sqlCommand);
    }
}