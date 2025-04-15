using System.Data;
using Dapper;
using Futurist.Common.Helpers;
using Futurist.Domain;
using Futurist.Domain.Common;
using Futurist.Repository.Command.JobMonitoringCommand;
using Futurist.Repository.Interface;

namespace Futurist.Repository.SqlServer;

public class JobMonitoringRepository : IJobMonitoringRepository
{
    private readonly IDbConnection _connection;

    public JobMonitoringRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<PagedList<JobMonitoring>> GetJobMonitoringPagedListAsync(GetJobMonitoringPagedListCommand command)
    {
        // group by JobId and retrieve the latest log for each column
        const string sql = """
                           WITH AggregatedTimes AS (
                               SELECT
                                    JobId,
                                    MIN(TimeStamp) AS TimeStart,
                                    MAX(TimeStamp) AS TimeEnd
                               FROM JobLogs
                               /**groupby**/
                           ),
                           LatestJobLogs AS (
                               SELECT 
                                    jl.*,
                                    ROW_NUMBER() OVER (PARTITION BY jl.JobId ORDER BY jl.TimeStamp DESC) AS rn
                               FROM JobLogs jl
                           )
                           SELECT
                               LJL.[Message],
                               LJL.[MessageTemplate],
                               LJL.[Level],
                               A.TimeStart,
                               A.TimeEnd,
                               LJL.[Exception],
                               LJL.[Properties],
                               LJL.[SourceContext],
                               LJL.[Status],
                               LJL.[JobId]
                           FROM LatestJobLogs LJL
                           INNER JOIN AggregatedTimes A ON LJL.JobId = A.JobId
                           /**where**/
                           /**orderby**/
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                           """;
        
        const string sqlCount = """
                                WITH AggregatedTimes AS (
                                    SELECT
                                         JobId,
                                         MIN(TimeStamp) AS TimeStart,
                                         MAX(TimeStamp) AS TimeEnd
                                    FROM JobLogs
                                    GROUP BY JobId
                                ),
                                LatestJobLogs AS (
                                    SELECT 
                                         jl.*,
                                         ROW_NUMBER() OVER (PARTITION BY jl.JobId ORDER BY jl.TimeStamp DESC) AS rn
                                    FROM JobLogs jl
                                )
                                SELECT COUNT(*)
                                FROM LatestJobLogs LJL
                                INNER JOIN AggregatedTimes A ON LJL.JobId = A.JobId
                                /**where**/
                                """;
        
        var pagedListRequest = command.PagedListRequest;

        var sqlBuilder = new SqlBuilder();

        sqlBuilder.Where("LJL.rn = 1");
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.JobId), out var jobIdFilter))
        {
            if (int.TryParse(jobIdFilter, out var jobId))
            {
                sqlBuilder.Where($"LJL.JobId = @JobId", new { JobId = jobId });
            }
            else
            {
                var match = RegexHelper.LogicalOperatorRegex().Match(jobIdFilter);
                if (match.Success)
                {
                    sqlBuilder.Where($"LJL.JobId {match.Groups[1].Value} @JobId", new { Id = match.Groups[2].Value });
                }
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.Message), out var messageFilter))
        {
            if (messageFilter.Contains('*') | messageFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.Message LIKE @Message", new { Message = messageFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.Message = @Message", new { Message = messageFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.Level), out var levelFilter))
        {
            if (levelFilter.Contains('*') | levelFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.Level LIKE @Level", new { Level = levelFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.Level = @Level", new { Level = levelFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.TimeStart), out var timeStartFilter) && DateTime.TryParse(timeStartFilter, out var timeStart))
        {
            sqlBuilder.Where($"A.TimeStart = @TimeStart", new { TimeStart = timeStart });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.TimeEnd), out var timeEndFilter) && DateTime.TryParse(timeEndFilter, out var timeEnd))
        {
            sqlBuilder.Where($"A.TimeEnd = @TimeEnd", new { TimeEnd = timeEnd });
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.Exception), out var exceptionFilter))
        {
            if (exceptionFilter.Contains('*') | exceptionFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.Exception LIKE @Exception", new { Exception = exceptionFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.Exception = @Exception", new { Exception = exceptionFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.Properties), out var propertiesFilter))
        {
            if (propertiesFilter.Contains('*') | propertiesFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.Properties LIKE @Properties", new { Properties = propertiesFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.Properties = @Properties", new { Properties = propertiesFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.SourceContext), out var sourceContextFilter))
        {
            if (sourceContextFilter.Contains('*') | sourceContextFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.SourceContext LIKE @SourceContext", new { SourceContext = sourceContextFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.SourceContext = @SourceContext", new { SourceContext = sourceContextFilter });
            }
        }
        
        if (pagedListRequest.Filters.TryGetValue(nameof(JobMonitoring.Status), out var statusFilter))
        {
            if (statusFilter.Contains('*') | statusFilter.Contains('%'))
            {
                sqlBuilder.Where($"LJL.Status LIKE @Status", new { Status = statusFilter.Replace('*', '%') });
            }
            else
            {
                sqlBuilder.Where($"LJL.Status = @Status", new { Status = statusFilter });
            }
        }
        
        sqlBuilder.GroupBy("JobId");
        
        var sort = pagedListRequest.IsSortAscending ? "ASC" : "DESC";
        sqlBuilder.OrderBy(string.IsNullOrEmpty(pagedListRequest.SortBy)
            ? $"Id {sort}"
            : $"{pagedListRequest.SortBy} {sort}");
        
        sqlBuilder.AddParameters(new
            { Offset = (pagedListRequest.PageNumber - 1) * pagedListRequest.PageSize, pagedListRequest.PageSize });
        
        var template = sqlBuilder.AddTemplate(sql);
        var logs = await _connection.QueryAsync<JobMonitoring>(template.RawSql, template.Parameters);
        template = sqlBuilder.AddTemplate(sqlCount);
        var count = await _connection.ExecuteScalarAsync<int>(sqlCount, template.Parameters);
        return new PagedList<JobMonitoring>(logs, pagedListRequest.PageNumber, pagedListRequest.PageSize, count);
    }

    public async Task<JobMonitoring?> GetJobMonitoringAsync(GetJobMonitoringCommand command)
    {
        const string sql = """
                           SELECT * FROM Logs
                           WHERE Id = @Id
                           """;
        
        return await _connection.QueryFirstOrDefaultAsync<JobMonitoring>(sql, new { command.Id });
    }
}