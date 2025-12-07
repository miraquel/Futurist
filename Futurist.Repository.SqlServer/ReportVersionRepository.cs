using System.Data;
using System.Reflection;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.ReportVersionCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class ReportVersionRepository : IReportVersionRepository
{
    private readonly IDbConnection _dbConnection;

    public ReportVersionRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<FgCostVerSp>> GetAllFgCostVerAsync(GetAllFgCostVerCommand command)
    {
        const string storedProcedure = "FgCostVer_Select";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        parameters.Add("@VerId", command.VerId, DbType.Int32);
        FgCostVerSpMapping();
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QueryAsync<FgCostVerSp>(storedProcedure, parameters, commandType: CommandType.StoredProcedure, transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<FgCostVerDetailSp>> GetAllFgCostVerDetailsByRofoIdAsync(GetAllFgCostVerDetailsCommand command)
    {
        const string storedProcedure = "FgCostVerDetail_Select_ByRofoId";
        var parameters = new DynamicParameters();
        parameters.Add("@RofoId", command.RofoId, DbType.Int32);
        parameters.Add("@VerId", command.VerId, DbType.Int32);
        FgCostVerDetailSpMapping();
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QueryAsync<FgCostVerDetailSp>(storedProcedure, parameters, command.DbTransaction, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<MupVerSp>> GetAllMupVerAsync(GetAllMupVerCommand command)
    {
        const string storedProcedure = "MupVerSelect_Det";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        parameters.Add("@VerId", command.VerId, DbType.Int32);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QueryAsync<MupVerSp>(storedProcedure, parameters, command.DbTransaction, commandType: CommandType.StoredProcedure);
    }

    public async Task<SpTask?> InsertVersionAsync(InsertVersionCommand command)
    {
        const string storedProcedure = "Version_Insert";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        parameters.Add("@Notes", command.Notes, DbType.String);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QuerySingleOrDefaultAsync<SpTask>(storedProcedure, parameters, command.DbTransaction, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<int>> GetVersionRoomIdsAsync(GetVersionRoomIdsCommand command)
    {
        // const string storedProcedure = "Version_Select_Room";
        // await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        // return await _dbConnection.QueryAsync<int>(storedProcedure, command.DbTransaction, commandType: CommandType.StoredProcedure);
        
        const string query = """
                             SELECT DISTINCT Room FROM FgCost
                             UNION
                             SELECT DISTINCT Room FROM Mup
                             UNION
                             SELECT DISTINCT Room FROM BomStd
                             UNION
                             SELECT DISTINCT Room FROM Rofo
                             ORDER BY Room;
                             """;
        
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QueryAsync<int>(query, transaction: command.DbTransaction);
    }

    public async Task<IEnumerable<Versions>> GetVersionsAsync(GetVersionsCommand command)
    {
        const string storedProcedure = "Version_Select_Version";
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room, DbType.Int32);
        await _dbConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        return await _dbConnection.QueryAsync<Versions>(storedProcedure, parameters, command.DbTransaction, commandType: CommandType.StoredProcedure);
    }
    
    private static void FgCostVerSpMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "Previous Calc", nameof(FgCostVerSp.PreviousCalc) }, 
            { "RmPm+Y", nameof(FgCostVerSp.CostRmPmY) },
            { "RofoQty Prev", nameof(FgCostVerSp.RofoQtyPrev) },
            { "Rm Prev", nameof(FgCostVerSp.RmPrev) },
            { "Pm Prev", nameof(FgCostVerSp.PmPrev) },
            { "StdCost Prev", nameof(FgCostVerSp.StdCostPrev) },
            { "Yield Prev", nameof(FgCostVerSp.YieldPrev) },
            { "RmPm+Y Prev", nameof(FgCostVerSp.CostRmPmYPrev) },
            { "CostPrice Prev", nameof(FgCostVerSp.CostPricePrev) },
            { "Delta Absolute", nameof(FgCostVerSp.DeltaAbsolute) },
            { "Ratio RMPM/S", nameof(FgCostVerSp.RatioRmPmToSalesPrice) },
            { "Ratio CP/S", nameof(FgCostVerSp.RatioCostPriceToSalesPrice) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(FgCostVerSp), new CustomPropertyTypeMap(
            typeof(FgCostVerSp),
            (type, columnName) => mapper(type, columnName)!));
    }
    
    private static void FgCostVerDetailSpMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "UNITID", nameof(FgCostVerDetailSp.UnitId) },
            { "QtyRofo", nameof(FgCostVerDetailSp.RofoQty) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(FgCostVerDetailSp), new CustomPropertyTypeMap(
            typeof(FgCostVerDetailSp),
            (type, columnName) => mapper(type, columnName)!));
    }
}