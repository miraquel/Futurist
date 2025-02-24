using Hangfire;

namespace Futurist.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private BackgroundJobServer _backgroundJobServer;
 
    public Worker(ILogger<Worker> logger, BackgroundJobServer backgroundJobServer)
    {
        _logger = logger;
        _backgroundJobServer = backgroundJobServer;
        
        GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection");
    }
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _backgroundJobServer = new BackgroundJobServer();
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _backgroundJobServer.Dispose();
        return base.StopAsync(cancellationToken);
    }
}