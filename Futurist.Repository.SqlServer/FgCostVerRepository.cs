using System.Data;
using System.Reflection;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.FgCostVerCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class FgCostVerRepository : IFgCostVerRepository
{
    private readonly IDbConnection _dbConnection;

    public FgCostVerRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<FgCostVerSp>> GetAllFgCostVerAsync(GetAllFgCostVerCommand command)
    {
        const string query = "EXEC FgCostVer_Select_new @Room";
        
        Dictionary<string, string> columnMappings = new()
        {
            { "Room", nameof(FgCostVerSp.Room) },
            { "RofoDate" , nameof(FgCostVerSp.RofoDate) },
            { "ItemId", nameof(FgCostVerSp.ItemId) },
            { "ItemName", nameof(FgCostVerSp.ItemName) },
            { "In Kg", nameof(FgCostVerSp.InKg) },
            { "SalesPrice", nameof(FgCostVerSp.SalesPrice) },
            { "RofoQty", nameof(FgCostVerSp.RofoQty) },
            { "RmPrice", nameof(FgCostVerSp.RmPrice) },
            { "PmPrice", nameof(FgCostVerSp.PmPrice) },
            { "StdCostPrice", nameof(FgCostVerSp.StdCostPrice) },
            { "Yield", nameof(FgCostVerSp.Yield) },
            { "RmPm+Y", nameof(FgCostVerSp.CostRmPmY) },
            { "CostPrice", nameof(FgCostVerSp.CostPrice) },
            { "Previous Calc", nameof(FgCostVerSp.PreviousCalc) },
            { "SalesPrice Prev", nameof(FgCostVerSp.SalesPricePrev) },
            { "RofoQty Prev", nameof(FgCostVerSp.RofoQtyPrev) },
            { "Rm Prev", nameof(FgCostVerSp.RmPrev) },
            { "Pm Prev", nameof(FgCostVerSp.PmPrev) },
            { "StdCost Prev", nameof(FgCostVerSp.StdCostPrev) },
            { "Yield Prev", nameof(FgCostVerSp.YieldPrev) },
            { "RmPm+Y Prev", nameof(FgCostVerSp.CostRmPmYPrev) },
            { "CostPrice Prev", nameof(FgCostVerSp.CostPricePrev) },
            { "Delta Absolute", nameof(FgCostVerSp.DeltaAbsolute) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(FgCostVerSp), new CustomPropertyTypeMap(
            typeof(FgCostVerSp),
            (type, columnName) => mapper(type, columnName)!));

        var parameters = new { command.Room };

        var result = await _dbConnection.QueryAsync<FgCostVerSp>(query, parameters);
        return result;
    }

    public async Task<SpTask?> InsertFgCostVerAsync(InsertFgCostVerCommand command)
    {
        const string query = "EXEC Version_Insert @Room, @Notes";
        return await _dbConnection.QuerySingleOrDefaultAsync<SpTask>(query,
            new { command.Room, command.Notes },
            command.DbTransaction);
    }

    public async Task<IEnumerable<int>> GetFgCostVerRoomIdsAsync(GetFgCostVerRoomIdsCommand command)
    {
        const string query = """
                             SELECT DISTINCT Room FROM FgCost
                             ORDER BY Room;
                             """;
        var result = await _dbConnection.QueryAsync<int>(query);
        return result;
    }
}