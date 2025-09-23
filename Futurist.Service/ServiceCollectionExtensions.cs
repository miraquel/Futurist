using Futurist.Service.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Futurist.Service;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        // TODO: Add services here
        services.AddScoped<MapperlyMapper>();
        services.AddScoped<IRofoService, RofoService>();
        services.AddScoped<IBomStdService, BomStdService>();
        services.AddScoped<IMupService, MupService>();
        services.AddScoped<IFgCostService, FgCostService>();
        services.AddScoped<IJobMonitoringService, JobMonitoringService>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        services.AddScoped<IItemAdjustmentService, ItemAdjustmentService>();
        services.AddScoped<IReportVersionService, ReportVersionService>();
        services.AddScoped<IItemForecastService, ItemForecastService>();
        services.AddScoped<IScmReportService, ScmReportService>();
        services.AddScoped<IAnlRmService, AnlRmService>();
        services.AddScoped<IRmForecastService, RmForecastService>();
        services.AddScoped<ICurrentDataService, CurrentDataService>();
    }
}