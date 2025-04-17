using Futurist.Service.Dto;
using Futurist.Service.Dto.Common;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Futurist.Web.Hangfire;

public class JobLoggerAttribute : JobFilterAttribute, IElectStateFilter, IServerFilter
{
    private readonly ILogger _logger = Log.ForContext<JobLoggerAttribute>();

    public void OnStateElection(ElectStateContext context)
    {
        if (context.BackgroundJob.Job.Method.Name.Contains("Notify"))
            return;

        if (context.CandidateState is EnqueuedState)
        {
            _logger.Information("JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, context.BackgroundJob.Job.Method.Name, context.BackgroundJob.Job.Args[0], "Enqueued", "Waiting for processing");
        }
    }

    public void OnPerforming(PerformingContext context)
    {
        if (context.BackgroundJob.Job.Method.Name.Contains("Notify"))
            return;
        
        var methodName = context.BackgroundJob.Job.Method.Name;
        var roomId = context.BackgroundJob.Job.Args[0];
        
        _logger.Information("JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Started", "Processing");
    }

    public void OnPerformed(PerformedContext context)
    {
        // Skip logging for notification jobs
        if (context.BackgroundJob.Job.Method.Name.Contains("Notify"))
            return;

        var methodName = context.BackgroundJob.Job.Method.Name;
        var roomId = context.BackgroundJob.Job.Args[0];

        switch (context.Result)
        {
            case ServiceResponse<SpTaskDto> resultTaskSp when context.Exception != null:
                _logger.Error(context.Exception, "JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Failed", string.Join(", ", resultTaskSp.Errors));
                break;
            // map the context.Result to a dictionary
            case ServiceResponse<SpTaskDto> { Data.StatusId: false } resultTaskSp:
                _logger.Error("JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Failed", resultTaskSp.Data?.StatusName);
                break;
            case ServiceResponse<SpTaskDto> resultTaskSp:
                _logger.Information("JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Succeeded", resultTaskSp.Data?.StatusName);
                break;
            case ServiceResponse<string> resultString when context.Exception != null:
                _logger.Error(context.Exception, "JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Failed", resultString.Errors);
                break;
            case ServiceResponse<string> resultString:
                _logger.Information("JobId: {JobId}, {MethodName} with Room {RoomId} {Status}, Result: {Result}", context.BackgroundJob.Id, methodName, roomId, "Succeeded", resultString.Data);
                break;
        }
    }
}