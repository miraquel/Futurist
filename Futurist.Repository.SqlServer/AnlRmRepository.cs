using System.Data;
using System.Reflection;
using Dapper;
using Futurist.Domain;
using Futurist.Repository.Command.AnlRmCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class AnlRmRepository : IAnlRmRepository
{
    private readonly IDbConnection _sqlConnection;

    public AnlRmRepository(IDbConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<AnlRmPrice>> GetAnlRmpPrice(GetAnlRmPriceCommand command, CancellationToken cancellationToken)
    {
        const string query = "AnlRmPrice_plan_vs_actual";
        
        AnlRmPriceMapping();
        
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room);
        parameters.Add("@VerId", command.VerId);
        parameters.Add("@Year", command.Year);
        parameters.Add("@Month", command.Month);
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<AnlRmPrice>(sqlCommand);
    }

    public async Task<IEnumerable<AnlKurs>> GetAnlKursAsync(GetAnlKursCommand command, CancellationToken cancellationToken)
    {
        const string query = "AnlKurs_plan_vs_actual";
        
        AnlKursMapping();
        
        var parameters = new DynamicParameters();
        parameters.Add("@Version", command.Version);
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<AnlKurs>(sqlCommand);
    }

    public async Task<IEnumerable<AnlFgPrice>> GetAnlFgPriceAsync(GetAnlFgPriceCommand command, CancellationToken cancellationToken)
    {
        const string query = "AnlFgPrice_plan_vs_actual";
        
        AnlFgPriceMapping();
        
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room);
        parameters.Add("@VerId", command.VerId);
        parameters.Add("@Year", command.Year);
        parameters.Add("@Month", command.Month);
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<AnlFgPrice>(sqlCommand);
    }

    public async Task<IEnumerable<AnlPmPrice>> GetAnlPmPriceAsync(GetAnlPmPriceCommand command, CancellationToken cancellationToken)
    {
        const string query = "AnlPmPrice_plan_vs_actual";
        AnlPmPriceMapping();
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room);
        parameters.Add("@VerId", command.VerId);
        parameters.Add("@Year", command.Year);
        parameters.Add("@Month", command.Month);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<AnlPmPrice>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetRoomIdsAsync(GetRoomIdsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT Room 
                             FROM RofoVer
                             ORDER BY Room
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _sqlConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetRofoVerIdsAsync(GetRofoVerIdsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT VerId 
                             FROM RofoVer
                             WHERE Room = @Room
                             ORDER BY VerId
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            new { command.Room },
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _sqlConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetYearsAsync(GetYearsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT YEAR(RofoDate) AS Year
                             FROM RofoVer
                             WHERE Room = @Room AND VerId = @VerId
                             ORDER BY Year
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            new { command.Room, command.VerId },
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _sqlConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetMonthsAsync(GetMonthsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT MONTH(RofoDate) AS Month
                             FROM RofoVer
                             WHERE Room = @Room AND VerId = @VerId AND YEAR(RofoDate) = @Year
                             ORDER BY Month
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            new { command.Room, command.VerId, command.Year },
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _sqlConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<int>> GetVerIdsAsync(GetVerIdsCommand command, CancellationToken cancellationToken)
    {
        const string query = """
                             SELECT DISTINCT VerId
                             FROM Version
                             ORDER BY VerId
                             """;
        
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            command.DbTransaction,
            cancellationToken: cancellationToken);
        return await _sqlConnection.QueryAsync<int>(sqlCommand);
    }

    public async Task<IEnumerable<AnlRmPriceGroup>> GetAnlRmPriceGroupAsync(GetAnlRmPriceGroupCommand command, CancellationToken cancellationToken)
    {
        const string query = "AnlRmPriceGroup_plan_vs_actual";
        AnlRmPriceGroupMapping();
        var parameters = new DynamicParameters();
        parameters.Add("@Room", command.Room);
        parameters.Add("@VerId", command.VerId);
        parameters.Add("@Year", command.Year);
        parameters.Add("@Month", command.Month);
        await _sqlConnection.ExecuteAsync("SET ARITHABORT ON", transaction: command.DbTransaction);
        var sqlCommand = new CommandDefinition(
            query,
            parameters,
            command.DbTransaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure);
        return await _sqlConnection.QueryAsync<AnlRmPriceGroup>(sqlCommand);
    }

    private static void AnlRmPriceMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "A/P", nameof(AnlRmPrice.Ap) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(AnlRmPrice), new CustomPropertyTypeMap(
            typeof(AnlRmPrice),
            (type, columnName) => mapper(type, columnName)!));
    }
    
    private static void AnlKursMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "A/P", nameof(AnlKurs.Ap) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(AnlKurs), new CustomPropertyTypeMap(
            typeof(AnlKurs),
            (type, columnName) => mapper(type, columnName)!));
    }
    
    private static void AnlFgPriceMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "A/P", nameof(AnlFgPrice.Ap) }
        };

        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));

        SqlMapper.SetTypeMap(typeof(AnlFgPrice), new CustomPropertyTypeMap(
            typeof(AnlFgPrice),
            (type, columnName) => mapper(type, columnName)!));
    }
    
    private static void AnlPmPriceMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "A/P", nameof(AnlPmPrice.Ap) }
        };
        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));
        SqlMapper.SetTypeMap(typeof(AnlPmPrice), new CustomPropertyTypeMap(
            typeof(AnlPmPrice),
            (type, columnName) => mapper(type, columnName)!));
    }
    
    private static void AnlRmPriceGroupMapping()
    {
        Dictionary<string, string> columnMappings = new()
        {
            { "A/P", nameof(AnlRmPriceGroup.Ap) }
        };
        var mapper = new Func<Type, string, PropertyInfo?>((type, columnName) =>
            type.GetProperty(columnMappings.GetValueOrDefault(columnName, columnName)));
        SqlMapper.SetTypeMap(typeof(AnlRmPriceGroup), new CustomPropertyTypeMap(
            typeof(AnlRmPriceGroup),
            (type, columnName) => mapper(type, columnName)!));
    }
}